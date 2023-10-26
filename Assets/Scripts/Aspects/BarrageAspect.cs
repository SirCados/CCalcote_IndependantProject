using UnityEngine;

public class BarrageAspect : MonoBehaviour
{
    public float RepeatTime = 0.1f;
    public GameObject BarrageProjectile;
    public GameObject CurrentTarget;
    public bool IsBarraging = false;//TODO: getter?
    public bool IsRecovering = false;
    public Transform EmissionPoint;

    int _counter = 1;
    [SerializeField] int _recovery; //will always be larger than timesToRepeat. 
    [SerializeField] int _timesToRepeat; //number of times the barrage will fire on a given press.

    public void PerformBarrage()
    {
        IsBarraging = true;
        IsRecovering = true;
        InvokeRepeating("SpawnBarrageProjectile", 0f, RepeatTime);        
    }

    void SpawnBarrageProjectile()
    {
        if (_counter > _recovery)
        {
            IsRecovering = false;
            _counter = 1;
            CancelInvoke();
            return;
        }
        else if (_counter > _timesToRepeat)
        {
            IsBarraging = false;
            _counter++;
            return;
        }
        _counter++;
        BarrageProjectile projectile = Instantiate(BarrageProjectile, transform).GetComponent<BarrageProjectile>();
        projectile.Target = CurrentTarget.transform;
        projectile.TargetRigidBody = CurrentTarget.GetComponent<Rigidbody>();
        transform.DetachChildren();
    }
}
