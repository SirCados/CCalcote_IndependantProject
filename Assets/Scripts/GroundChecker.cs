using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    PlayerController _avatar;

    private void Awake()
    {
        _avatar = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _avatar.IsGrounded = true;
            //_avatar.ResetAirDashes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _avatar.IsGrounded = false;
        }
    }
}
