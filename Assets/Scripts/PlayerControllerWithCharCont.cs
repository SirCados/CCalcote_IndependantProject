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

    [SerializeField][Range(0, 1)] float _airWalk;

    [SerializeField] float _rateOfAcceleration;
    [SerializeField] float _maximumSpeed;
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
