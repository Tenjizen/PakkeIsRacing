using Kayak;
using UnityEngine;

namespace Character.State
{
    public class CharacterDeathState : CharacterStateBase
    {
        private KayakController _kayakController;

        private bool _transitionIn = false;
        private bool _respawned = false;
        private bool _cameraSwitchState = false;
        private float _timerToRespawnCheckpoint = 0;
        private float _timerFadeOutStart = 0;
        private float _speed = 100;

        public CharacterDeathState(CharacterMultiPlayerManager character) : base(character)
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
        }

        public override void EnterState(CharacterManager character)
        {
            IsDead = true;
            Debug.Log("mort");


            //character.BalanceGaugeManagerRef.SetBalanceGaugeColor();
            //character.BalanceGaugeManagerRef.SetBalanceGaugeScale();
        }
        private void Respawn(Vector3 vector3)
        {
            //put kayak in checkpoint position & rotation
            _kayakController.transform.localPosition = vector3;
            _timerFadeOutStart += Time.deltaTime;

            //Reset value
            //_kayakController.CanReduceDrag = true;
            //CameraManagerRef.CanMoveCameraManually = true;
            //CharacterManagerRef.SetBalanceValueToCurrentSide(0);
            //_respawned = true;

            //Switch state camera
            //CameraRespawnState cameraRespawnState = new CameraRespawnState();
            //CameraManagerRef.SwitchState(cameraRespawnState);


            //CharacterNavigationState characterNavigationState = new CharacterNavigationState(Character);
        }
        public override void UpdateState(CharacterManager character)
        {

            Respawn(CharacterManagerRef.InCam.TargetRespawn);

            //this.SwitchState(character);

            //if (character.RespawnLastCheckpoint == false)
            //{
            //    //Rotate kayak at 180 in z with balance
            //    if (character.Balance > 0 && character.Balance < 60)
            //    {
            //        character.Balance += Time.deltaTime * _speed;
            //    }
            //    else if (character.Balance < -0 && character.Balance > -60)
            //    {
            //        character.Balance -= Time.deltaTime * _speed;
            //    }
            //}

            ////Switch camera
            //if (Mathf.Abs(character.Balance) >= 60 && _cameraSwitchState == false || character.RespawnLastCheckpoint == true && _cameraSwitchState == false)
            //{
            //    _cameraSwitchState = true;
            //    character.BalanceGaugeManagerRef.SetBalanceGaugeDisable();
            //}
            //MakeBoatRotationWithBalance(_kayakController.transform, 1);


            //Timer transition In
            //if (_transitionIn && _respawned == false)
            //{
            //    _timerToRespawnCheckpoint += Time.deltaTime;
            //}

            if (_timerFadeOutStart > 0.5f)
            {
                SwitchState(character);
            }

        }

        public override void FixedUpdate(CharacterManager character)
        {
        }

        public override void SwitchState(CharacterManager character)
        {
            //Transition out
            //character.TransitionManagerProperty.LaunchTransitionOut(SceneTransition.TransitionType.Fade);

            IsDead = false;

            //Switch state character
            CharacterNavigationState characterNavigationState = new CharacterNavigationState(Character);
            characterNavigationState.CanBeMoved = true;
            character.SwitchState(characterNavigationState);

        }

        public override void ExitState(CharacterManager character)
        {
            //character.Balance = 0;
        }
    }


}