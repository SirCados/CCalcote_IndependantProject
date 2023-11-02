using UnityEngine;

public class BarrageAspect : MonoBehaviour
{
    public float RepeatTime = 0.1f;
    public GameObject BarrageProjectile;    
    public bool IsBarraging = false;
    public bool IsRecovering = false;

    int _counter = 1;
    [SerializeField] int _recoveryTime; //will always be larger than timesToRepeat. 
    [SerializeField] int _timesToRepeat; //number of times the barrage will fire on a given press.
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
        else if (_counter > _timesToRepeat)
        {
            IsBarraging = false;
            _counter++;
            return;
        }
        _counter++;
        BarrageProjectile projectile = Instantiate(BarrageProjectile, transform).GetComponent<BarrageProjectile>();
        projectile.Target = _currentTarget;
        projectile.TargetRigidBody = _currentTarget.GetComponent<Rigidbody>();
        transform.DetachChildren();
    }

    void SetUpBarrageAspect()
    {
        _currentTarget = GetComponentInParent<PlayerController>().CurrentTarget;
    }
}
