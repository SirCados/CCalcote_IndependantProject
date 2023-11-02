using UnityEngine;

//from https://docs.unity3d.com/Manual/InverseKinematics.html

public class IKControl : MonoBehaviour
{
    public bool IsIKActive = false;
    public Transform LeftHandObject = null;
    public Transform ObjectToLookAt = null;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            if (IsIKActive)
            {
                if(ObjectToLookAt != null)
                {
                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(ObjectToLookAt.position);                    
                } 

                if(LeftHandObject != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, .5f);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObject.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObject.rotation);
                }
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetLookAtWeight(0);
            }

        }
    }
}
