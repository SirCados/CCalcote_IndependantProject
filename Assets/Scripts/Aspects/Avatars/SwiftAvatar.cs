using System.Collections;
using UnityEngine;

public class SwiftAvatar : AvatarAspect
{    
    public override void PerformAirDash(Vector2 inputVector)
    {
        if (RemainingAirDashes == 2)
        {
            RemainingAirDashes--;
            _animator.SetBool("IsJumping", true);
            Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
            _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
        }
        else if(RemainingAirDashes == 1)
        {
            base.PerformAirDash(inputVector);
        }
    }

    public override void SetupAvatarAspect()
    {
        base.SetupAvatarAspect();
        _aimWalk = 2;
    }
}
