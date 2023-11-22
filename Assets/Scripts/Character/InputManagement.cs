using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Character
{
    public class InputManagement : MonoBehaviour
    {
        private PlayerConfig _pc;

        private GameplayInputs _gameplayInputs;

        public GameplayInputs GameplayInputs { get { return _gameplayInputs; } private set { _gameplayInputs = value; } }
        [SerializeField] float DeadzoneJoystick = 0.3f;
        [SerializeField] float DeadzoneJoystickTrigger = 0.3f;
        [field: SerializeField] public InputsEnum Inputs { get; private set; }

        private void Awake()
        {
            _gameplayInputs = new GameplayInputs();
            _gameplayInputs.Enable();

        }

        private void Update()
        {
            //GatherInputs();
        }

        public void InitPlayer(PlayerConfig pc)
        {
            _pc = pc;
            _pc.Input.onActionTriggered += Input_onActionTriggered;

        }

        void Input_onActionTriggered(CallbackContext obj)
        {
            InputsEnum inputsEnum = Inputs;

            if (obj.action.name == _gameplayInputs.Boat.PaddleLeft.name)
                inputsEnum.PaddleLeft = obj.ReadValue<float>() > DeadzoneJoystickTrigger;

            if (obj.action.name == _gameplayInputs.Boat.PaddleRight.name)
                inputsEnum.PaddleRight = obj.ReadValue<float>() > DeadzoneJoystickTrigger;

            if (obj.action.name == _gameplayInputs.Boat.StaticRotateLeft.name)
                inputsEnum.RotateLeft = obj.ReadValue<float>();

            if (obj.action.name == _gameplayInputs.Boat.StaticRotateRight.name)
                inputsEnum.RotateRight = obj.ReadValue<float>();

            inputsEnum.Deadzone = DeadzoneJoystick;

            if (obj.action.name == _gameplayInputs.Boat.AnyButton.name)
                inputsEnum.AnyButton = obj.ReadValue<float>() > DeadzoneJoystickTrigger;

            if (obj.action.name == _gameplayInputs.Boat.ShowLeaveMenu.name)
                inputsEnum.Start = obj.ReadValue<float>() > DeadzoneJoystickTrigger; 
            


            if (obj.action.name == _gameplayInputs.Boat.Purify.name)
                inputsEnum.Purify = obj.ReadValue<float>() > DeadzoneJoystickTrigger;




            Inputs = inputsEnum;
        }





        private void GatherInputs()
        {
            InputsEnum inputsEnum = Inputs;

            //_pc.Input.currentActionMap.

            inputsEnum.PaddleLeft = _gameplayInputs.Boat.PaddleLeft.ReadValue<float>() > DeadzoneJoystickTrigger;
            inputsEnum.PaddleRight = _gameplayInputs.Boat.PaddleRight.ReadValue<float>() > DeadzoneJoystickTrigger;

            inputsEnum.RotateLeft = _gameplayInputs.Boat.StaticRotateLeft.ReadValue<float>();
            inputsEnum.RotateRight = _gameplayInputs.Boat.StaticRotateRight.ReadValue<float>();

            inputsEnum.Deadzone = DeadzoneJoystick;

            inputsEnum.AnyButton = _gameplayInputs.Boat.AnyButton.ReadValue<float>() > 0.3f;
            inputsEnum.Start = _gameplayInputs.Boat.ShowLeaveMenu.ReadValue<float>() > 0.3f;

            Inputs = inputsEnum;

        }
    }

    [Serializable]
    public struct InputsEnum
    {
        public bool PaddleLeft;
        public bool PaddleRight;

        public float RotateLeft;
        public float RotateRight;

        public float Deadzone;

        public bool Unbalanced;

        public bool AnyButton;
        public bool Start;

        public bool Purify;
    }
}