using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class CreditsMenu : MenuController
    {
        [SerializeField] private OptionMenuManager _optionMenuControllerManager;
        
        [SerializeField, ReadOnly] public bool CanBeOpened = true;
        
        private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
        private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();
        
        protected override void Start()
        {
            foreach (Image image in MenuGameObject.GetComponentsInChildren<Image>())
            {
                _imagesDictionary.Add(image, image.color.a);
                image.DOFade(0, 0).SetUpdate(true);
            }
            foreach (TMP_Text text in MenuGameObject.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text, text.color.a);
                text.DOFade(0, 0).SetUpdate(true);
            }

            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ClosePauseMenu.started += CloseMenu;

            base.SetMenu(IsActive, IsUsable);
        }

        private void CloseMenu(InputAction.CallbackContext context)
        {
            if (IsActive == false)
            {
                return;
            }

            SetMenu();
            _optionMenuControllerManager.CloseMenu();
        }

        public void SetMenu()
        {
            if (CanBeOpened == false)
            {
                return;
            }

            IsActive = IsActive == false;
            MenuGameObject.SetActive(IsActive);

            const float fadeTime = 0.1f;
            foreach (var pair in _imagesDictionary)
            {
                pair.Key.DOKill();
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime).SetUpdate(true);
            }
            foreach (var pair in _textsDictionary)
            {
                pair.Key.DOKill();
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime).SetUpdate(true);
            }

            IsUsable = IsActive;
        }
    }
}
