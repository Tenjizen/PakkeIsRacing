using UnityEngine;

namespace Character.Camera.State
{
    public class CameraUnbalancedState : CameraStateBase
    {
        public override void EnterState(CameraManager camera)
        {
        }
        
        public override void UpdateState(CameraManager camera)
        {
            CamManager.MakeSmoothCameraBehindBoat();
            CamManager.MakeTargetFollowRotationWithKayak();
            if (Mathf.Abs(CamManager.CharacterManager.Balance) < CamManager.CharacterManager.Data.BalanceDeathLimit)
            {
                RotateCameraInZ();
            }
            else
            {
                CamManager.SmoothResetRotateZ();
            }
 
            CamManager.ApplyRotationCamera();
            
            CamManager.ShakeCamera(CamManager.Data.AmplitudeShakeWhenUnbalanced);
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
                CamManager.RotationZ = Mathf.Lerp(CamManager.RotationZ, CamManager.CharacterManager.Balance + 10, 0.01f);
            }
            else if (CamManager.CharacterManager.Balance < 0)
            {
                CamManager.RotationZ = Mathf.Lerp(CamManager.RotationZ, CamManager.CharacterManager.Balance - 10, 0.01f);
            }
        }
    }
}
