using UnityEngine;

public class BlastProjectile : MonoBehaviour
{
    Rigidbody _projectileRigidBody;
    public BlastExplosion Explosion;
    ParticleSystem _particles;
    public Transform Target;
    [SerializeField] float _projectileSpeed = 20;

    public delegate void ExplosionEvent();
    public static event ExplosionEvent OnExplosion;


    private void Awake()
    {
        _projectileRigidBody = GetComponent<Rigidbody>();
        _particles = GetComponentInChildren<ParticleSystem>();
        
    }

    private void Start()
    {
        RotateProjectile();
    }

    private void FixedUpdate()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        _projectileRigidBody.velocity = transform.forward * _projectileSpeed;   
        //I want to add a z rotation, but I need to get particles to swirl like in explosion.
    }

    private void RotateProjectile()
    {
        Vector3 colliderAdjustment = new Vector3(0, 0.25f, 0);
        Vector3 heading = (Target.position - transform.position) + colliderAdjustment;
        Quaternion rotation = Quaternion.LookRotation(heading);
        transform.rotation = rotation;
    }

    void DoExplosion()
    {
        if(OnExplosion != null)
            OnExplosion();
        Instantiate(Explosion, transform.position, new Quaternion());        
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DoExplosion();
    }
}
