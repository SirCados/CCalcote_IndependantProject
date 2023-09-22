using UnityEngine;

//Need to make Projectile baseclass?

public class BarrageProjectile: MonoBehaviour
{
    public Transform Target;
    public float ProjectileSpeed = 20f;

    Rigidbody _projectileRigidBody;    

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
        transform.LookAt(Target);
        _projectileRigidBody.velocity = transform.forward * ProjectileSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("impact with " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
