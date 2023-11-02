using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    AvatarAspect _avatar;

    private void Awake()
    {
        SetUpGroundChecker();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _avatar.IsGrounded = true;
            _avatar.ResetAirDashes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _avatar.IsGrounded = false;
        }
    }

    void SetUpGroundChecker()
    {
        _avatar = GetComponentInParent<AvatarAspect>();
    }
}
