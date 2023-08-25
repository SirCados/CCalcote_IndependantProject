using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    PlayerInput _playerInput;
    Rigidbody _playerRigidBody;
    InputAction _moveAction;
    InputAction _jumpAction;

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

    void MovePlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);

        _playerRigidBody.AddForce(movementVector.normalized * 10f, ForceMode.Force);
    }
}
