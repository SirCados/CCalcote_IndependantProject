using System.Collections;
using UnityEngine;

public class GracefulAvatar : AvatarAspect
{
    bool _isInputBlocked = false;
    public override void PerformAirDash(Vector2 inputVector)
    {
        if (!_isInputBlocked && RemainingAirDashes !=0)
        {
            _isInputBlocked = true;
            RemainingAirDashes--;
            _animator.SetBool("IsJumping", true);
            Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
            _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
            StartCoroutine(AirJumpPause());
        }
        else
        {
            //play error sound
        }
    }

    IEnumerator AirJumpPause()
    {
        yield return new WaitForSecondsRealtime(.5f);
        _isInputBlocked = false;
    }
}
