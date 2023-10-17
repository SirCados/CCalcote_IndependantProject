using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public AvatarAspect ManifestedAvatar;
    public BarrageAspect ManifestedBarrage;
    public bool IsGrounded = true;

    public GameObject CurrentTarget;

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
    NeutralState _neutralState;

    Rigidbody _playerRigidBody;



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
        Move();
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

    void Move()
    {
        Vector3 currentVelocity = _playerRigidBody.velocity;
        _inputVector = (_currentState == _neutralState) ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        float speed = (IsGrounded) ? 30 : 3;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(_inputVector.x, 0, _inputVector.y) * speed);

        targetVelocity.y = (IsGrounded || _currentState == _dashState) ? 0 : -10;

        Vector3 velocityChange = (targetVelocity - currentVelocity) * 10;

        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }


    void SubscribeToEvents()
    {
        _barrageAction.started += Barrage;
        //_barrageAction.performed += Barrage;
        //_barrageAction.canceled += Barrage;

        _jumpAction.started += Jump;
        //_jumpAction.performed += JumpPlayer;
        //_jumpAction.canceled += JumpPlayer;

    
    }

    void UnsubscribeToEvents()
    {
        _barrageAction.started -= Barrage;
        //_barrageAction.performed -= Barrage;
        //_barrageAction.canceled -= Barrage;

        _jumpAction.started -= Jump;
        //_jumpAction.performed -= JumpPlayer;
        //_jumpAction.canceled -= JumpPlayer;

    }

    void SetupCharacterController()
    {
        _playerInput = GetComponent<PlayerInput>();

        _barrageAction = _playerInput.actions["Barrage"];
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];        

        _neutralState = new NeutralState();
        ChangeState(_neutralState);
        _airborneState = new AirborneState();                
        _dashState = new DashState();
        _dashState.AvatarBody = ManifestedAvatar;
        _dashState.NextState = _airborneState; //make constructors

        _barrageState = new BarrageState(ManifestedBarrage);
        _barrageState.NextState = _neutralState;

        _playerRigidBody = GetComponent<Rigidbody>();
    }
}
