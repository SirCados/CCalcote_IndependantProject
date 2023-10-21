using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetDummy : MonoBehaviour
{
    public float Minumum = 2f;
    public float Maximum = 3f;
    public float Speed = 5f;
    public float StartFiring = 2f;
    public float RepeatFire = 5f;

    public BarrageAspect ManifestedBarrage;
    public AvatarAspect Target;
    public Transform FacingIndicator;

    // Use this for initialization
    void Start()
    {
        Minumum = transform.position.x;
        Maximum = transform.position.x + 10;
        FireAtInterval();
    }

    // Update is called once per frame
    void Update()
    {       
        if (Target.IsGameOver)
        {
            CancelInvoke();
        }
        else
        {
            FacingIndicator.LookAt(Target.transform);
            transform.position = new Vector3(Mathf.PingPong(Time.time * Speed, Maximum - Minumum) - Minumum, transform.position.y, transform.position.z);
        }
    }

    void FireAtInterval()
    {
        InvokeRepeating("BarrageAttack", StartFiring, RepeatFire);
    }

    void BarrageAttack()
    {
        print("Enemy Barrage!");
        ManifestedBarrage.PerformBarrage();
    }

}
