using UnityEngine;

public class MovingTargetDummy : MonoBehaviour, IController
{
    public float Minumum = 2f;
    public float Maximum = 3f;
    public float Speed = 5f;
    public float StartFiring = 2f;
    public float RepeatFire = 5f;

    public BarrageAspect ManifestedBarrage;    
    public PlayerController PlayerController;
    public Transform FacingIndicator;

    public Transform CurrentTarget;

    AvatarAspect _target;

    // Use this for initialization
    void Start()
    {
        Minumum = transform.position.x;
        Maximum = transform.position.x + 10;
        FireAtInterval();
        _target = PlayerController.GetComponentInChildren<AvatarAspect>();
    }

    // Update is called once per frame
    void Update()
    {       
        if (_target.IsGameOver)
        {
            CancelInvoke();
        }
        else
        {
            FacingIndicator.LookAt(_target.transform);
            transform.position = new Vector3(Mathf.PingPong(Time.time * Speed, Maximum - Minumum) - Minumum, transform.position.y, transform.position.z);
        }
    }

    void FireAtInterval()
    {
        InvokeRepeating("BarrageAttack", StartFiring, RepeatFire);
    }

    void BarrageAttack()
    {
        ManifestedBarrage.PerformBarrage();
    }

    public Transform Target
    {
        get => CurrentTarget;
        set => CurrentTarget = value;
    }

}
