using UnityEngine;
using Character.State;

namespace Character.Camera.State
{
    public class CameraStartGameState : CameraStateBase
    {
        private bool _startTimer;
        private float _timer;
        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("StartGame");
            CamManager.ShakeCamera(0);

        }
        public override void UpdateState(CameraManager camera)
        {

            if (_startTimer == true)
            {
                _timer += Time.deltaTime;
            }

            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = false;
            camera.CanMoveCameraManually = false;

            if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            {
                _startTimer = true;
                CamManager.CameraAnimator.Play("FreeLook");

                if (_timer >= camera.TimerBeforeCanMovingAtStart)
                {
                    CameraNavigationState cameraNavigationState = new CameraNavigationState();
                    this.SwitchState(camera);
                    CamManager.SwitchState(cameraNavigationState);
                }
            }
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void LateUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {
            Debug.Log("switch");
            camera.CanMoveCameraManually = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = true;
        }
    }
}
