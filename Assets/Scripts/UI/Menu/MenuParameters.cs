using Character;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuParameters : MenuController
{
        [SerializeField] public MenuLeave menuLeaveManager;
    [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();

    private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
    private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();

    [SerializeField, ReadOnly] public bool CanBeOpened = true;

    private int _index;

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

        //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowLeaveMenu.started += AbleDisable;
        CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuDown.started += Down;
        CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuUp.started += Up;
    }

    public override void Set(bool isActive, bool isUsable)
    {
        base.Set(isActive, isUsable);

        if (isUsable == false)
        {
            return;
        }

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

        SetMenu();
    }

    public void SetMenu()
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

        if (IsActive == true)
        {

            SetTile();
            Time.timeScale = 0.5f;
            menuLeaveManager.CanBeOpened = false;
            //menuLeaveManager.IsUsable = false;
        }
        else if (IsActive == false)
        {
            Time.timeScale = 1;
            menuLeaveManager.CanBeOpened = true;
            //menuLeaveManager.IsUsable = true;
        }
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
}
