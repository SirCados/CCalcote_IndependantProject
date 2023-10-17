using UnityEngine;

public class AvatarAspect : MonoBehaviour
{
    public bool IsGrounded = true;    

    [SerializeField] float _airDashSpeedLimit;
    [SerializeField] float _accelerationRate;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;
    [SerializeField] int _maxiumAirDashes;
    [SerializeField] int _remainingAirDashes;
    [SerializeField] GameObject _facingIndicator;

    GameObject _currentTarget;
    Rigidbody _playerRigidBody;
    public Vector2 InputVector;
    Vector3 _dashTargetPosition;

    private void Awake()
    {
        SetupAvatarAspect();
    }

    private void FixedUpdate()
    {        
        //RotateCharacter();
    }

    public void PerformNeutral()
    {
        Vector3 velocityChange = (Vector3.zero - _playerRigidBody.velocity) * _accelerationRate;
        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public void PerformMove(Vector2 inputVector)
    {
        InputVector = inputVector;
        Transform parent = GetComponentInParent<Transform>();
        parent.TransformDirection(Vector3.forward);
        Vector3 currentVelocity = _playerRigidBody.velocity;       
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 10;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);
        targetVelocity.y = (IsGrounded) ? 0 : -_fallRate;
        Vector3 velocityChange = (targetVelocity - currentVelocity) * _accelerationRate;

        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public void PerformJump(IState currentState, Vector2 inputVector)
    {
        Vector3 airVelocity = Vector3.up * _jumpForce;
        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
    }

    public void PerformAirDash(IState currentState, Vector2 inputVector) //seperate out into Avatar object. Avatar Object will handle all movement. Player Controller will tell avatar to move. 
    {
        if (!currentState.Equals(typeof(DashState)) && _remainingAirDashes != 0)
        {
            Vector3 dashVector = new Vector3(inputVector.x, 0, inputVector.y);
            _dashTargetPosition = transform.position + (dashVector * _movementSpeed * 2);
            _remainingAirDashes -= 1;
            Vector3 dashVelocity = Vector3.ClampMagnitude(new Vector3(inputVector.x, 0, inputVector.y) * _movementSpeed, _airDashSpeedLimit);
            _playerRigidBody.MovePosition(dashVelocity);
        }
    }

    void RotateCharacter()
    {
        if (_currentTarget)
        {
            _facingIndicator.transform.LookAt(_currentTarget.transform);
        }
    }

    public void ResetAirDashes()
    {
        _remainingAirDashes = _maxiumAirDashes;
    } 
    
    void SetupAvatarAspect()
    {
        ResetAirDashes();
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _currentTarget = GetComponentInParent<PlayerController>().CurrentTarget; //account for target switch in PlayerController
    }
}
