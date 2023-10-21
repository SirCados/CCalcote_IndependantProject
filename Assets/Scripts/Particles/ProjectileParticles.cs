using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticles : MonoBehaviour
{
    [SerializeField] float _particleDuration = .15f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", _particleDuration);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
    
}
