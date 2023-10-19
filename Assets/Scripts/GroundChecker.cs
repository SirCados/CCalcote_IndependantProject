using UnityEngine;

public class GroundChecker : MonoBehaviour
{
   public AvatarAspect Avatar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            print("grounded");
            Avatar.IsGrounded = true;
            Avatar.ResetAirDashes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            print("airborne");
            Avatar.IsGrounded = false;
        }
    }
}
