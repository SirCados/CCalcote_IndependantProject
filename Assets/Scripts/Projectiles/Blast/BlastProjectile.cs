using UnityEngine;

public class BlastProjectile : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] float _projectileSpeed = 20;

    Rigidbody _projectileRigidBody;
    public BlastExplosion Explosion;
    public Transform Target;
    float _targetDistance;
    public bool IsArcing = true;
    float _moveTime = 0;

    bool _isFalling = false;
    Vector3[] _trajectoryPoints = new Vector3[3];


    public delegate void ExplosionEvent();
    public static event ExplosionEvent OnExplosion;


    private void Awake()
    {
        _projectileRigidBody = GetComponent<Rigidbody>();      
    }

    private void Start()
    {
        RotateProjectile();
    }

    private void FixedUpdate()
    {
        MoveProjectile();
    }

    public void SetDistanceToTarget()
    {
        if (Target != null)
        {
            _targetDistance = Vector3.Distance(transform.position, Target.position);
        }
    }

    void MoveProjectile()
    {

        PlotCourse();
        _moveTime += Time.deltaTime / _projectileSpeed;
        if (IsArcing)
        {
            _projectileRigidBody.position = Vector3.Lerp(_trajectoryPoints[0], _trajectoryPoints[1], _moveTime);
        }
        else
        {
            _projectileRigidBody.velocity = transform.forward * _projectileSpeed * 16;
        }
        
    }

    void RotateProjectile()
    {
        Vector3 colliderAdjustment = new Vector3(0, 0.25f, 0);
        Vector3 heading = (Target.position - transform.position) + colliderAdjustment;
        Quaternion rotation = Quaternion.LookRotation(heading);
        transform.rotation = rotation;
        //I want to add a z rotation, but I need to get particles to swirl like in explosion.
        //I want to account for the parabolic arc and make the projectile rotate along its X as it moves.
    }

    void PlotCourse()
    {
        Vector3 pointA = transform.position;
        Vector3 pointB = Target.position;
        Vector3 midPoint = (pointA + pointB) / 2;
        float distance = Vector3.Distance(pointA, pointB);
        midPoint.y = (IsArcing) ? distance / 3 : midPoint.y;
        _trajectoryPoints[0] = pointA;
        _trajectoryPoints[1] = midPoint;
        _trajectoryPoints[2] = pointB;
    }

    void DoExplosion()
    {
        if(OnExplosion != null)
            OnExplosion();
        Instantiate(Explosion, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        DoExplosion();
    }
}
