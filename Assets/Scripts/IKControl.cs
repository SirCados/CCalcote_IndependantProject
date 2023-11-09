using UnityEngine;

//from https://docs.unity3d.com/Manual/InverseKinematics.html

public class IKControl : MonoBehaviour
{
    public bool IsIKActive = false;
    public bool IsActive = true;
    public bool IsAiming = false;

    Transform _leftHandTarget = null;
    Transform _rightHandTarget = null;
    Transform _objectToLookAt = null;

    Animator _animator;

    private void Awake()
    {
        SetUpIKControl();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            if (IsIKActive)
            {
                if(_objectToLookAt != null)
                {
                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(_objectToLookAt.position);                    
                } 

                if(_leftHandTarget != null && IsActive)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, .5f);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandTarget.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandTarget.rotation);
                }

                if (_rightHandTarget != null && IsAiming)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, .5f);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandTarget.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandTarget.rotation);
                }
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetLookAtWeight(0);
            }

        }
    }

    void SetUpIKControl()
    {
        _animator = GetComponent<Animator>();
        _leftHandTarget = GetComponentInParent<PlayerController>().CurrentTarget;
        _rightHandTarget = _leftHandTarget;
        _objectToLookAt = GetComponentInParent<PlayerController>().CurrentTarget;
    }
}
