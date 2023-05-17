using System;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuManager : MonoBehaviour
    {
        [SerializeField] protected MenuController PauseMenuTopSelection;
        [SerializeField] protected GameObject PauseMenuGameObject;
        [SerializeField, ReadOnly] public bool IsActive = false;
        [SerializeField, ReadOnly] public bool CanBeOpened = true;

        private Dictionary<Image,float> _imagesDictionary = new Dictionary<Image,float>();
        private Dictionary<TMP_Text,float> _textsDictionary = new Dictionary<TMP_Text,float>();

        private void Start()
        {
            foreach (Image image in PauseMenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                _imagesDictionary.Add(image,image.color.a);
                image.DOFade(0, 0);
            }
            foreach (TMP_Text text in PauseMenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text,text.color.a);
                text.DOFade(0, 0);
            }
            
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowPauseMenus.started += Set;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ClosePauseMenu.started += CloseMenu;
        }

        private void Set(InputAction.CallbackContext context)
        {
            if (CanBeOpened == false)
            {
                return;
            }
            SetMenu();
        }

        public void SetMenu()
        {
            CharacterManager characterManager = CharacterManager.Instance;
            characterManager.CurrentStateBaseProperty.CanCharacterMove = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = IsActive;

            CharacterManager.Instance.CameraManagerProperty.CanRotateCamera = IsActive;
            IsActive = IsActive == false;

            const float fadeTime = 0.2f;
            foreach (var pair in _imagesDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
            }
            foreach (var pair in _textsDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
            }

            PauseMenuTopSelection.Set(IsActive, IsActive);
        }

        private void CloseMenu(InputAction.CallbackContext context)
        {
            if (IsActive == false)
            {
                return;
            }
            
            Set(context);
        }


    }
}