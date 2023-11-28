using UnityEngine;

public class BarrageAspect : MonoBehaviour
{
    public float RepeatTime = 0.1f;
    public GameObject Projectile;    
    public bool IsBarraging = false;
    public bool IsRecovering = false;

    [SerializeField] Transform _emitterPosition;

    int _counter = 1;
    [SerializeField] int _recoveryTime; //will always be larger than timesToRepeat. 
    [SerializeField] int _amountOfProjectiles; //number of times the barrage will fire on a given press.
    Transform _currentTarget;

    private void Awake()
    {
        SetUpBarrageAspect();
    }

    public void PerformBarrage()
    {
        IsBarraging = true;
        IsRecovering = true;
        InvokeRepeating("SpawnBarrageProjectile", 0f, RepeatTime);        
    }

    void SpawnBarrageProjectile()
    {
        if (_counter > _recoveryTime)
        {
            IsRecovering = false;
            _counter = 1;
            CancelInvoke();
            return;
        }
        else if (_counter > _amountOfProjectiles)
        {
            IsBarraging = false;
            _counter++;
            return;
        }
        _counter++;
        BarrageProjectile barrageProjectile = Instantiate(Projectile, _emitterPosition).GetComponent<BarrageProjectile>();
        barrageProjectile.Target = _currentTarget.GetComponentInChildren<Rigidbody>().transform;
        barrageProjectile.TargetRigidBody = _currentTarget.GetComponentInChildren<Rigidbody>();
        _emitterPosition.DetachChildren();
        //transform.DetachChildren();
    }

    void SetUpBarrageAspect()
    {
        _currentTarget = GetComponentInParent<IController>().Target;
    }
}
