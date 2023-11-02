using UnityEngine;

public class AvatarAspect : MonoBehaviour
{
    public bool IsGameOver = false;
    public bool IsGrounded = true;
    public bool IsDashing = false;
    public int RemainingAirDashes;

    public float RotationIntensity;

    [SerializeField] int _maximumHealth = 3;
    int _currentHealth;

    [SerializeField] float _airDashSpeedLimit;
    [SerializeField] float _accelerationRate;
    [SerializeField] float _dashDistance;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;
    [SerializeField] int _maxiumAirDashes;    
    [SerializeField] GameObject _facingIndicator;
    [SerializeField] Transform _avatarModelTransform;
    
    Animator _animator;
    GameObject _currentTarget;
    Rigidbody _playerRigidBody;    
    public Vector2 InputVector;
    Vector3 _dashStartPosition;
    Vector3 _dashVector;

    public GameObject DashPoint;

    private void Awake()
    {
        SetupAvatarAspect();        
    }

    private void Update()
    {
        HandleJumpAndFallingAnimations();
        RotateCharacter();        
        Debug.DrawLine(_currentTarget.transform.position + Vector3.up, (_currentTarget.transform.forward * 5) + Vector3.up, Color.red, 2f);
    }

    private void FixedUpdate()
    {        
        CheckIfDashIsDone();
    }

    public void PerformMove(Vector2 inputVector)
    {
        Vector3 vectorToRotate = new Vector3(inputVector.x, 0, inputVector.y);
        Vector3 forwardProduct = vectorToRotate.z * -_avatarModelTransform.forward;
        Vector3 rightProduct = vectorToRotate.x * _avatarModelTransform.right;
        Vector3 rotatedVector = forwardProduct + rightProduct;

        if (IsGrounded && !_animator.GetBool("IsJumping"))
        {
            _animator.SetFloat("xInput", rotatedVector.x);
            _animator.SetFloat("yInput", rotatedVector.z);
            float movement = Mathf.Abs(inputVector.magnitude);
            _animator.SetFloat("Movement", movement);
        }
        InputVector = inputVector;       
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 3;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);        
        Vector3 velocityChange = (targetVelocity - _playerRigidBody.velocity) * _accelerationRate;
        velocityChange.y = (IsGrounded) ? 0 : -_fallRate;        
        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public void PerformJump(Vector2 inputVector)
    {
        _animator.SetBool("IsJumping", true);
        Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
        
    }

    public void PerformAirDash(Vector2 inputVector) 
    {
        IsDashing = true;
        _playerRigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        _playerRigidBody.useGravity = false;
        _dashVector = ((inputVector != Vector2.zero) ? new Vector3(inputVector.x, 0, inputVector.y) : _avatarModelTransform.forward) * _dashDistance;                       
        RemainingAirDashes -= 1;
        _dashStartPosition = _playerRigidBody.position;
        _playerRigidBody.AddForce(_dashVector * _dashSpeed, ForceMode.VelocityChange);
    }

    public void TakeDamage(int incomingDamage)
    {
        _currentHealth -= incomingDamage;
        if(_currentHealth >= 0)
        {
            _currentHealth = 0;
            IsGameOver = true;
            print("GAME OVER!!! RESTART!");
        }
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
        if (IsDashing)
        {
            float distance = Vector3.Distance(_playerRigidBody.position, _dashStartPosition);

            if (distance > _dashDistance)
            {
                ResetAfterDash();
            }
        }            
    }

    void ResetAfterDash()
    {
        _playerRigidBody.AddForce(_playerRigidBody.velocity * -1, ForceMode.VelocityChange);
        _playerRigidBody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        _playerRigidBody.useGravity = true;
        _playerRigidBody.Sleep();

        IsDashing = false;
    }

    void RotateCharacter()
    {
        if (_currentTarget)
        {
            _facingIndicator.transform.LookAt(_currentTarget.transform);
            Vector3 look = new Vector3(_currentTarget.transform.position.x, _playerRigidBody.position.y -1, _currentTarget.transform.position.z);
            _avatarModelTransform.LookAt(look);
        }
    }

    public void ResetAirDashes()
    {
        RemainingAirDashes = _maxiumAirDashes;
    }

    void HandleJumpAndFallingAnimations()
    {
        if (!IsGrounded)
        {
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", true);
        }
        else if (IsGrounded && _animator.GetBool("IsFalling"))
        {
            _animator.SetBool("IsFalling", false);
        }
    }

    void SetupAvatarAspect()
    {
        _currentHealth = _maximumHealth;
        ResetAirDashes();
        _animator = GetComponentInChildren<Animator>();
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _currentTarget = GetComponentInParent<PlayerController>().CurrentTarget; //account for target switch in PlayerController
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsGrounded && IsDashing && (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Wall")))
        {
            print("bump");
            ResetAfterDash();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsGrounded && IsDashing && (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Wall")))
        {
            print("bump");
            ResetAfterDash();
        }
    }
}
