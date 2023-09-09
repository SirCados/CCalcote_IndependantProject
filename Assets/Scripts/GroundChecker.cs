using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    PlayerController _player;

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _player.IsGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _player.IsGrounded = false;
        }
    }
}
