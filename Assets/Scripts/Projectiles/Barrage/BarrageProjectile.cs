using UnityEngine;

//Using Homing Missile code from video:https://www.youtube.com/watch?v=Z6qBeuN-H1M
//Need to make Projectile baseclass?

public class BarrageProjectile: MonoBehaviour
{
    public Transform Target;
    public Rigidbody TargetRigidBody;

    [SerializeField] int _damage;
    [SerializeField] int _stabilityDamage;

    [Header("MOVEMENT")]
    [SerializeField] float _projectileSpeed = 20;
    [SerializeField] float _projectileAgility = 25;

    [Header("PREDICTION")]
    [SerializeField] float _maxDistancePrediction = 100;
    [SerializeField] float _minDistancePrediction = 5;
    [SerializeField] float _maxTimePrediction = 5;
    Rigidbody _projectileRigidBody;
    Vector3 _standardPrediction;

    [SerializeField] ParticleSystem _hitParticles;

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
        _projectileRigidBody.velocity = transform.forward * _projectileSpeed;
        float leadTimePercentage = Mathf.InverseLerp(_minDistancePrediction, _maxDistancePrediction, Vector3.Distance(transform.position, Target.position));
        PredictTargetMovement(leadTimePercentage);
        RotateProjectile();
    }

    private void PredictTargetMovement(float leadTimePercentage)
    {
        float predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = Target.position + TargetRigidBody.velocity * predictionTime;
    }

    private void RotateProjectile()
    {
        Vector3 heading = _standardPrediction - transform.position;
        Quaternion rotation = Quaternion.LookRotation(heading);
        _projectileRigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _projectileAgility * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Avatar")
        {
            AvatarAspect target = other.gameObject.GetComponentInParent<AvatarAspect>();
            GiveDamage(target);           
        }

        if(other.transform.tag != "Projectile")
        {
            Instantiate(_hitParticles, transform.position, new Quaternion());
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }

    void GiveDamage(AvatarAspect avatar)
    {
        print("take this!");
        avatar.TakeHit(_damage, _stabilityDamage);
    }
}
