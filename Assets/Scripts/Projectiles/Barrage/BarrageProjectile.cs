using UnityEngine;


//Need to make Projectile baseclass?
//
public class BarrageProjectile: MonoBehaviour
{
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
        _projectileRigidBody.AddForce(Vector3.forward, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("impact");
        Destroy(gameObject);
    }
}
