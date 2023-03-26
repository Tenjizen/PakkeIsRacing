using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public abstract class CameraStateBase
    {
        public enum CameraMode
        {
            Navigation = 0,
            Combat = 1,
        }

        public CameraStateBase(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour)
        {
            CameraManagerRef = cameraManagerRef;
            MonoBehaviourRef = monoBehaviour;
        }

        protected CameraManager CameraManagerRef;
        protected MonoBehaviour MonoBehaviourRef;

        protected bool CanResetShoulderOffset = true;

        public abstract void EnterState(CameraManager camera);

        public abstract void UpdateState(CameraManager camera);

        public abstract void FixedUpdate(CameraManager camera);

        public abstract void SwitchState(CameraManager camera);

        protected void ClampRotationCameraValue(float min, float max)
        {
            CameraManagerRef.CinemachineTargetYaw = ClampAngle(CameraManagerRef.CinemachineTargetYaw, float.MinValue, float.MaxValue);
            CameraManagerRef.CinemachineTargetPitch = ClampAngle(CameraManagerRef.CinemachineTargetPitch, min, max);
        }
        public float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public void ManageFreeCameraMove(ref float timerCameraReturnBehindBoat, CameraMode cameraMode)
        {
            //rotate freely with inputs
            if (CameraManagerRef.CanMoveCameraManually)
            {
                Vector2 aim = Vector2.zero;
                switch (cameraMode)
                {
                    case CameraMode.Navigation:
                        aim = CameraManagerRef.Input.Inputs.RotateCamera;
                        CameraManagerRef.CinemachineTargetPitch += CameraManagerRef.JoystickFreeRotationY.Evaluate(aim.y);
                        break;

                    case CameraMode.Combat:
                        aim = CameraManagerRef.Input.Inputs.MovingAim;
                        aim = new Vector2(aim.x, aim.y * -1);
                        CameraManagerRef.CinemachineTargetPitch += CameraManagerRef.JoystickFreeRotationY.Evaluate(aim.y);
                        break;
                }

                //Controller
                CameraManagerRef.CinemachineTargetYaw += CameraManagerRef.JoystickFreeRotationX.Evaluate(aim.x);
                //CameraManagerRef.CinemachineTargetPitch = CameraManagerRef.JoystickFreeRotationY.Evaluate(aim.y); (jsp pk ici ça fait de la merde)

                #region clavier souris
                //KBM
                //CameraManagerRef.CinemachineTargetYaw += CameraManagerRef.Input.Inputs.RotateCamera.x;
                //CameraManagerRef.CinemachineTargetPitch += CameraManagerRef.Input.Inputs.RotateCamera.y;
                #endregion

                //last inputs
                CameraManagerRef.LastInputX = aim.x != 0 ? aim.x : CameraManagerRef.LastInputX;
                CameraManagerRef.LastInputY = aim.y != 0 ? aim.y : CameraManagerRef.LastInputY;

                timerCameraReturnBehindBoat = 0;
            }
        }

        private float _uselessFloatForRef;
        public void ManageFreeCameraMove(CameraMode cameraMode)
        {
            ManageFreeCameraMove(ref _uselessFloatForRef, cameraMode);
        }

        public void SetFOV(CinemachineVirtualCamera virtualCamera ,float value)
        {
            virtualCamera.m_Lens.FieldOfView = value;
        }

        public void ResetShoulderOffset()
        {
            if (CanResetShoulderOffset == false)
            {
                return;
            }

            Vector3 currentOffset = Vector3.Lerp(CameraManagerRef.CinemachineCombat3rdPersonFollow.ShoulderOffset, CameraManagerRef.CombatBaseShoulderOffset, 0.01f);
            CameraManagerRef.CinemachineCombat3rdPersonFollow.ShoulderOffset = currentOffset;
        }
    }
}


