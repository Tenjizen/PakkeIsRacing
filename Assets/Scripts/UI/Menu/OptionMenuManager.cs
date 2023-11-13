using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Character.State;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class OptionMenuManager : MenuController
    {
        [SerializeField, ReadOnly] public bool CanBeOpened = true;
        [SerializeField] private UIMenuManager _menuManager;
        
        [SerializeField] private ParametersMenu _parametersMenu;
        [SerializeField] private ControllerMenu _menuController;
        [SerializeField] private CreditsMenu creditsMenu;
        
        [SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private Image _backgroundImage;
        
        private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
        private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();

        private int _index;

        protected override void Start()
        {
            _index = 0;

            foreach (Image image in MenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                if (image == _backgroundImage)
                {
                    _backgroundImage.DOFade(0, 0);
                    continue;
                }
                _imagesDictionary.Add(image, image.color.a);
                image.DOFade(0, 0).SetUpdate(true);
            }
            foreach (TMP_Text text in MenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text, text.color.a);
                text.DOFade(0, 0).SetUpdate(true);
            }

            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowLeaveMenu.started += AbleDisable;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuDown.started += Down;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuUp.started += Up;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ClosePauseMenu.started += CloseMenu;
        }

        public override void SetMenu(bool isActive, bool isUsable)
        {
            if (isUsable == false)
            {
                return;
            }
            base.SetMenu(isActive, isUsable);

            if (IsUsable)
            {
                SetTile();
            }
        }

        private void AbleDisable(InputAction.CallbackContext context)
        {
            if (CharacterManager.Instance.IsGameLaunched == false || CanBeOpened == false || CharacterManager.Instance.CurrentStateBaseProperty.CanOpenMenus == false)
            {
                return;
            }

            OpenCloseMenu();
        }


        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex <= 0)
            {
                return;
            }

            base.Up(context);
            _index--;
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex >= Height || _index + 1 >= _objectsList.Count)
            {
                return;
            }

            base.Down(context);
            _index++;
            SetTile();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE
            Application.Quit();
#endif
        }

        public void OpenCloseMenu()
        {
            const float fadeTime = 0.1f;

            if (_parametersMenu.IsActive == false && _menuController.IsActive == false && creditsMenu.IsActive == false)
            {
                CharacterManager characterManager = CharacterManager.Instance;
                characterManager.CurrentStateBaseProperty.CanCharacterMove = IsActive;
                characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = IsActive;
                characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = IsActive;

                float fadeValue = IsActive == false ? 0.8f : 0;
                _backgroundImage.DOFade(fadeValue, fadeTime).SetUpdate(true);
            }

            IsActive = IsActive == false;

            foreach (var pair in _imagesDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime).SetUpdate(true);
            }
            foreach (var pair in _textsDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime).SetUpdate(true);
            }

            IsUsable = IsActive;

            if (IsActive == false)
            {
                Time.timeScale = 1;
                _menuManager.CanBeOpened = true;
                return;
            }

            SetTile();
            Time.timeScale = 0f;
            _menuManager.CanBeOpened = false;
            if (_menuManager.IsActive == true)
            {
                _menuManager.SetMenu();
            }
        }

        private void CloseMenu(InputAction.CallbackContext context)
        {
            if (IsActive == false)
            {
                return;
            }

            OpenCloseMenu();
        }

        public void OpenParameters()
        {
            CanBeOpened = false;
            _parametersMenu.SetMenu();
            OpenCloseMenu();
            Time.timeScale = 0f;
        }

        public void OpenController()
        {
            CanBeOpened = false;
            _menuController.SetMenu();
            OpenCloseMenu();
            Time.timeScale = 0f;
        }
        public void OpenCredits()
        {
            CanBeOpened = false;
            creditsMenu.SetMenu();
            OpenCloseMenu();
            Time.timeScale = 0f;
        }

        private void SetTile()
        {
            if (_objectsList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < _objectsList.Count; i++)
            {
                _objectsList[i].Set(i == _index);
                _objectsList[i].IsSelected = i == _index;
            }
        }

        public void CloseMenu()
        {
            transform.DOMove(transform.position, 0.005f).SetUpdate(true).OnComplete(SetMenuFromCloseMenu);
        }

        private void SetMenuFromCloseMenu()
        {
            if (IsActive)
            {
                return;
            }

            CanBeOpened = true;
            OpenCloseMenu();
        }

        public void LastCheckpoint()
        {
            OpenCloseMenu();

            CanBeOpened = false;

            CharacterManager.Instance.RespawnLastCheckpoint = true;

            CharacterDeathState characterDeathState = new CharacterDeathState();
            CharacterManager.Instance.SwitchState(characterDeathState);

        }
    }
}
