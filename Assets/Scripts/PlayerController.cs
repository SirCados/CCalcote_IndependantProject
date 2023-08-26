using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    PlayerInput _playerInput;
    Rigidbody _playerRigidBody;
    InputAction _moveAction;
    InputAction _jumpAction;
    string _airActionMap = "Air";
    string _groundActionMap = "Ground";
    public bool _isGrounded = true;
    

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerRigidBody = GetComponent<Rigidbody>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        JumpPlayer();
        GroundCheck();
    }

    void MovePlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        if (_playerInput.currentActionMap.name == _groundActionMap)
        {
            _playerRigidBody.AddForce(movementVector.normalized * 5f, ForceMode.Force);
        }
        else if (_playerInput.currentActionMap.name == _airActionMap)
        {
            _playerRigidBody.AddForce(movementVector.normalized * 1f, ForceMode.Force);
        }
        
    }

    void JumpPlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        if (_jumpAction.IsPressed() && _isGrounded)
        {
            Vector3 movementVector = new Vector3(0, 1, 0);
            _playerRigidBody.AddForce(movementVector.normalized * 2f, ForceMode.Impulse);
            print("jump");
            _isGrounded = false;
        } 
        else if (_jumpAction.IsPressed() && !_isGrounded)
        {
            Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
            _playerRigidBody.AddForce(movementVector.normalized * 5f, ForceMode.Impulse);
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
