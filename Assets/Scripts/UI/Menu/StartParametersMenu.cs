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
    public class StartParametersMenu : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();

        private List<Image> _imagesDictionary = new List<Image>();
        private List<TMP_Text> _textsDictionary = new List<TMP_Text>();

        [SerializeField, ReadOnly] public bool CanBeOpened = true;

        private int _index;

        protected override void Start()
        {
            _index = 0;
            foreach (Image image in MenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                _imagesDictionary.Add(image);
            }
            foreach (TMP_Text text in MenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text);
            }

            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuDown.started += Down;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuUp.started += Up;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuLeft.started += Left;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuRight.started += Right;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowLeaveMenu.started += CloseMenu;

            IsActive = true;
            IsUsable = true;
            
            SetTile();
        }

        private void CloseMenu(InputAction.CallbackContext context)
        {
            IsActive = false;
            
            foreach (var item in _objectsList)
            {
                item.IsSelected = false;
            }
            
            const float fadeTime = 1f;
            foreach (Image image in _imagesDictionary)
            {
                image.DOKill();
                image.DOFade(0, fadeTime).SetUpdate(true);
            }
            foreach (TMP_Text txt in _textsDictionary)
            {
                txt.DOKill();
                txt.DOFade(0, fadeTime).SetUpdate(true);
            }

            IsUsable = IsActive;
            
            StartCoroutine(SetGameLaunched(fadeTime));
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
        
        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }

            _objectsList[_index].Activate(new InputAction.CallbackContext());
        }
        
        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }

            _objectsList[_index].Activate(new InputAction.CallbackContext());
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

        private IEnumerator SetGameLaunched(float time)
        {
            yield return new WaitForSeconds(time);
            MenuGameObject.SetActive(IsActive);
            CharacterManager.Instance.IsGameLaunched = true;
        }
    }
}
