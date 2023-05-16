using UnityEngine;
using Character.State;

namespace Character.Camera.State
{
    public class CameraStartGameState : CameraStateBase
    {

        public override void EnterState(CameraManager camera)
        {
            CamManager.CameraAnimator.Play("StartGame");

        }
        public override void UpdateState(CameraManager camera)
        {
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = false;
            camera.CanMoveCameraManually = false;

            if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            {
                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CamManager.SwitchState(cameraNavigationState);
                this.SwitchState(camera);
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
