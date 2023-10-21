using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTargetDummy : MonoBehaviour
{
    public float min = 2f;
    public float max = 3f;
    public float Speed = 5f;
    public GameObject[] EnemyAspects = new GameObject[4];

   public GameObject _manifestedAspect;

    // Use this for initialization
    void Start()
    {
        min = transform.position.x;
        max = transform.position.x + 10;
        //ChooseRandomManifestation();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * Speed, max - min) - min, transform.position.y, transform.position.z);
    }

    void ChooseRandomManifestation()
    {
        //In the future this will be moved into the enemy AI class
        //The enemy will have random attacks and characteristics based on the object that will be loaded
        //Currently it is just a visual representation
        MeshFilter filter = _manifestedAspect.GetComponent<MeshFilter>();
        int randomIndex = Random.Range(0, EnemyAspects.Length);
        filter.sharedMesh = EnemyAspects[randomIndex].GetComponent<MeshFilter>().sharedMesh;
    }

}
