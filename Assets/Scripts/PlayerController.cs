using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool IsGrounded = true;
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
    Rigidbody _playerRigidBody;
    Vector2 _inputVector;

    BarrageState _barrageState;
    DashState _dashState;
    NeutralState _neutralState;
    IState _currentState;

    Vector3 _dashTargetPosition;

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
        Move();
        RotateCharacter();
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

    void Move()
    {
        Vector3 currentVelocity = _playerRigidBody.velocity;
        _inputVector = (_currentState == _neutralState)? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 10;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(_inputVector.x, 0, _inputVector.y) * speed);

        targetVelocity.y = (IsGrounded || _currentState == _dashState) ? 0 : -_fallRate;        

        Vector3 velocityChange = (targetVelocity - currentVelocity) * _accelerationRate;

        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    void Jump(InputAction.CallbackContext context)
    {
        Vector3 airVelocity = Vector3.zero;

        if (IsGrounded)
        {
            airVelocity = Vector3.up * _jumpForce;
            _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
        }
        //else if (_jumpAction.WasPressedThisFrame() && !IsGrounded && _remainingAirDashes !=0)
        //{
        //    AirDash();
        //}
    }

    void AirDash() //seperate out into Avatar object. Avatar Object will handle all movement. Player Controller will tell avatar to move. 
    {
        Vector3 inputVector;
        if(_currentState != _dashState)
        {
            inputVector = new Vector3(_inputVector.x, 0, _inputVector.y);
            ChangeState(_dashState);
            _dashTargetPosition = transform.position + (inputVector * _movementSpeed * 2);
            _remainingAirDashes -= 1;
        }
        
        Vector3 dashVelocity = Vector3.ClampMagnitude(new Vector3(_inputVector.x, 0, _inputVector.y) * _movementSpeed, _airDashSpeedLimit);
        _playerRigidBody.MovePosition(dashVelocity);
    }

    void RotateCharacter()
    {
        if (CurrentTarget)
        {
            _facingIndicator.transform.LookAt(CurrentTarget.transform);
        }
    }

    void Barrage(InputAction.CallbackContext context)
    {
        if(_currentState == _neutralState)
        {
            print("Press");
            ChangeState(_barrageState);
        }
    }

    public void ResetAirDashes()
    {
        _remainingAirDashes = _maxiumAirDashes;
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
        _playerRigidBody = GetComponent<Rigidbody>();

        _barrageAction = _playerInput.actions["Barrage"];

        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        ResetAirDashes();

        _neutralState = new NeutralState();
        ChangeState(_neutralState);        
        _barrageState = new BarrageState(ManifestedBarrage);
        _barrageState.NextState = _neutralState;
        _dashState = new DashState();
        _dashState.TempBody = this;
        _dashState.NextState = _neutralState;//make constructors
    }
}
