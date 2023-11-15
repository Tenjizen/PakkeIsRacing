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

            character.BalanceGaugeManagerRef.SetBalanceGaugeColor();
            character.BalanceGaugeManagerRef.SetBalanceGaugeScale();
        }
        public override void UpdateState(CharacterManager character)
        {
            if (character.RespawnLastCheckpoint == false)
            {
                //Rotate kayak at 180 in z with balance
                if (character.Balance > 0 && character.Balance < 60)
                {
                    character.Balance += Time.deltaTime * _speed;
                }
                else if (character.Balance < -0 && character.Balance > -60)
                {
                    character.Balance -= Time.deltaTime * _speed;
                }
            }

            //Switch camera
            if (Mathf.Abs(character.Balance) >= 60 && _cameraSwitchState == false || character.RespawnLastCheckpoint == true && _cameraSwitchState == false)
            {
                _cameraSwitchState = true;
                character.BalanceGaugeManagerRef.SetBalanceGaugeDisable();
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
            //character.TransitionManagerProperty.LaunchTransitionOut(SceneTransition.TransitionType.Fade);


            //Switch state character
            CharacterNavigationState characterNavigationState = new CharacterNavigationState(Character);
            character.SwitchState(characterNavigationState);

        }

        public override void ExitState(CharacterManager character)
        {
            character.Balance = 0;
        }
    }


}