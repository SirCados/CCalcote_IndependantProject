using UnityEngine;

public class AimingRing : MonoBehaviour
{
    Transform _container;

    private void Awake()
    {
        _container = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
           //_container.Rotate(0, 5 * Time.deltaTime, 0);// makes aiming ring difficult, need to rework
        }
    }
}
