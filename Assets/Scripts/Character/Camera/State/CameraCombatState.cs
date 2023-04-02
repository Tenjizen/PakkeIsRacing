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
        private float _targetFOV;

        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("Combat");

            _baseFov = CamManager.VirtualCameraCombat.m_Lens.FieldOfView;
            _currentFov = _baseFov;

            _targetOffset = CamManager.CombatBaseShoulderOffset + CamManager.Data.CombatOffset;
        }
        
        public override void UpdateState(CameraManager camera)
        {
            CamManager.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            CamManager.ApplyRotationCameraInCombat();

            CamManager.CinemachineTargetPitch = ClampAngle(CamManager.CinemachineTargetPitch, CamManager.Data.HeightClamp.x, CamManager.Data.HeightClamp.y);

            ClampRotationCameraValue(CamManager.Data.HeightClamp.x, CamManager.Data.HeightClamp.y);

            //fov
            const float fovLerp = 0.05f;
            if (CharacterManager.Instance.InputManagementProperty.Inputs.Shoot && CharacterManager.Instance.WeaponCooldown <= 0)
            {
                _targetFOV = Mathf.Lerp(_targetFOV,CamManager.Data.CombatZoomFov, CamManager.Data.CombatZoomFovLerp);
            }
            else
            {
                _targetFOV = CamManager.Data.CombatFov;
            }
            CamManager.CurrentStateBase.SetFOV(CamManager.VirtualCameraCombat, _currentFov);
            _currentFov = Mathf.Lerp(_currentFov, _targetFOV, fovLerp);

            const float offsetLerp = 0.05f;
            _currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, offsetLerp);
            CamManager.CinemachineCombat3RdPersonFollow.ShoulderOffset = _currentOffset;
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
