using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWithCharCont : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _moveAction;
    InputAction _jumpAction;
    CharacterController _characterController;
    Vector2 _storedInputVector;
    Vector3 _currentMovement;

    float _currentSpeed = 0;
    float _fallingVelocity = 0;
    float _speedSmoothTime = 0.01f;
    float _gravity;
    float _jumpTime = 0.5f;
    float _initialJumpVelocity;

    [SerializeField][Range(0, 1)] float _airWalk;

    [SerializeField] float _rateOfAcceleration;
    [SerializeField] float _maximumSpeed;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _fallRate;

    bool _isJumpPressed = false;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _moveAction = _playerInput.actions["Move"];

        _jumpAction = _playerInput.actions["Jump"];

        _jumpAction.started += OnJump;
        _jumpAction.canceled += OnJump;
               
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
     
        Vector3 movementVector = new Vector3(inputVector.x, -_fallRate, inputVector.y) * _maximumSpeed * Time.deltaTime;

        _currentMovement = movementVector;
       
        _characterController.Move(_currentMovement);
    }

    void SetupJumpVariables()
    {
        _gravity = (-2 * _jumpHeight) / Mathf.Pow(_jumpTime/2, 2);
        _initialJumpVelocity = (2 * _jumpHeight) / _jumpTime;
    }

    void HandleJump()
    {
        if (_isJumpPressed)
        {
            _currentMovement.y += _initialJumpVelocity; 
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (_characterController.isGrounded)
        {
            return smoothTime;
        }
        if (_airWalk == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / _airWalk;
    }
}
