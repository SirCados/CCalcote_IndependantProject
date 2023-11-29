using System.Collections;
using UnityEngine;

public class SwiftAvatar : AvatarAspect
{    
    public override void PerformAirDash(Vector2 inputVector)
    {
        
        if (RemainingAirDashes == _maxiumAirDashes)
        {
            RemainingAirDashes--;
            _animator.SetBool("IsJumping", true);
            Vector3 airVelocity = new Vector3(inputVector.x, _jumpForce, inputVector.y);
            _playerRigidBody.AddForce(airVelocity, ForceMode.VelocityChange);
        }
        else if (RemainingAirDashes < _maxiumAirDashes)
        {
            base.PerformAirDash(inputVector);
        }
    }

    public override void SetupAvatarAspect()
    {
        base.SetupAvatarAspect();
        _aimWalk = .5f;
    }
}
