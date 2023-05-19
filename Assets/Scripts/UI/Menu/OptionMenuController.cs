using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class OptionMenuController : MenuController
    {
        [SerializeField, ReadOnly] public bool CanBeOpened = true;
        [SerializeField] private UIMenuManager _menuManager;
        [SerializeField] private ParametersMenu _parametersMenu;
        [SerializeField] private ControllerMenu _menuController;
        [SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private TMP_Text _text;
        
        private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
        private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();

        private int _index;
        
        private void Awake()
        {
            Color color = Color.white;
            _text.color = color;
        }

        protected override void Start()
        {
            _index = 0;
            
            foreach (Image image in MenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                _imagesDictionary.Add(image, image.color.a);
                image.DOFade(0, 0);
            }
            foreach (TMP_Text text in MenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text, text.color.a);
                text.DOFade(0, 0);
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
            if (CanBeOpened == false)
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
            CharacterManager characterManager = CharacterManager.Instance;
            characterManager.CurrentStateBaseProperty.CanCharacterMove = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = IsActive;
            characterManager.CameraManagerProperty.CanRotateCamera = IsActive;

            IsActive = IsActive == false;

            const float fadeTime = 0.1f;
            foreach (var pair in _imagesDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
            }
            foreach (var pair in _textsDictionary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
            }

            IsUsable = IsActive;

            if (IsActive)
            {
                SetTile();
                Time.timeScale = 0.5f;
                _menuManager.CanBeOpened = false;
                if (_menuManager.IsActive == true)
                {
                    _menuManager.SetMenu();
                }
            }
            else if (IsActive == false)
            {
                Time.timeScale = 1;
                _menuManager.CanBeOpened = true;
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
            _parametersMenu.SetMenu();
            SetVariableFalse();
        }

        public void OpenController()
        {
            _menuController.SetMenu();
            SetVariableFalse();
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

        private void SetVariableFalse()
        {
            IsUsable = false;
            CanBeOpened = false;
            IsActive = false;
        }
        
        public void SetVariableTrue()
        {
            StartCoroutine(SetFieldToTrueEnumerator(0.1f));
        }
        
        IEnumerator SetFieldToTrueEnumerator(float time)
        {
            if (IsActive)
            {
                yield break;
            }

            yield return new WaitForSeconds(time);
            IsUsable = true;
            CanBeOpened = true;
            IsActive = true;
        }
    }
}
