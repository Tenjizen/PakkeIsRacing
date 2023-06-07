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
            CamManager.ShakeCameraWarning(0);
        }
        public override void UpdateState(CameraManager camera)
        {
            if (CharacterManager.Instance.IsGameLaunched == false)
            {
                return;
            }
            
            if (_startTimer)
            {
                _timer += Time.deltaTime;
            }

            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanOpenMenus = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = false;
            camera.CanMoveCameraManually = false;

            if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            {
                CharacterManager.Instance.StartGame.Invoke();
                _startTimer = true;
                CamManager.CameraAnimator.Play("FreeLook");

            }
            if (_timer >= camera.TimerBeforeCanMovingAtStart)
            {
                SwitchState(camera);
                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CamManager.SwitchState(cameraNavigationState);
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
            camera.CanMoveCameraManually = true;
            
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanOpenMenus = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = true;
        }
    }
}
