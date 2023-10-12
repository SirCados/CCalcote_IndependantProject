using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;

    public GameObject CurrentTarget;

    [SerializeField] float _airDashSpeedLimit;
    [SerializeField] float _accelerationRate;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;
    [SerializeField] int _maxiumAirDashes;
    [SerializeField] GameObject _facingIndicator;
    
    int _remainingAirDashes;
    InputAction _jumpAction;
    InputAction _moveAction;
    InputAction _barrageAction;
    PlayerInput _playerInput;
    Vector2 _inputVector;

    IState _currentState;
    AirborneState _airborneState;
    BarrageState _barrageState;
    DashState _dashState;
    MoveState _moveState;
    NeutralState _neutralState;
    

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
    private void Update()
    {
        StateControllerUpdate();
    }

    private void FixedUpdate()
    {
        if(_currentState == _moveState)
        {
            ManifestedAvatar.MoveAvatar(_currentState, _inputVector);
        }        
    }

    public void StateControllerUpdate()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdateState();
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
        if(_currentState == _neutralState)
        {
            ChangeState(_barrageState);
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (_currentState != _barrageState || _currentState != _dashState)
        {
            ChangeState(_airborneState);
        }
        else if (_currentState == _airborneState && _remainingAirDashes != 0)
        {
            ChangeState(_dashState);
        }
    }

    void Move(InputAction.CallbackContext context)
    {
        if (_currentState != _neutralState || _currentState != _airborneState)
        {
            ChangeState(_moveState);
            _inputVector = _moveAction.ReadValue<Vector2>();
        }
    }

    void SubscribeToEvents()
    {
        _barrageAction.started += Barrage;
        //_barrageAction.performed += Barrage;
        //_barrageAction.canceled += Barrage;

        _jumpAction.started += Jump;
        //_jumpAction.performed += JumpPlayer;
        //_jumpAction.canceled += JumpPlayer;

        _moveAction.started += Move;
        _moveAction.performed += Move;
    }

    void UnsubscribeToEvents()
    {
        _barrageAction.started -= Barrage;
        //_barrageAction.performed -= Barrage;
        //_barrageAction.canceled -= Barrage;

        _jumpAction.started -= Jump;
        //_jumpAction.performed -= JumpPlayer;
        //_jumpAction.canceled -= JumpPlayer;

        _moveAction.started -= Move;
        _moveAction.performed -= Move;
    }

    void SetupCharacterController()
    {
        _playerInput = GetComponent<PlayerInput>();

        _barrageAction = _playerInput.actions["Barrage"];
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];        

        _neutralState = new NeutralState();
        ChangeState(_neutralState);
        _moveState = new MoveState();
        _airborneState = new AirborneState();                
        _dashState = new DashState();
        _dashState.AvatarBody = ManifestedAvatar;
        _dashState.NextState = _airborneState; //make constructors

        _barrageState = new BarrageState(ManifestedBarrage);
        _barrageState.NextState = _neutralState;
    }
}
