using Kayak;
using UnityEngine;

namespace Character.State
{
    public class CharacterDeathState : CharacterStateBase
    {
        private KayakController _kayakController;

        private float _timerFadeOutStart = 0;

        public CharacterDeathState(CharacterMultiPlayerManager character) : base(character)
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
        }

        public override void EnterState(CharacterManager character)
        {
            IsDead = true;

        }
        private void Respawn(Vector3 vector3)
        {
            //put kayak in checkpoint position & rotation
            _kayakController.transform.localPosition = vector3;

            var rota = _kayakController.transform.localRotation;
            rota.y = CharacterManagerRef.InCam.CameraMain.transform.rotation.y;
            _kayakController.transform.localRotation = rota;

            _kayakController.Rigidbody.velocity = Vector3.zero;

            _timerFadeOutStart += Time.deltaTime;

        }
        public override void UpdateState(CharacterManager character)
        {

            Respawn(CharacterManagerRef.InCam.TargetRespawn);

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
            IsDead = false;

            //Switch state character
            CharacterNavigationState characterNavigationState = new CharacterNavigationState(Character);
            characterNavigationState.CanBeMoved = true;
            CharacterManagerRef.InCam.MultipleTargetCamera.AddTarget(_kayakController.transform);

            character.SwitchState(characterNavigationState);

        }

        public override void ExitState(CharacterManager character)
        {
        }
    }


}