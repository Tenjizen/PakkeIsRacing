using UnityEngine;

namespace Art.Script
{
    public enum IKType
    {
        Paddle = 0,
        Harpoon = 1,
        Net = 2
    }

    [RequireComponent(typeof(Animator))]
    public class IKControl : MonoBehaviour
    {
        private Animator _animator;

        [ReadOnly] public bool IkActive = true;
        public Transform PaddleRightHandObj;
        public Transform PaddleLeftHandObj;
        public GameObject PaddleGrabIK;
        public Transform HarpoonRightHandObj;
        public GameObject HarpoonGrabIK;
        public Transform NetRightHandObj;
        public Transform NetLeftHandObj;
        public GameObject NetGrabIK;
        public Transform LookObj;
        public IKType Type;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            SetPaddle();
            IkActive = true;
        }

        public void OnAnimatorIK()
        {
            if (_animator == null || IkActive == false)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                _animator.SetLookAtWeight(0);
                return;
            }

            if (LookObj != null)
            {
                _animator.SetLookAtWeight(1);
                _animator.SetLookAtPosition(LookObj.position);
            }

            switch (Type)
            {
                case IKType.Paddle:
                    PaddleGrabIK.SetActive(true);
                    HarpoonGrabIK.SetActive(false);
                    NetGrabIK.SetActive(false);

                    _animator.SetIKRotation(AvatarIKGoal.RightHand, PaddleRightHandObj.rotation);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, PaddleRightHandObj.position);
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, PaddleLeftHandObj.rotation);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, PaddleLeftHandObj.position);
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    break;
                case IKType.Harpoon:
                    HarpoonGrabIK.SetActive(true);
                    PaddleGrabIK.SetActive(false);
                    NetGrabIK.SetActive(false);

                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, HarpoonRightHandObj.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, HarpoonRightHandObj.rotation);

                    break;
                case IKType.Net:
                    PaddleGrabIK.SetActive(false);
                    HarpoonGrabIK.SetActive(false);
                    NetGrabIK.SetActive(true);

                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, NetLeftHandObj.rotation);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, NetLeftHandObj.position);

                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, NetRightHandObj.rotation);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, NetRightHandObj.position);
                    break;
            }
        }

        public void SetNet()
        {
            Type = IKType.Net;    
        }

        public void SetHarpoon()
        {
            Type = IKType.Harpoon;
        }

        public void SetPaddle()
        {
            Type = IKType.Paddle;
        }
    }
}