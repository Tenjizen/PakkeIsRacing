using System;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class PauseMenuUIManager : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool _isActive = false;
        [SerializeField] private GameObject _pauseMenuUIGameObject;
        [SerializeField] private List<PauseMenuUIButtonController> _buttonsList = new List<PauseMenuUIButtonController>();

        private int _index;

        private void Start()
        {
            _buttonsList.ForEach(x => x.Set(false));
            _buttonsList[_index].Set(true);
            _pauseMenuUIGameObject.SetActive(false);
            
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowPauseMenus.started += Set;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.PauseMenuPrevious.started += Previous;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.PauseMenuNext.started += Next;
        }

        private void Previous(InputAction.CallbackContext context)
        {
            if (_index > 0)
            {
                _buttonsList[_index].Set(false);
                _index--;
                _buttonsList[_index].Set(true);
            }
        }
        
        private void Next(InputAction.CallbackContext context)
        {
            if (_index < _buttonsList.Count - 1)
            {
                _buttonsList[_index].Set(false);
                _index++;
                _buttonsList[_index].Set(true);
            }
        }

        private void Set(InputAction.CallbackContext context)
        {
            Debug.Log("pause menu set");
            
            CharacterManager characterManager = CharacterManager.Instance;
            characterManager.CurrentStateBaseProperty.CanCharacterMove = _isActive;
            characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = _isActive;
            characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = _isActive;
            
            _isActive = _isActive == false;
            _pauseMenuUIGameObject.SetActive(_isActive);
            _buttonsList[_index].Set(true);
        }
    }
}
