using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;

    public bool ikActive = false;
    public Transform PagaieRightHandObj = null;
    public Transform PagaieLeftHandObj = null;
    public GameObject PagaieGrabIK;
    public Transform HarpoonRightHandObj = null;
    public GameObject HarpoonGrabIK;
    public Transform FiletRightHandObj = null;
    public Transform FiletLeftHandObj = null;
    public GameObject FiletGrabIK;
    public Transform lookObj = null;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (PagaieRightHandObj != null)
                {
                    PagaieGrabIK.SetActive(true);
                    HarpoonGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, PagaieRightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, PagaieRightHandObj.rotation);
                }

                if (PagaieLeftHandObj != null)
                {
                    PagaieGrabIK.SetActive(true);
                    HarpoonGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, PagaieLeftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, PagaieLeftHandObj.rotation);
                }

                else if (HarpoonRightHandObj != null)
                {
                    HarpoonGrabIK.SetActive(true);
                    PagaieGrabIK.SetActive(false);
                    PagaieGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(false);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, HarpoonRightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, HarpoonRightHandObj.rotation);
                }

                else if (FiletRightHandObj != null)
                {
                    PagaieGrabIK.SetActive(false);
                    PagaieGrabIK.SetActive(false);
                    HarpoonGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(true);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, FiletRightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, FiletRightHandObj.rotation);
                }

                else if (FiletLeftHandObj != null)
                {
                    PagaieGrabIK.SetActive(false);
                    PagaieGrabIK.SetActive(false);
                    HarpoonGrabIK.SetActive(false);
                    FiletGrabIK.SetActive(true);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, FiletLeftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, FiletLeftHandObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}

