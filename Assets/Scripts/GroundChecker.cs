using UnityEngine;

public class GroundChecker : MonoBehaviour
{
   public AvatarAspect Avatar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Avatar.IsGrounded = true;
            //_avatar.ResetAirDashes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Avatar.IsGrounded = false;
        }
    }
}
