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
            _timerFadeOutStart = 0;
        }
        bool spawn;
        private void Respawn(Vector3 vector3, CharacterManager character)
        {
            if (spawn == true) return;

            //put kayak in checkpoint position & rotation
            vector3.y = GameManager.Instance.WavesRef.GetHeight(vector3);
            _kayakController.transform.localPosition = vector3;

            _kayakController.transform.eulerAngles = new Vector3(_kayakController.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, _kayakController.transform.eulerAngles.z);
            _kayakController.Rigidbody.velocity = Vector3.zero;

            spawn = true;
            //_timerFadeOutStart += Time.deltaTime;
            Debug.Log("respawn");
            SwitchState(character);

        }

        public override void UpdateState(CharacterManager character)
        {

            Respawn(CharacterManagerRef.InCam.TargetRespawn,character);

            //if (_timerFadeOutStart > 0.3f)
            //{
            //    SwitchState(character);
            //}

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
            CharacterManagerRef.InCam.MultipleTargetCamera.AddTarget(_kayakController.transform, 1);
            CharacterManagerRef.InCam.IsInCameraViewValue = true;
            CharacterManagerRef.InCam.Timer = 0;
            Debug.Log("tf");
            character.SwitchState(characterNavigationState);

        }

        public override void ExitState(CharacterManager character)
        {
        }
    }
}