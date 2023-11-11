using UnityEngine;

public class AvatarAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public bool IsDashing = false;
    public bool IsGameOver = false;
    public bool IsGrounded = true;
    public int RemainingAirDashes;

    [SerializeField] int _maximumHealth = 3;
    int _currentHealth;

    [SerializeField] float _accelerationRate;
    [SerializeField] float _dashDistance;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _fallRate;
    [SerializeField] int _maxiumAirDashes;    
    [SerializeField] Transform _barrageEmitter;
    [SerializeField] Transform _avatarModelTransform;
    
    Animator _animator;
    IKControl _ikControl;
    Rigidbody _playerRigidBody;
    Transform _currentTarget;
    Vector3 _dashStartPosition;
    Vector3 _dashVector;

    private void Awake()
    {
        SetupAvatarAspect();        
    }

    private void Update()
    {
        HandleJumpAndFallingAnimations();
        RotateCharacter();
    }

    private void FixedUpdate()
    {        
        CheckIfDashIsDone();
        PerformHandRaise();
    }

    public void PerformMove(Vector2 inputVector)
    {
        //calculate the inputs for animation controller
        Vector3 vectorToRotate = new Vector3(inputVector.x, 0, inputVector.y);
        Vector3 forwardProduct = vectorToRotate.z * -_avatarModelTransform.forward;
        Vector3 rightProduct = vectorToRotate.x * _avatarModelTransform.right;
        Vector3 rotatedVector = forwardProduct + rightProduct;

        if (IsGrounded && !_animator.GetBool("IsJumping"))
        {
            _animator.SetFloat("xInput", rotatedVector.x);
            _animator.SetFloat("yInput", rotatedVector.z);
            float movement = Mathf.Abs(inputVector.magnitude);//.sqrMagnitude more performant than .magnitude
            _animator.SetFloat("Movement", movement);
        }
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed / 3;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);        
        Vector3 velocityChange = (targetVelocity - _playerRigidBody.velocity) * _accelerationRate;
        velocityChange = (IsBlasting) ? velocityChange / 5 : velocityChange;
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

    public void PerformHandRaise()
    {
        if (IsBlasting)
        {
            _ikControl.IsBlasting = true;
            _ikControl.IsActive = false;
        }
        else if (!_ikControl.IsActive && !IsBlasting)
        {
            _ikControl.IsActive = true;
            _ikControl.IsBlasting = false;
        }
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
            _playerRigidBody.velocity = new Vector3(_playerRigidBody.velocity.x, 0, _playerRigidBody.velocity.z); 
        }
    }

    public void SlowMoveVelocity()
    {
        if (IsGrounded)
        {
            _playerRigidBody.velocity = _playerRigidBody.velocity / 2;
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

    void RotateCharacter()
    {
        if (_currentTarget)
        {
            _barrageEmitter.transform.LookAt(_currentTarget);
            Vector3 look = new Vector3(_currentTarget.position.x, _playerRigidBody.position.y -1, _currentTarget.position.z);
            _avatarModelTransform.LookAt(look);
        }
    }

    void ResetAfterDash()
    {
        //Add a force that is the equal opposite of the force that dashed the player.
        _playerRigidBody.AddForce(_playerRigidBody.velocity * -1, ForceMode.VelocityChange);
        //Ensure the original rigidbody constraints are in place
        _playerRigidBody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        _playerRigidBody.useGravity = true;
        //sleep the rigidbody for at least a frame just in case something happens. Not sure if this is necessary
        _playerRigidBody.Sleep();

        IsDashing = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        //This conditional is gross. Can I clean this up in the future? 
        if (!IsGrounded && IsDashing && (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Avatar")))
        {
            ResetAfterDash();            
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //This conditional is gross. Can I clean this up in the future? 
        if (!IsGrounded && IsDashing && (collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Avatar")))
        {
            ResetAfterDash();
        }
    }

    void SetupAvatarAspect()
    {
        _currentHealth = _maximumHealth;
        ResetAirDashes();
        _animator = GetComponentInChildren<Animator>();
        _ikControl = GetComponentInChildren<IKControl>();
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _currentTarget = GetComponentInParent<PlayerController>().CurrentTarget;
    }
}
