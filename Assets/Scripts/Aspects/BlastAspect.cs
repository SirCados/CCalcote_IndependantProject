using UnityEngine;

public class BlastAspect : MonoBehaviour
{
    public bool IsBlasting = false;
    public bool IsGrounded = false;
    public bool IsProjectileActive = false;
    public GameObject Projectile;
    public Transform CurrentTarget;
    public Transform BlastAimingRing;

    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Transform _emitterPosition;

    [SerializeField] int _linePoints = 10;
    [SerializeField] float _timeBetweenPoints = .1f;


    AvatarAspect _avatarAspect;

    private void Awake()
    {
        SetUpBlastAspect();
        transform.DetachChildren();
    }

    private void OnEnable()
    {
        BlastProjectile.OnExplosion += HandleExplosion;
    }

    private void OnDisable()
    {
        BlastProjectile.OnExplosion -= HandleExplosion;
    }

    private void Update()
    {
        if (IsBlasting)
        {
            DrawLine();
        }        
    }

    public void BeginBlast()
    {
        IsBlasting = true;
        Vector3 initialPosition = new Vector3(CurrentTarget.position.x, 0, CurrentTarget.position.z);
        BlastAimingRing.position = initialPosition;
        BlastAimingRing.gameObject.SetActive(true);
        _lineRenderer.enabled = true;
    }

    public void EndBlast()
    {
        IsBlasting = false;
        _lineRenderer.enabled = false;
        if (!_avatarAspect.IsKnockedDown || !_avatarAspect.IsInHitStun)
        {
            //TODO: I think I set up a race condition with this. The states and all the toggles need to be cleaned up and have their responsibilities checked anyways.
            SpawnBlastProjectile();
        }
    }

    void DrawLine()
    {
        //TODO: This is good enough for now, but this will need to be a parabolic arc at some point.
        Vector3 pointA = transform.position;
        Vector3 pointB = BlastAimingRing.position;
        Vector3 midPoint = (pointA + pointB) / 2;
        float distance = Vector3.Distance(pointA, pointB);
        midPoint.y = (IsGrounded)?distance/3 : midPoint.y;
        _lineRenderer.SetPosition(0, pointA);
        _lineRenderer.SetPosition(1, midPoint);
        _lineRenderer.SetPosition(2, pointB);
    }

    public void PerformRingMove(Vector2 inputVector)
    {
        float speed = .5f;
        Vector3 targetVelocity = transform.TransformDirection(new Vector3(inputVector.x, 0, inputVector.y) * speed);
        BlastAimingRing.Translate(targetVelocity);
    }

    void SpawnBlastProjectile()
    {
        IsProjectileActive = true;
        BlastProjectile blastProjectile = Instantiate(Projectile, _emitterPosition).GetComponent<BlastProjectile>();
        blastProjectile.Target = BlastAimingRing;
        blastProjectile.SetDistanceToTarget();
        blastProjectile.IsArcing = _avatarAspect.IsGrounded;
        _emitterPosition.DetachChildren();        
    }

    void HandleExplosion()
    {
        IsProjectileActive = false;
        BlastAimingRing.gameObject.SetActive(false);
    }

    void SetUpBlastAspect()
    {
        BlastAimingRing.parent = GetComponentInParent<Transform>();        
        BlastAimingRing.gameObject.SetActive(false);
        _lineRenderer.positionCount = 3;
        _avatarAspect = GetComponentInParent<AvatarAspect>();
    }
}
