using UnityEngine;

public class AimingRing : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.Rotate(0, 5 * Time.deltaTime, 0);
        }
    }
}
