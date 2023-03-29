using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraDeathState : CameraStateBase
    {

        public override void EnterState(CameraManager camera)
        {
            Debug.Log("cam death");
            CamManager.ShakeCamera(0);
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
        public override void SwitchState(CameraManager camera)
        {

        }


        private void Isdead()
        {

            CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance += CamManager.Data.ValueAddForDistanceWhenDeath;

            if (CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance > CamManager.Data.MaxValueDistanceToStartDeath)
            {
                CamManager.StartDeath = true;
            }
            CamManager.CameraAngleOverride += CamManager.Data.ValueAddForTopDownWhenDeath;
        }

    }
}
