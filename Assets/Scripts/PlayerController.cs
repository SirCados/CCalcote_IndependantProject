using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _accelerationRate;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;

    public bool IsGrounded = true;
    InputAction _jumpAction;
    InputAction _moveAction;
    PlayerInput _playerInput;
    Rigidbody _playerRigidBody;
    Vector2 _inputVector;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerRigidBody = GetComponent<Rigidbody>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
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
    }

    void MovePlayer()
    {
        Vector3 currentVelocity = _playerRigidBody.velocity;
        _inputVector = _moveAction.ReadValue<Vector2>();
        float fallingMagnitude = (IsGrounded) ? 0 : -_fallRate;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(_inputVector.x, fallingMagnitude, _inputVector.y) * _movementSpeed);        
        
        Vector3 velocityChange = (targetVelocity - currentVelocity) * _accelerationRate;

        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);       
    }
    
    void JumpPlayer(InputAction.CallbackContext context)
    {
        Vector3 airVelocity = Vector3.zero; 

        if (_jumpAction.WasPressedThisFrame() && _jumpAction.IsPressed() && IsGrounded)
        {
            airVelocity = Vector3.up * _jumpForce;
            print("jump");
        }
        else if (_jumpAction.WasPressedThisFrame() && _jumpAction.IsPressed() && !IsGrounded)
        {
            airVelocity = Vector3.ClampMagnitude(new Vector3(_inputVector.x, 0, _inputVector.y) * _movementSpeed, 10);
            print("dash");
        }

        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
    }

    void SubscribeToEvents()
    {
        _jumpAction.started += JumpPlayer;
        _jumpAction.performed += JumpPlayer;
        _jumpAction.canceled += JumpPlayer;
    }

    void UnsubscribeToEvents()
    {
        _jumpAction.started -= JumpPlayer;
        _jumpAction.performed -= JumpPlayer;
        _jumpAction.canceled -= JumpPlayer;
    }
}
