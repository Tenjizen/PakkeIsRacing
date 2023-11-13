using Kayak;
using UnityEngine;

namespace Character.State
{
    public class CharacterDeathState : CharacterStateBase
    {
        private KayakController _kayakController;
        private InputManagement _inputs;

        private bool _transitionIn = false;
        private bool _respawned = false;
        private bool _cameraSwitchState = false;
        private float _timerToRespawnCheckpoint = 0;
        private float _timerFadeOutStart = 0;
        private float _speed = 100;

        public CharacterDeathState() : base()
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
            _inputs = CharacterManagerRef.InputManagementProperty;
        }

        public override void EnterState(CharacterManager character)
        {
            IsDead = true;
            CharacterManagerRef.ScriptDebug.ResetTimerDebug();

            CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeColor();
            CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeScale();
        }
        public override void UpdateState(CharacterManager character)
        {
            if (character.RespawnLastCheckpoint == false)
            {
                //Rotate kayak at 180 in z with balance
                if (CharacterManagerRef.Balance > 0 && CharacterManagerRef.Balance < 60)
                {
                    CharacterManagerRef.Balance += Time.deltaTime * _speed;
                }
                else if (CharacterManagerRef.Balance < -0 && CharacterManagerRef.Balance > -60)
                {
                    CharacterManagerRef.Balance -= Time.deltaTime * _speed;
                }
            }

            //Switch camera
            if (Mathf.Abs(CharacterManagerRef.Balance) >= 60 && _cameraSwitchState == false || character.RespawnLastCheckpoint == true && _cameraSwitchState == false)
            {
                _cameraSwitchState = true;
                CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeDisable();
            }
            MakeBoatRotationWithBalance(_kayakController.transform, 1);


            //Timer transition In
            if (_transitionIn && _respawned == false)
            {
                _timerToRespawnCheckpoint += Time.deltaTime;
            }

            if (_timerFadeOutStart > 1.5f && _respawned)
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
            CharacterManagerRef.TransitionManagerProperty.LaunchTransitionOut(SceneTransition.TransitionType.Fade);


            //Switch state character
            CharacterNavigationState characterNavigationState = new CharacterNavigationState();
            CharacterManagerRef.SwitchState(characterNavigationState);

        }

        public override void ExitState(CharacterManager character)
        {
            CharacterManagerRef.Balance = 0;
        }

        private void RespawnCheckpoint(Transform checkpoint)
        {
            if (_respawned == true)
            {
                _timerFadeOutStart += Time.deltaTime;
            }
            else
            {
                //put kayak in checkpoint position & rotation
                _kayakController.transform.position = checkpoint.position;
                _kayakController.transform.rotation = checkpoint.rotation;

                //Reset value
                _kayakController.CanReduceDrag = true;
                CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                _respawned = true;
            }
        }
    }


}