using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraCombatState : CameraStateBase
    {
        private float _currentFov;
        private float _baseFov = 55f;
        private Vector3 _targetOffset;
        private Vector3 _currentOffset;

        public CameraCombatState(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour) :
            base(cameraManagerRef, monoBehaviour)
        {
        }

        public override void EnterState(CameraManager camera)
        {
            Debug.Log("Camera Combat");

            CameraManagerRef.AnimatorRef.Play("Combat");

            _baseFov = CameraManagerRef.VirtualCameraCombat.m_Lens.FieldOfView;
            _currentFov = _baseFov;

            _targetOffset = CameraManagerRef.CombatBaseShoulderOffset + CameraManagerRef.CombatOffset;
        }
        
        public override void UpdateState(CameraManager camera)
        {
            CameraManagerRef.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            CameraManagerRef.ApplyRotationCameraInCombat();

            CameraManagerRef.CinemachineTargetPitch = ClampAngle(CameraManagerRef.CinemachineTargetPitch, CameraManagerRef.HeightClamp.x, CameraManagerRef.HeightClamp.y);

            ClampRotationCameraValue(CameraManagerRef.HeightClamp.x, CameraManagerRef.HeightClamp.y);

            const float fovLerp = 0.05f;
            _currentFov = Mathf.Lerp(_currentFov, CameraManagerRef.CombatFov, fovLerp);
            CameraManagerRef.CurrentStateBase.SetFOV(CameraManagerRef.VirtualCameraCombat, _currentFov);

            const float offsetLerp = 0.05f;
            _currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, offsetLerp);
            CameraManagerRef.CinemachineCombat3rdPersonFollow.ShoulderOffset = _currentOffset;
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
