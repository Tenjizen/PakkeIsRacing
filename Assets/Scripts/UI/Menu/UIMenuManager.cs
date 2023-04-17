using System;
using Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class UIMenuManager : MonoBehaviour
    {
        [SerializeField] protected MenuController PauseMenuTopSelection;
        [SerializeField] protected GameObject PauseMenuGameObject;
        [SerializeField, ReadOnly] protected bool IsActive = false;

        private void Start()
        {
            PauseMenuGameObject.gameObject.SetActive(false);
            
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowPauseMenus.started += Set;
        }

        protected virtual void Set(InputAction.CallbackContext context)
        {
            CharacterManager characterManager = CharacterManager.Instance;
            characterManager.CurrentStateBaseProperty.CanCharacterMove = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = IsActive;

            IsActive = IsActive == false;
            PauseMenuGameObject.gameObject.SetActive(IsActive);
            PauseMenuTopSelection.Set(true,true);
        }
    }
}