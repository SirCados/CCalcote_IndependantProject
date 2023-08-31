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
    float _fallingVelocity = 0;
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

    private void Update()
    {
        CheckIfGrounded();
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
        
        if (_jumpAction.WasPressedThisFrame() && _characterController.isGrounded)
        {
            print("Jump");
            _fallingVelocity = Mathf.Sqrt(-2 * _fallRate * _jumpHeight);           
        }

        _fallingVelocity += Time.deltaTime * _fallRate;

        //add the smooth damp to smoothly rise to max jump height

        Vector3 movementVector = new Vector3(inputVector.x, _fallRate, inputVector.y) + (Vector3.up * _fallingVelocity);

        if (_characterController.isGrounded) //not sure if I need the seperate action maps
        {
            if(_fallingVelocity != 0)
            {
                _fallingVelocity = 0;
            }
            //How to handle accelleration?
            _characterController.Move(movementVector * _movementSpeed * Time.deltaTime);
        }
        else if (!_characterController.isGrounded)
        {
            _characterController.Move(movementVector * (_movementSpeed / 5) * Time.deltaTime);
        }

    }

    void CheckIfGrounded()
    {
        print(_characterController.isGrounded);
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

===From Endless Jumper===

[Range(0, 1)]
    float _airWalk;

float GetModifiedSmoothTime(float smoothTime)
    {
        if (_characterController.isGrounded || !_isObeyingGravity || _isSuperFalling)
        {
            return smoothTime;
        }
        if (_airWalk == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / _airWalk;
    }

==IN MOVE FUNCTION==

            float targetSpeed = ((isRunning) ? _runSpeed : _walkSpeed) * inputDirection.magnitude;
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(SpeedSmoothTime));

            _gravityVelocity += Time.deltaTime * ((_isSuperFalling) ? Gravity/2 : Gravity);

            Vector3 movement = transform.forward * ((_isSuperFalling) ? 30 : _currentSpeed) + Vector3.up * _gravityVelocity;
        
            _characterController.Move(movement * Time.deltaTime);
            _currentSpeed = new Vector2(_characterController.velocity.x, _characterController.velocity.z).magnitude;

*/
