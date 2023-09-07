using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput _playerInput;
    Rigidbody _playerRigidBody;
    InputAction _moveAction;
    InputAction _jumpAction;
    public bool _isGrounded = true;

    [SerializeField] float _movementSpeed;
    [SerializeField] float _jumpHeight;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerRigidBody = GetComponent<Rigidbody>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    private void Update()
    {
        //MovePlayer();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        JumpPlayer();
        GroundCheck();
    }

    void MovePlayer()
    {
        print("move");
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        if (_moveAction.IsPressed() && _isGrounded)
        {
            //Vector3 newPosition = transform.position + movementVector * _movementSpeed * Time.deltaTime;
            //_playerRigidBody.MovePosition(newPosition);
            _playerRigidBody.velocity = Vector3.zero;
            _playerRigidBody.AddForce(movementVector.normalized * _movementSpeed, ForceMode.Force);
        }
        else if (_moveAction.IsPressed() && !_isGrounded)
        {
            //Vector3 newPosition = transform.position + movementVector * (_movementSpeed / 5) * Time.deltaTime;
            //_playerRigidBody.MovePosition(newPosition);
            _playerRigidBody.AddForce(movementVector.normalized * (_movementSpeed / 5), ForceMode.Force);
        }        
    }

    void JumpPlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        if (_jumpAction.WasPressedThisFrame() && _jumpAction.IsPressed() && _isGrounded)
        {
            Vector3 movementVector = new Vector3(0, 1, 0);
            _playerRigidBody.AddForce(movementVector.normalized * _jumpHeight, ForceMode.Impulse);
            print("jump");
            _isGrounded = false;
        } 
        else if (_jumpAction.WasPressedThisFrame() && _jumpAction.IsPressed() && !_isGrounded)
        {
            Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
            Vector3 newPosition = transform.position + movementVector * (_movementSpeed / 5) * Time.deltaTime;
            _playerRigidBody.MovePosition(newPosition);
            //_playerRigidBody.AddForce(movementVector.normalized * (_movementSpeed * 2), ForceMode.Impulse);
            //needs to stop after a specific distance traveled
        }
    }

    void GroundCheck()
    {
        float groundCheckDistance = (GetComponent<CapsuleCollider>().height / 2) + 0.1f;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance))
        {
            _isGrounded = true;
        }
    }
}
