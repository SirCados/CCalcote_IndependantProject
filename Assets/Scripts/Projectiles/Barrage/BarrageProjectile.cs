using UnityEngine;


//Need to make Projectile baseclass?
//
public class BarrageProjectile: MonoBehaviour
{
    Rigidbody _projectileRigidBody;
    public float ProjectileSpeed = 20f;

    private void Awake()
    {
        _projectileRigidBody = GetComponent<Rigidbody>();
        
    }

    private void FixedUpdate()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        _projectileRigidBody.velocity = transform.forward * ProjectileSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("impact with " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
