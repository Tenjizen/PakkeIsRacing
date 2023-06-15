using UnityEngine;

namespace Character.Camera.State
{
    public class CameraUnbalancedState : CameraStateBase
    {
        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("FreeLook");
        }

        public override void UpdateState(CameraManager camera)
        {
            CamManager.MakeSmoothCameraBehindBoat();
            CamManager.MakeTargetFollowRotationWithKayak();
            CamManager.SmoothResetDistanceValue();
            if (Mathf.Abs(CamManager.CharacterManager.Balance) < CamManager.CharacterManager.Data.BalanceDeathLimit)
            {
                RotateCameraInZ();
            }
            else
            {
                CamManager.SmoothResetRotateZ();
            }

            CamManager.ApplyRotationCamera();

            CamManager.ShakeCameraWarning(CamManager.Data.AmplitudeShakeWhenUnbalanced);
        }

        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void LateUpdate(CameraManager camera)
        {

        }

        public override void SwitchState(CameraManager camera)
        {

        }

        private void RotateCameraInZ()
        {
            if (CamManager.CharacterManager.Balance > 0)
            {
                CamManager.RotationZ = Mathf.Lerp(CamManager.RotationZ, CamManager.CharacterManager.Balance + 10, CamManager.Data.BalanceRotationZ * Time.deltaTime * 100);
            }
            else if (CamManager.CharacterManager.Balance < 0)
            {
                CamManager.RotationZ = Mathf.Lerp(CamManager.RotationZ, CamManager.CharacterManager.Balance - 10, CamManager.Data.BalanceRotationZ * Time.deltaTime * 100);
            }
        }
    }
}
