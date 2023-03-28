namespace Character.Camera.State
{
    public class CameraDialogState : CameraStateBase
    {
        public override void EnterState(CameraManager camera)
        {
            //look-at
            CamManager.VirtualCameraCombat.LookAt = null;
        }
        public override void UpdateState(CameraManager camera)
        {

        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
