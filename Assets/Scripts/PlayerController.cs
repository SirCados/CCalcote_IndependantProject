using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool IsGrounded = true;
    public GameObject CurrentTarget;
    public GameObject BarrageProjectile;
    public GameObject BarrageEmmissionPoint;

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
        MovePlayer();
        RotateCharacter();
    }

    void MovePlayer()
    {
        Vector3 currentVelocity = _playerRigidBody.velocity;
        _inputVector = _moveAction.ReadValue<Vector2>();
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 10;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(_inputVector.x, 0, _inputVector.y) * speed);

        targetVelocity.y = (IsGrounded) ? 0 : -_fallRate;        

        Vector3 velocityChange = (targetVelocity - currentVelocity) * _accelerationRate;

        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    void JumpPlayer(InputAction.CallbackContext context)
    {
        Vector3 airVelocity = Vector3.zero;

        if (IsGrounded)
        {
            airVelocity = Vector3.up * _jumpForce;
            print("jump");
        }
        else if (_jumpAction.WasPressedThisFrame() && !IsGrounded && _remainingAirDashes !=0)
        {
            _remainingAirDashes -= 1;
            airVelocity = Vector3.ClampMagnitude(new Vector3(_inputVector.x, 0, _inputVector.y) * _movementSpeed, _airDashSpeedLimit);
            print("dash");
        }

        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
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
        print("barrage");
        BarrageProjectile projectile = Instantiate(BarrageProjectile, BarrageEmmissionPoint.transform).GetComponent<BarrageProjectile>();
        projectile.Target = CurrentTarget.transform;
        projectile.TargetRigidBody = CurrentTarget.GetComponent<Rigidbody>();
        BarrageEmmissionPoint.transform.DetachChildren();
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

        _jumpAction.started += JumpPlayer;
        //_jumpAction.performed += JumpPlayer;
        //_jumpAction.canceled += JumpPlayer;
    }

    void UnsubscribeToEvents()
    {

        _barrageAction.started -= Barrage;
        //_barrageAction.performed -= Barrage;
        //_barrageAction.canceled -= Barrage;

        _jumpAction.started -= JumpPlayer;
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
    }
}
