using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
    AirborneState _airborneState;
    BarrageState _barrageState;
    DashState _dashState;
    NeutralState _neutralState;
    MoveState _moveState;

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
        //ManifestedAvatar.MoveAvatar(_currentState, _inputVector);
        //Move();
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
        if(_currentState == _neutralState)
        {
            ChangeState(_moveState);
        }

        if(_currentState == _moveState)
        {
            print(_moveAction.ReadValue<Vector2>());
            _moveState.SetInputs(_moveAction.ReadValue<Vector2>());
        }
    }

    void Neutral(InputAction.CallbackContext context)
    {
        ChangeState(_neutralState);
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
        _moveAction.canceled += Neutral;

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
        _moveAction.canceled -= Neutral;
    }

    void SetupCharacterController()
    {
        _playerInput = GetComponent<PlayerInput>();

        _barrageAction = _playerInput.actions["Barrage"];
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];        

        _neutralState = new NeutralState(ManifestedAvatar);
        ChangeState(_neutralState);
        _moveState = new MoveState(_neutralState, ManifestedAvatar);
        _airborneState = new AirborneState(_neutralState, ManifestedAvatar);                
        _dashState = new DashState(_airborneState, ManifestedAvatar);
        _barrageState = new BarrageState(_neutralState, ManifestedBarrage);

        _playerRigidBody = GetComponent<Rigidbody>();
    }
}
