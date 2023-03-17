using UnityEngine;

namespace Character.Camera.State
{
    public class CameraCombatState : CameraStateBase
    {
        public CameraCombatState(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour) :
            base(cameraManagerRef, monoBehaviour)
        {
        }

        public override void EnterState(CameraManager camera)
        {
            Debug.Log("Camera Combat");
        }
        public override void UpdateState(CameraManager camera)
        {
            CameraManagerRef.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            CameraManagerRef.ApplyRotationCamera();
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
