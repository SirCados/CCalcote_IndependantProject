using UnityEngine;
using System.Collections;

public class AvatarAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public bool IsDashing = false;
    public bool IsGameOver = false;
    public bool IsGettingUp = false;
    public bool IsGrounded = true;
    public bool IsKnockedDown = false;
    public bool IsInHitStun = false;
    public bool IsInvulnerable = false;
    public bool IsSturdy = false;

    [Header("PUBLIC AVATAR STATS")]
    public int CurrentHealth;
    public int CurrentStability;
    public int RemainingAirDashes;

    [Header("AVATAR STATS")]
    [Tooltip("How much damage the avatar can take before game over.")]
    [SerializeField]protected int _maximumHealth = 3;
    [Tooltip("Reduced damage taken and stabilty loss from hits. Cannot reduce them to 0")]
    [SerializeField][Range(0, 20)] protected int _defense;
    [Tooltip("If avatar reaches 0 Stabilty, it is knocked down.")]
    [SerializeField][Range(30, 60)] protected int _maximumStability;
    [Tooltip("How fast an avatar can move")]
    [SerializeField] protected float _movementSpeed;
    [Tooltip("How fast an avatar can get to top speed.")]
    [SerializeField] protected float _accelerationRate;
    [Tooltip("How much force an avatar recieves when jumping.")]
    [SerializeField]protected float _jumpForce;
    [Tooltip("How well an avatar can move in the air while falling.")]
    [SerializeField][Range(0, 1)] protected float _airWalk;
    [Tooltip("How fast an avatar will fall, will affect jump height.")]
    [SerializeField] protected float _fallRate;
    [Tooltip("Number of times an avatar can air dash before touching the ground.")]
    [SerializeField] protected int _maxiumAirDashes;
    [Tooltip("Distance of an air dash.")]
    [SerializeField] protected float _dashDistance;
    [Tooltip("Speed of an air dash.")]
    [SerializeField] protected float _dashSpeed;

    [SerializeField] protected Transform _facingIndicator;
    [SerializeField] protected Transform _avatarModelTransform;

    [Range(0, 1)] protected float _aimWalk = .2f;

    protected Animator _animator;
    protected IKControl _ikControl;
    protected Rigidbody _playerRigidBody;
    protected Transform _currentTarget;
    protected Vector3 _dashStartPosition;
    protected Vector3 _dashVector;

    private void Awake()
    {
        SetupAvatarAspect();        
    }

    private void FixedUpdate()
    {
        HandleJumpAndFallingAnimations();
        RotateCharacter();
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
        float speed = (IsGrounded) ? _movementSpeed : _movementSpeed * _airWalk;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);        
        Vector3 velocityChange = (targetVelocity - _playerRigidBody.velocity) * _accelerationRate;
        velocityChange = (IsBlasting) ? velocityChange * _aimWalk : velocityChange;
        velocityChange.y = (IsGrounded)? 0:-_fallRate;        
        _playerRigidBody.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public void PerformJump(Vector2 inputVector)
    {
        _animator.SetBool("IsJumping", true);
        Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
        _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);        
    }

    public virtual void PerformAirDash(Vector2 inputVector) 
    {
        IsDashing = true;
        RemainingAirDashes--;
        _playerRigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        _playerRigidBody.useGravity = false;
        _dashVector = ((inputVector != Vector2.zero) ? new Vector3(inputVector.x, 0, inputVector.y) : _avatarModelTransform.forward) * _dashDistance;                       
        
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
        else if (!IsBlasting)
        {
            _ikControl.IsActive = true;
            _ikControl.IsBlasting = false;
        }
    }

    public void TakeHit(int incomingDamage, int incomingStabilityLoss)
    {
        print("ouch");
        //Will cause damage loss if multiple hits happen too close together.
        //need to confirm or find a better way to handle everything
        StopCoroutine(HandleHit(incomingDamage, incomingStabilityLoss));
        StartCoroutine(HandleHit(incomingDamage, incomingStabilityLoss));
    }

    IEnumerator HandleHit(int incomingDamage, int incomingStabilityLoss)
    {
        if (!IsInvulnerable)
        {
            StopCoroutine(RegainStability());
            int damageToTake = incomingDamage - _defense;
            CurrentHealth -= (damageToTake > 1) ? damageToTake : 1;
            IsInHitStun = true;
            _animator.SetBool("IsInHitStun", true);
            if (!IsSturdy)
            {
                int stabilityToLose = (incomingStabilityLoss - Mathf.CeilToInt(_defense / 2));
                CurrentStability -= (stabilityToLose > 1) ? stabilityToLose : 1;
                if (CurrentStability <= 0)
                {
                    CurrentStability = 0;
                    IsKnockedDown = true;
                    _animator.SetBool("IsKnockedDown", true);
                }
            }
            IsDashing = false;
            yield return new WaitForSeconds(.1f);
            IsInHitStun = false;
            _animator.SetBool("IsInHitStun", false);
            StartCoroutine(RegainStability());
        }        
    }

    public virtual IEnumerator RegainStability()
    {
        if (IsKnockedDown)
        {
            yield return new WaitForSeconds(3);
            GetUpSequence();
        }
        yield return new WaitForSeconds(3);
        while (CurrentStability < _maximumStability && !IsKnockedDown)
        {
            CurrentStability += 1;
            yield return new WaitForSeconds(1f);
        }
    }

    protected void GetUpSequence()
    {
        IsKnockedDown = false;
        _animator.SetBool("IsKnockedDown", false);
        IsGettingUp = true;
        _animator.SetBool("IsGettingUp", true);
        StartCoroutine(Invulerability());
        StartCoroutine(GetUpAnimation());
        CurrentStability = _maximumStability;
    }

    IEnumerator Invulerability()
    {
        IsInvulnerable = true;
        IsSturdy = true;
        yield return new WaitForSeconds(5);
        IsInvulnerable = false;
        IsSturdy = false;
        yield break;
    }

    IEnumerator GetUpAnimation()
    {
        yield return new WaitWhile(() => _animator.GetBool("IsGettingUp"));
        IsGettingUp = false;
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
        if (_currentTarget && !(IsKnockedDown || IsGettingUp))
        {
            _facingIndicator.transform.LookAt(_currentTarget);
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

    public virtual void SetupAvatarAspect()
    {
        CurrentHealth = _maximumHealth;
        CurrentStability = _maximumStability;
        ResetAirDashes();
        _animator = GetComponentInChildren<Animator>();
        _ikControl = GetComponentInChildren<IKControl>();
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _currentTarget = GetComponentInParent<IController>().Target;
    }
}
