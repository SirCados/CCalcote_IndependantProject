using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetDummy : MonoBehaviour
{
    public float min = 2f;
    public float max = 3f;

    public float Speed = 5f;
    // Use this for initialization
    void Start()
    {
        min = transform.position.x;
        max = transform.position.x + 10;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * Speed, max - min) - min, transform.position.y, transform.position.z);
    }

}
