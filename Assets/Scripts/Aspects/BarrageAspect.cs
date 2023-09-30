using UnityEngine;

public class BarrageAspect : MonoBehaviour
{
    public float RepeatTime = 0.1f;
    public GameObject BarrageProjectile;
    public GameObject CurrentTarget;
    public bool IsBarraging = false;//TODO: getter?

    int _repeat = 0;//TODO: RENAME!

    public void PerformBarrage()
    {
        IsBarraging = true;
        InvokeRepeating("SpawnBarrageProjectile", 0f, RepeatTime);        
    }

    void SpawnBarrageProjectile()
    {
        if (_repeat > 10)
        {
            IsBarraging = false;
            _repeat = 0;
            CancelInvoke();
            return;
        }
        else if (_repeat > 2)
        {
            _repeat++;
            return;
        }
        _repeat++;
        print("Spawn!");
        BarrageProjectile projectile = Instantiate(BarrageProjectile, transform).GetComponent<BarrageProjectile>();
        projectile.Target = CurrentTarget.transform;
        projectile.TargetRigidBody = CurrentTarget.GetComponent<Rigidbody>();
        transform.DetachChildren();
    }
}
