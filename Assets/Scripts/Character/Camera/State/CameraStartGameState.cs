using UnityEngine;
using Character.State;

namespace Character.Camera.State
{
    public class CameraStartGameState : CameraStateBase
    {
        private bool _isTimerStarted;
        private float _timer;
        
        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("StartGame");
            CamManager.ShakeCameraWarning(0);
            
            CharacterManager.Instance.CharacterAnimatorProperty.SetBool("Sleep",true);
            CharacterManager.Instance.PaddleAnimatorProperty.SetBool("Sleep",true);
        }
        public override void UpdateState(CameraManager camera)
        {
            if (CharacterManager.Instance.IsGameLaunched == false)
            {
                CharacterStateBase character = CharacterManager.Instance.CurrentStateBaseProperty;
                character.CanCharacterMakeActions = false;
                character.CanCharacterOpenWeapons = false;
                character.CanOpenMenus = false;
                character.CanBeMoved = false;
                character.CanCharacterMove = false;
                camera.CanMoveCameraManually = false;
                return;
            }
            
            if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            {
                CharacterManager.Instance.StartGame.Invoke();
                _isTimerStarted = true;
                CamManager.CameraAnimator.Play("FreeLook");
                
                CharacterManager.Instance.CharacterAnimatorProperty.SetTrigger("WakeUp");
                CharacterManager.Instance.CharacterAnimatorProperty.SetBool("Sleep",false);
                
                CharacterManager.Instance.PaddleAnimatorProperty.SetTrigger("WakeUp");
                CharacterManager.Instance.PaddleAnimatorProperty.SetBool("Sleep",false);
            }

            if (_isTimerStarted)
            {
                _timer += Time.deltaTime;
            }

            if (_timer < camera.TimerBeforeCanMovingAtStart)
            {
                return;
            }
            
            SwitchState(camera);
            CameraNavigationState cameraNavigationState = new CameraNavigationState();
            CamManager.SwitchState(cameraNavigationState);
            CharacterManager.Instance.StartCoroutine(CharacterManager.Instance.CameraManagerProperty.LaunchEventStart());
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
            
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanOpenMenus = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = true;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = true;
            camera.CanMoveCameraManually = true;
        }
    }
}
