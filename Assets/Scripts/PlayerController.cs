using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public string CurrentState;

    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public bool IsGrounded = true;

    public GameObject CurrentTarget;

    [SerializeField] GameObject _facingIndicator;
    Rigidbody _playerRigidBody;

    int _remainingAirDashes;
    InputAction _jumpAction;
    InputAction _moveAction;
    InputAction _barrageAction;
    PlayerInput _playerInput;
    Vector2 _inputVector;

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
        print("Changing to " + newState);
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
        }
    }

    void JumpOrAirDash(InputAction.CallbackContext context)
    {
        
    }

    void GetInputsForMovement()
    {
        Vector2 inputs = (_currentState == _activeState) ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        _activeState.SetInputs(inputs);
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

        _playerRigidBody = GetComponent<Rigidbody>();
    }
}
