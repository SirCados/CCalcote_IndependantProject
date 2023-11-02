using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject[] Avatars;

    public string CurrentState;
    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public Transform CurrentTarget;
        
    InputAction _jumpAction;
    InputAction _moveAction;
    InputAction _barrageAction;
    PlayerInput _playerInput;

    IState _currentState;
    ActiveState _activeState;
    BarrageState _barrageState;
    DashState _dashState;

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
        SetupCharacterController();
    }

    private void OnEnable()
    {
        SubscribeToEvents();        
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    private void FixedUpdate()
    {
        if (!ManifestedAvatar.IsGameOver)
        {
            StateControllerUpdate();
            GetInputsForMovement();
        }
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
        _activeState.SetInputs(inputs);
    }

    void Barrage(InputAction.CallbackContext context)
    {
        if(_currentState == _activeState && !ManifestedBarrage.IsRecovering)
        {
            ChangeState(_barrageState);
            ManifestedAvatar.StopJumpVelocity();
        }
    }

    void JumpOrAirDash(InputAction.CallbackContext context)
    {
        if (_currentState == _activeState && !ManifestedBarrage.IsRecovering)
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

    void SubscribeToEvents()
    {
        _barrageAction.started += Barrage;
        _jumpAction.started += JumpOrAirDash;
    }

    void UnsubscribeToEvents()
    {
        _barrageAction.started -= Barrage;
        _jumpAction.started -= JumpOrAirDash;
    }

    void SetupCharacterController()
    {
        ManifestedAvatar = ManifestAvatar().GetComponent<AvatarAspect>();
        ManifestedBarrage = ManifestedAvatar.GetComponentInChildren<BarrageAspect>();
        _playerInput = GetComponent<PlayerInput>();

        _barrageAction = _playerInput.actions["Barrage"];
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];

        _activeState = new ActiveState(ManifestedAvatar);
        ChangeState(_activeState);               
        _dashState = new DashState(_activeState, ManifestedAvatar);
        _barrageState = new BarrageState(_activeState, ManifestedBarrage);
    }
}
