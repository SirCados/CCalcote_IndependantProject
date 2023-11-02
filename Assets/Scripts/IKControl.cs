using UnityEngine;

//from https://docs.unity3d.com/Manual/InverseKinematics.html

public class IKControl : MonoBehaviour
{
    public bool IsIKActive = false;
    Transform _leftHandObject = null;
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

                if(_leftHandObject != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, .5f);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandObject.position);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandObject.rotation);
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

    void SetUpIKControl()
    {
        _animator = GetComponent<Animator>();
        _leftHandObject = GetComponentInParent<PlayerController>().CurrentTarget;
        _objectToLookAt = GetComponentInParent<PlayerController>().CurrentTarget;
    }
}
