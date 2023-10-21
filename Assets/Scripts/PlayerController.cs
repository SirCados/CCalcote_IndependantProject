using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public string CurrentState;
    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public GameObject CurrentTarget;

    [SerializeField] Animator _animator;
    [SerializeField] GameObject _facingIndicator;   
    
    InputAction _jumpAction;
    InputAction _moveAction;
    InputAction _barrageAction;
    PlayerInput _playerInput;

    IState _currentState;
    ActiveState _activeState;
    BarrageState _barrageState;
    DashState _dashState;

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
        StateControllerUpdate();
        GetInputsForMovement();
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

    void Barrage(InputAction.CallbackContext context)
    {
        if(_currentState == _activeState)
        {
            ChangeState(_barrageState);
            ManifestedAvatar.StopJumpVelocity();
        }
    }

    void JumpOrAirDash(InputAction.CallbackContext context)
    {
        if (_currentState == _activeState)
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

    void GetInputsForMovement()
    {
        if(_currentState == _activeState)
        {
            Vector2 inputs = (_currentState == _activeState) ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            //_animator.transform.forward = transform.forward;
            _activeState.SetInputs(inputs);
            _animator.SetFloat("xInput", inputs.x);
            _animator.SetFloat("yInput", inputs.y);
            float movement = Mathf.Abs(inputs.magnitude);
            _animator.SetFloat("Movement", movement);

        }
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
