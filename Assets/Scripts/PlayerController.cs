using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IController
{
    public GameObject[] Avatars;

    public string CurrentState;
    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public BlastAspect ManifestedBlast;
    public EnemyController Enemy;
    public Transform CurrentTarget;

    [SerializeField][Range(0, 1)] float _TimeScale = 1;
        
    InputAction _jumpAction;
    InputAction _moveAction;
    InputAction _barrageAction;
    InputAction _blastAction;
    InputAction _aimXAction;
    InputAction _aimYAction;
    PlayerInput _playerInput;

    IState _currentState = new EmptyState();
    ActiveState _activeState;
    BarrageState _barrageState;
    BlastState _blastState;
    DashState _dashState;
    DownState _downState;
    GetUpState _getUpState;

    bool _isAiming = false;

    AvatarSelect _avatarSelect;

    public enum AvatarType
    {
        BALANCED,        
        HEAVY,
        FLOATY,
        SWIFT
    }

    public AvatarType AvatarToManifest;

    private void Awake()
    {
        AvatarToManifest = (AvatarType)GameObject.Find("AvatarSelect").GetComponent<AvatarSelect>().SelectedAvatar;
        StartCoroutine(GetTargetAvatar());        
        Time.timeScale = _TimeScale;
    }


    private void OnDisable()
    {
        //UnsubscribeToEvents();
    }

    private void FixedUpdate()
    {
        if (!ManifestedAvatar.IsGameOver)
        {
            //change to switch?
            if (_currentState != _downState && ManifestedAvatar.IsKnockedDown)
            {
                print("knocked down");
                ChangeState(_downState);
            }
            else if (!ManifestedAvatar.IsInHitStun)
            {
                StateControllerUpdate();
                GetInputsForMovement();
                if (_isAiming)
                {
                    GetInputsForAiming();
                }
            }
        }
    }

    IEnumerator GetTargetAvatar()
    {
        ManifestedAvatar = ManifestAvatar().GetComponent<AvatarAspect>();
        yield return new WaitForEndOfFrame();
        if (Enemy.GetComponentInChildren<AvatarAspect>() != null)
            CurrentTarget = Enemy.GetComponentInChildren<AvatarAspect>().transform;
        if (CurrentTarget != null)
        {
            SetupCharacterController();
            StopCoroutine(GetTargetAvatar());
        }
        else
            StartCoroutine(GetTargetAvatar());
    }

    public void StateControllerUpdate()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdateState();
            CurrentState = _currentState.ToString();
        }

        if (_currentState.NextState != null && _currentState.IsStateDone)
        {
            ChangeState(_currentState.NextState);
        }
    }

    public void ChangeState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExitState();
        }

        _currentState = newState;
        _currentState.OnEnterState();        
    }

    void GetInputsForMovement()
    {
        Vector2 inputs = (_currentState == _activeState && !ManifestedBarrage.IsRecovering) ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        if(_currentState == _activeState)
            _activeState.SetInputs(inputs);
    }

    void GetInputsForAiming()
    {
        float aimX = _aimXAction.ReadValue<float>();
        float aimY = _aimYAction.ReadValue<float>();

        Vector2 aimInputs = new Vector2(aimX, -aimY).normalized;
        _blastState.SetAimInputs(aimInputs);
        Vector2 movementInputs = _moveAction.ReadValue<Vector2>();        
        _blastState.SetMovementInputs(movementInputs);
    }

    void Barrage(InputAction.CallbackContext context)
    {
        if(_currentState == _activeState && !ManifestedBarrage.IsRecovering)
        {
            ChangeState(_barrageState);
            ManifestedAvatar.StopJumpVelocity();
        }
        else
        {
            PlayErrorSound();
        }
    }

    void Blast(InputAction.CallbackContext context)
    {
        if (_currentState == _activeState && !ManifestedBarrage.IsRecovering && !ManifestedBlast.IsProjectileActive && !ManifestedAvatar.IsDashing)
        {
            _isAiming = true;
            ChangeState(_blastState);
            ManifestedAvatar.StopJumpVelocity();
            ManifestedAvatar.SlowMoveVelocity();
        }
        else
        {
            PlayErrorSound();
        }
    }

    void Neutral(InputAction.CallbackContext context)
    {
        if(_currentState != _activeState && !ManifestedAvatar.IsDashing)
        {
            _isAiming = false;
            ChangeState(_activeState);
        }
    }

    void JumpOrAirDash(InputAction.CallbackContext context)
    {
        if (_currentState == _activeState && !ManifestedBarrage.IsRecovering && !ManifestedBlast.IsBlasting)
        {
            if (!ManifestedAvatar.IsGrounded && ManifestedAvatar.RemainingAirDashes != 0)
            {
                Vector2 inputs = _moveAction.ReadValue<Vector2>();
                _dashState.SetInputs(inputs);
                ChangeState(_dashState);
            }
            else if(ManifestedAvatar.IsGrounded)
            {
                _activeState.IsJumping = true;
            }
            else
            {
                PlayErrorSound();
            }
        }
        else
        {
            PlayErrorSound();
        }
    }

    GameObject ManifestAvatar()
    {
        AvatarType avatar = AvatarToManifest;
        GameObject manifestedAvatar;
        switch (avatar)
        {
            case AvatarType.BALANCED:
                manifestedAvatar = Instantiate(Avatars[0], transform);
                return manifestedAvatar;
            case AvatarType.HEAVY:
                manifestedAvatar = Instantiate(Avatars[1], transform);
                return manifestedAvatar;
            case AvatarType.FLOATY:
                manifestedAvatar = Instantiate(Avatars[2], transform);
                return manifestedAvatar;
            case AvatarType.SWIFT:
                manifestedAvatar = Instantiate(Avatars[3], transform);
                return manifestedAvatar;
        }
        return null;
    }

    void PlayErrorSound()
    {

    }

    public Transform Target
    {
        get => CurrentTarget;
        set => CurrentTarget = value;
    }

    void SubscribeToEvents()
    {
        _barrageAction.started += Barrage;
        _jumpAction.started += JumpOrAirDash;
        _blastAction.started += Blast;
        _blastAction.canceled += Neutral;
    }

    void UnsubscribeToEvents()
    {
        _barrageAction.started -= Barrage;
        _jumpAction.started -= JumpOrAirDash;
        _blastAction.started -= Blast;
        _blastAction.canceled -= Neutral;
    }

    void SetupCharacterController()
    {        
        ManifestedBarrage = ManifestedAvatar.GetComponentInChildren<BarrageAspect>();
        ManifestedBlast = ManifestedAvatar.GetComponentInChildren<BlastAspect>();
        CurrentTarget = Enemy.GetComponentInChildren<AvatarAspect>().transform;
        ManifestedAvatar.CurrentTarget = CurrentTarget;
        ManifestedBarrage.CurrentTarget = CurrentTarget;
        ManifestedBlast.CurrentTarget = CurrentTarget;
        _playerInput = GetComponent<PlayerInput>();


        _aimXAction = _playerInput.actions["AimX"];
        _aimYAction = _playerInput.actions["AimY"];
        _barrageAction = _playerInput.actions["Barrage"];
        _blastAction = _playerInput.actions["Blast"];
        _jumpAction = _playerInput.actions["Jump"];
        _moveAction = _playerInput.actions["Move"];

        _activeState = new ActiveState(ManifestedAvatar);
        ChangeState(_activeState);
        _barrageState = new BarrageState(_activeState, ManifestedBarrage);
        _blastState = new BlastState(_activeState, ManifestedAvatar, ManifestedBlast);
        _dashState = new DashState(_activeState, ManifestedAvatar);
        _getUpState = new GetUpState(_activeState, ManifestedAvatar);
        _downState = new DownState(_getUpState, ManifestedAvatar);

        SubscribeToEvents();
        
    }
}
