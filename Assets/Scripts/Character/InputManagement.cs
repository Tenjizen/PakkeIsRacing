using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Character
{
    public class InputManagement : MonoBehaviour
    {
        private GameplayInputs _gameplayInputs;
        public GameplayInputs GameplayInputs { get { return _gameplayInputs; } private set { } }
        public InputsEnum Inputs { get; private set; }

        private void Awake()
        {
            _gameplayInputs = new GameplayInputs();
            _gameplayInputs.Enable();
        }

        private void Update()
        {
            GatherInputs();
        }


        private void GatherInputs()
        {
            InputsEnum inputsEnum = Inputs;
            
            inputsEnum.PaddleLeft = _gameplayInputs.Boat.PaddleLeft.ReadValue<float>() > 0;
            inputsEnum.PaddleRight = _gameplayInputs.Boat.PaddleRight.ReadValue<float>() > 0;

            inputsEnum.RotateLeft = _gameplayInputs.Boat.StaticRotateLeft.ReadValue<float>();
            inputsEnum.RotateRight = _gameplayInputs.Boat.StaticRotateRight.ReadValue<float>();

            inputsEnum.RotateCameraClick = _gameplayInputs.Boat.RotateCameraActivation.ReadValue<float>() > 0;
            
            inputsEnum.RotateCamera = _gameplayInputs.Boat.RotateCamera.ReadValue<Vector2>();

            inputsEnum.OpenWeaponMenu = _gameplayInputs.Boat.OpenWheelMenu.ReadValue<float>() > 0.5f;
            inputsEnum.SelectWeaponMenu = _gameplayInputs.Boat.SelectOnWheel.ReadValue<float>();
            inputsEnum.DeselectWeapon = _gameplayInputs.Boat.DeselectWeapon.triggered;

            inputsEnum.Aim = _gameplayInputs.Boat.Aim.ReadValue<float>() > 0.5f;
            inputsEnum.Shoot = _gameplayInputs.Boat.Shoot.ReadValue<float>() > 0.5f;
            inputsEnum.MovingAim = _gameplayInputs.Boat.MoveAim.ReadValue<Vector2>();
            
            inputsEnum.DisplayControlScreen = _gameplayInputs.Boat.DisplayControlScreen.ReadValue<float>() > 0.5f;;

            inputsEnum.Deadzone = 0.3f;
            
            Inputs = inputsEnum;
        }
    }
    
    public struct InputsEnum
    {
        public bool PaddleLeft;
        public bool PaddleRight;
        
        public float RotateLeft;
        public float RotateRight;
        
        public bool RotateCameraClick;
        public Vector2 RotateCamera;
        
        public float Deadzone;
        
        public bool OpenWeaponMenu;
        public float SelectWeaponMenu;
        public bool DeselectWeapon;
        
        public bool Aim;
        public bool Shoot;
        public Vector2 MovingAim;

        public bool Unbalanced;

        public bool DisplayControlScreen;
    }
}