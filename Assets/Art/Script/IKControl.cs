using UnityEngine;

namespace Art.Script
{
    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour
    {
        private Animator _animator;

        public bool IkActive;
        public Transform PaddleRightHandObj;
        public Transform PaddleLeftHandObj;
        public GameObject PaddleGrabIK;
        public Transform HarpoonRightHandObj;
        public GameObject HarpoonGrabIK;
        public Transform NetRightHandObj;
        public Transform NetLeftHandObj;
        public GameObject NetGrabIK;
        public Transform LookObj;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null || IkActive == false)
            {
                return;
            }

            if (LookObj != null)
            {
                _animator.SetLookAtWeight(1);
                _animator.SetLookAtPosition(LookObj.position);
                return;
            }
            
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetLookAtWeight(0);
        }

        public void SetNet()
        {
            PaddleGrabIK.SetActive(false);
            HarpoonGrabIK.SetActive(false);
            NetGrabIK.SetActive(true);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, NetLeftHandObj.rotation);
        }

        public void SetHarpoon()
        {
            HarpoonGrabIK.SetActive(true);
            PaddleGrabIK.SetActive(false);
            NetGrabIK.SetActive(false);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, HarpoonRightHandObj.position);
        }

        public void SetPaddle()
        {
            PaddleGrabIK.SetActive(true);
            HarpoonGrabIK.SetActive(false);
            NetGrabIK.SetActive(false);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, PaddleRightHandObj.rotation);
        }
    }
}