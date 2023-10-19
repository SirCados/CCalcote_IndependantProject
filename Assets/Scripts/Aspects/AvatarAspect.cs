using UnityEngine;

public class AvatarAspect : MonoBehaviour
{
    public bool IsGrounded = true;
    public bool IsDashing = false;
    public int RemainingAirDashes;

    [SerializeField] float _airDashSpeedLimit;
    [SerializeField] float _accelerationRate;
    [SerializeField] float _dashDistance = 10f;
    [SerializeField] float _dashSpeed = .2f;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;
    [SerializeField] int _maxiumAirDashes;    
    [SerializeField] GameObject _facingIndicator;

    GameObject _currentTarget;
    Rigidbody _playerRigidBody;
    public Vector2 InputVector;
    Vector3 _dashTargetPosition;

    private void Awake()
    {
        SetupAvatarAspect();
    }

    private void Update()
    {
        RotateCharacter();
    }

    public void PerformMove(Vector2 inputVector)
    {
        InputVector = inputVector;       
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 3;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);        
        Vector3 velocityChange = (targetVelocity - _playerRigidBody.velocity) * _accelerationRate;
        velocityChange.y = (IsGrounded) ? 0 : -_fallRate;
        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public void PerformJump(Vector2 inputVector)
    {
        Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
    }

    public void PerformAirDash(Vector2 inputVector) 
    {
        IsDashing = true;
        _playerRigidBody.velocity = Vector3.zero;
        Vector3 dashVector = (inputVector != Vector2.zero)? new Vector3(inputVector.x, 0, inputVector.y) : Vector3.forward;
        IsDashing = true;
        _dashTargetPosition = _playerRigidBody.position + (dashVector * _dashDistance);
        RemainingAirDashes -= 1;
        _playerRigidBody.position = Vector3.Lerp(_playerRigidBody.position, _dashTargetPosition, _dashSpeed);
    }

    public void StopJumpVelocity()
    {        
        if (!IsGrounded)
        {
            _playerRigidBody.velocity = Vector3.down;
        }
    }

    public void CheckIfDashIsDone()
    {
        if((transform.parent.transform.position - _dashTargetPosition).magnitude < .5)
        {
            IsDashing = false;
        }
        else
        {
            _playerRigidBody.position = Vector3.Lerp(_playerRigidBody.position, _dashTargetPosition, .2f);//Clean up in Dash Rework
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
        RemainingAirDashes = _maxiumAirDashes;
    } 
    
    void SetupAvatarAspect()
    {
        ResetAirDashes();
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _currentTarget = GetComponentInParent<PlayerController>().CurrentTarget; //account for target switch in PlayerController
    }
}
