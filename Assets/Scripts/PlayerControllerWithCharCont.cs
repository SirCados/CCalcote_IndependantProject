using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWithCharCont : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _moveAction;
    InputAction _jumpAction;
    CharacterController _characterController;

    [SerializeField] float _movementSpeed;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _fallRate;
    //Look at how Jump is achieved here
    //https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

    //events for movement?
    //https://onewheelstudio.com/blog/2022/4/15/input-event-handlers-or-adding-juice-the-easy-way

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    private void FixedUpdate()
    {
        MovePlayer();
        JumpPlayer();
    }

    void MovePlayer()
    {
        
    }
}

/*
 
Vector2 inputVector = _moveAction.ReadValue<Vector2>();

        Vector3 movementVector = new Vector3(inputVector.x, -_fallRate, inputVector.y);
        if (_characterController.isGrounded) //not sure if I need the seperate action maps
        {
            _characterController.Move(movementVector * _movementSpeed * Time.deltaTime);
        }
        else if (!_characterController.isGrounded)
        {
            _characterController.Move(movementVector * (_movementSpeed / 5) * Time.deltaTime);
        }

    }

    void JumpPlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        if (_jumpAction.IsPressed() && _characterController.isGrounded)
        {
            Vector3 movementVector = Vector3.up;            
            _characterController.Move(movementVector * _jumpHeight * Time.deltaTime);
            print("jump");
        }
        else if (_jumpAction.IsPressed() && !_characterController.isGrounded)
        {
            Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
            _characterController.Move(movementVector * (_movementSpeed * 2) * Time.deltaTime);
        }

*/
