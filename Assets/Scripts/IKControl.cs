using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//from https://docs.unity3d.com/Manual/InverseKinematics.html

public class IKControl : MonoBehaviour
{
    public bool IsIKActive = false;
    public Transform LeftHandObject = null;
    public Transform ObjectToLookAt = null;
    public Transform EmmissionPoint = null;

    public float xRotation;
    public float yRotation;
    public float zRotation;
    public float wRotation;

    public Quaternion InitalRotation;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        InitalRotation = _animator.GetBoneTransform(HumanBodyBones.Chest).localRotation;
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
                    //_animator.SetBoneLocalRotation(HumanBodyBones.Chest, new Quaternion(xRotation, 0, 0, 1));
                   // _animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, new Quaternion(xRotation, yRotation, zRotation, 1));

                    //EmmissionPoint.LookAt(ObjectToLookAt);
                    //float pivot = EmmissionPoint.localRotation.y;
                    //print(pivot);

                    //_animator.bodyRotation = new Quaternion(0, _animator.bodyRotation.y + pivot, 0, 0);

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
