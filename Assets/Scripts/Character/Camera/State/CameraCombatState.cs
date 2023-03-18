using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraCombatState : CameraStateBase
    {
        private float _currentFov;
        private float _baseFov = 55f;
        private Vector3 _currentOffset;
        private Vector3 _targetOffset;

        public CameraCombatState(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour) :
            base(cameraManagerRef, monoBehaviour)
        {
        }

        public override void EnterState(CameraManager camera)
        {
            Debug.Log("Camera Combat");
            
            _baseFov = CameraManagerRef.VirtualCamera.m_Lens.FieldOfView;
            _currentFov = _baseFov;

            CanResetShoulderOffset = false;
            _currentOffset = CameraManagerRef.Cinemachine3rdPersonFollow.ShoulderOffset;
            _targetOffset = _currentOffset + CameraManagerRef.combatOffset;
        }
        public override void UpdateState(CameraManager camera)
        {
            CameraManagerRef.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            CameraManagerRef.ApplyRotationCamera();
            
            const float fovLerp = 0.1f;
            _currentFov = Mathf.Lerp(_currentFov, CameraManagerRef.combatFov, fovLerp);
            CameraManagerRef.CurrentStateBase.SetFOV(_currentFov);

            const float offsetLerp = 0.1f;
            _currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, offsetLerp);
            CameraManagerRef.Cinemachine3rdPersonFollow.ShoulderOffset = _currentOffset;
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
