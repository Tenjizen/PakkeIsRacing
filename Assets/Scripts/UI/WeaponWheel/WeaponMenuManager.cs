using System;
using System.Runtime.CompilerServices;
using Character;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UI.WeaponWheel
{
    public class WeaponMenuManager : MonoBehaviour
    {
        [SerializeField] private InputManagement _inputManagement;
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private GameObject _weaponUI;
        [SerializeField] private WeaponWheelButtonController _harpoonButton;
        [SerializeField] private WeaponWheelButtonController _netButton;

        private bool _isMenuOpen;

        private void Start()
        {
            _weaponUI.SetActive(_isMenuOpen);
        }

        private void Update()
        {
            if (Input.GetJoystickNames().Length <= 0)
            {
                return;
            }
            
            if ((_inputManagement.Inputs.OpenWeaponMenu && _isMenuOpen == false) || 
                (_inputManagement.Inputs.OpenWeaponMenu == false && _isMenuOpen))
            {
                PressMenu();
            }

            if (_isMenuOpen)
            {
                WeaponChoice();
            }
        }

        private void PressMenu()
        {
            _characterManager.CurrentStateBase.CanCharacterMove = _isMenuOpen;
            _characterManager.CurrentStateBase.CanCharacterMakeActions = _isMenuOpen;
            _cameraManager.CanMoveCameraManually = _isMenuOpen;
            
            _isMenuOpen = _isMenuOpen == false;
            
            //change player state
            if (_isMenuOpen == false)
            {
                WeaponWheelButtonController button = EventSystem.current.currentSelectedGameObject.GetComponent<WeaponWheelButtonController>();
                if (button != null)
                {
                    button.Select();
                }
            }
            ResetSelection();
            
            _weaponUI.SetActive(_isMenuOpen);
            Time.timeScale = _isMenuOpen ? 1f : 1f;
        }

        private void WeaponChoice()
        {
            const float DEADZONE = 0.5f;
            switch (_inputManagement.Inputs.SelectWeaponMenu)
            {
                case < -DEADZONE:
                    _harpoonButton.Hover();
                    _netButton.Exit();
                    break;
                case > DEADZONE:
                    _netButton.Hover();
                    _harpoonButton.Exit();
                    break;
                default:
                    _netButton.Exit();
                    _harpoonButton.Exit();
                    break;
            }
        }

        public void ResetSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
