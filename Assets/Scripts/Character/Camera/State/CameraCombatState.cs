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

        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("Combat");

            _baseFov = CamManager.VirtualCameraCombat.m_Lens.FieldOfView;
            _currentFov = _baseFov;

            _targetOffset = CamManager.CombatBaseShoulderOffset + CamManager.Data.CombatOffset;

            _cameraNoise = CamManager.VirtualCameraCombat.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cameraNoise.m_AmplitudeGain = 1f;
        }
        
        public override void UpdateState(CameraManager camera)
        {
            CamManager.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            CamManager.ApplyRotationCameraInCombat();

            CamManager.CinemachineTargetPitch = ClampAngle(CamManager.CinemachineTargetPitch, CamManager.Data.CombatHeightClamp.x, CamManager.Data.CombatHeightClamp.y);
            CamManager.CinemachineTargetYaw = ClampAngle(CamManager.CinemachineTargetYaw, CamManager.Data.CombatLengthClamp.x, CamManager.Data.CombatLengthClamp.y);

            ClampRotationCameraValue(CamManager.Data.CombatHeightClamp, CamManager.Data.CombatLengthClamp);  

            //fov
            const float fovLerp = 0.05f;
            if (CharacterManager.Instance.InputManagementProperty.Inputs.Shoot && CharacterManager.Instance.WeaponCooldown <= 0)
            {
                float lerp = CamManager.Data.CombatZoomFovLerp;
                _targetFOV = Mathf.Lerp(_targetFOV,CamManager.Data.CombatZoomFov, lerp);
                FreeAimMultiplier = Mathf.Lerp(FreeAimMultiplier, CamManager.Data.CombatZoomAimSpeedMultiplier, lerp);
                _cameraNoise.m_AmplitudeGain = Mathf.Lerp(_cameraNoise.m_AmplitudeGain, 0, lerp);
            }
            else
            {
                _targetFOV = CamManager.Data.CombatFov;
                FreeAimMultiplier = 1f;
            }
            CamManager.CurrentStateBase.SetFOV(CamManager.VirtualCameraCombat, _currentFov);
            _currentFov = Mathf.Lerp(_currentFov, _targetFOV, fovLerp);

            //offset
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
