using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraDeathState : CameraStateBase
    {

        public override void EnterState(CameraManager camera)
        {
            CamManager.ShakeCameraWarning(0);
            CamManager.CameraAngleOverride = 0;
        }
        public override void UpdateState(CameraManager camera)
        {
            CamManager.SmoothResetRotateZ();
            if (Mathf.Abs(CamManager.RotationZ) <= 5)
            {
                Isdead();
            }
            CamManager.ApplyRotationCameraWhenCharacterDeath();
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


        private void Isdead()
        {

            CamManager.VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance += CamManager.Data.ValueAddForDistanceWhenDeath * Time.deltaTime;

            if (CamManager.VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance > CamManager.Data.MaxValueDistanceToStartDeath)
            {
                CamManager.StartDeath = true;
            }
            CamManager.CameraAngleOverride += CamManager.Data.ValueAddForTopDownWhenDeath * Time.deltaTime;
        }

    }
}
