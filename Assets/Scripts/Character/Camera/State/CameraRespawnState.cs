using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraRespawnState : CameraStateBase
    {
        public override void EnterState(CameraManager camera)
        {
            CamManager.ShakeCamera(0);
            CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = CamManager.Data.CameraDistanceRespawn;
            CamManager.CameraAngleOverride = CamManager.Data.CameraAngleTopDownRespawn;
            ResetCameraBehindBoat();
        }
        public override void UpdateState(CameraManager camera)
        {
            CamManager.SmoothResetRotateZ();
            Respawn();
            CamManager.ApplyRotationCameraWhenCharacterDeath();
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }


        private void ResetCameraBehindBoat()
        {
            //Start
            CamManager.MakeTargetFollowRotationWithKayak();

            //Middle
            Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;
            Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

            CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), 1f);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, 1f);
            CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

            //End
            CamManager.ApplyRotationCamera();
        }


        private void Respawn()
        {
            if (CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance >= 7)
                CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance -= Time.deltaTime * CamManager.Data.MultiplyTimeForDistanceWhenRespawn /* CameraManagerRef.ValueRemoveForDistanceWhenRespawn*/;

            if (CamManager.VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance <= 7 && CamManager.CameraAngleOverride <= 0)
            {
                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CamManager.SwitchState(cameraNavigationState);
            }

            if (CamManager.CameraAngleOverride > 0)
                CamManager.CameraAngleOverride -= Time.deltaTime *  CamManager.Data.MultiplyTimeForTopDownWhenRespawn;
        }
    }
}
