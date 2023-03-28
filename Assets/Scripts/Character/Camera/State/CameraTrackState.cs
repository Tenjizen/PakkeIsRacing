using UnityEngine;

namespace Character.Camera.State
{
    public class CameraTrackState : CameraStateBase
    {
        private string _cameraName;
    
        public CameraTrackState(string cameraName) : base()
        {
            _cameraName = cameraName;
        }

        public override void EnterState(CameraManager camera)
        {
            Debug.Log("track");
            CamManager.AnimatorRef.Play(_cameraName);
            CamManager.Brain.m_BlendUpdateMethod = Cinemachine.CinemachineBrain.BrainUpdateMethod.FixedUpdate;

            //look-at
            CamManager.VirtualCameraCombat.LookAt = null;
        }
        public override void UpdateState(CameraManager camera)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CamManager.SwitchState(cameraNavigationState);
            }
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
