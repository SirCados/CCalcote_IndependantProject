using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerWithCharCont : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction _moveAction;
    InputAction _jumpAction;
    CharacterController _characterController;
    Vector2 _storedInputVector;

    float _currentSpeed = 0;
    float _fallingVelocity = 0;
    float _speedSmoothTime = 0.01f;    

    [SerializeField] float _rateOfAcceleration;
    [SerializeField] float _maximumSpeed;
    [SerializeField] float _jumpHeight;
    [SerializeField] float _fallRate;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 inputVector = _moveAction.ReadValue<Vector2>();
     
        Vector3 movementVector = new Vector3(inputVector.x, -_fallRate, inputVector.y) * _maximumSpeed * Time.deltaTime;
       
        _characterController.Move(movementVector);
    }    
}
