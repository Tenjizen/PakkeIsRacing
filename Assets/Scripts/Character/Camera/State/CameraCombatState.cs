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
        private float _baseYaw;

        private float _min, _max;

        private CinemachineBasicMultiChannelPerlin _cameraNoise;

        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("Combat");

            _baseYaw = CamManager.CharacterManager.KayakControllerProperty.transform.rotation.eulerAngles.y;

            if (_baseYaw + CamManager.Data.CombatLengthClamp.x < -360)
            {
                _min = (_baseYaw + CamManager.Data.CombatLengthClamp.x) + 360;
                _max += 360;
            }
            else
            {
                _min = (_baseYaw + CamManager.Data.CombatLengthClamp.x);
            }
            if (_baseYaw + CamManager.Data.CombatLengthClamp.y > 360)
            {
                _max = (_baseYaw + CamManager.Data.CombatLengthClamp.y) - 360;
                _min -= 360;
            }
            else
            {
                _max = (_baseYaw + CamManager.Data.CombatLengthClamp.y);
            }


            if (camera.CinemachineTargetYaw > 0 && _min < 0 && _max < 0)
            {
                _min += 360;
                _max += 360;
            }
            else if (camera.CinemachineTargetYaw < 0 && _min > 0 && _max > 0)
            {
                _min -= 360;
                _max -= 360;
            }

            _baseFov = CamManager.VirtualCameraCombat.m_Lens.FieldOfView;
            _currentFov = _baseFov;

            if (camera.CinemachineTargetYaw > 180 && _max < 180 && _min < 0)
            {
                camera.CinemachineTargetYaw -= 360;
            }
            else if (camera.CinemachineTargetYaw < -180 && _max > -180 && _min > 0)
            {
                camera.CinemachineTargetYaw += 360;
            }

            _cameraNoise = CamManager.VirtualCameraCombat.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cameraNoise.m_AmplitudeGain = CharacterManager.Instance.CurrentProjectile.Data.CameraBaseShakeAmount;

            CombatSensitivityMultiplier = CamManager.Data.CameraCombatSensibility;
        }

        public override void UpdateState(CameraManager camera)
        {
            if (Mathf.Abs(CamManager.RotationZ) > 0)
            {
                CamManager.SmoothResetRotateZ();
            }

            CamManager.CurrentStateBase.ManageFreeCameraMove(CameraMode.Combat);
            
            CamManager.CinemachineTargetPitch = ClampAngle(CamManager.CinemachineTargetPitch, CamManager.Data.CombatHeightClamp.x, CamManager.Data.CombatHeightClamp.y);

            CamManager.CinemachineTargetYaw = ClampAngle(CamManager.CinemachineTargetYaw, _min, _max);

            ClampRotationCameraValue(CamManager.Data.CombatHeightClamp, CamManager.Data.CombatLengthClamp);

            //fov
            const float fovLerp = 0.05f;
            if (CharacterManager.Instance.InputManagementProperty.Inputs.Shoot && CharacterManager.Instance.WeaponCooldown <= 0)
            {
                float lerp = CamManager.Data.CombatZoomFovLerp;
                _targetFOV = Mathf.Lerp(_targetFOV, CamManager.Data.CombatZoomFov, lerp);
                FreeAimMultiplier = Mathf.Lerp(FreeAimMultiplier, CamManager.Data.CombatZoomAimSpeedMultiplier, lerp);
                _cameraNoise.m_AmplitudeGain = Mathf.Lerp(_cameraNoise.m_AmplitudeGain, CharacterManager.Instance.CurrentProjectile.Data.CameraStabilizedShakeAmount, lerp);
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
        public override void LateUpdate(CameraManager camera)
        {
            CamManager.ApplyRotationCameraInCombat();
        }
        public override void SwitchState(CameraManager camera)
        {

        }
    }
}
