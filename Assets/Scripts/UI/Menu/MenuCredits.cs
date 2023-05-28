using Character;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuCredits : MenuController
{
    [SerializeField] private OptionMenuManager _optionMenuControllerManager;
    private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
    private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();

    [SerializeField, ReadOnly] public bool CanBeOpened = true;

    protected override void Start()
    {
        //foreach (Image image in MenuGameObject.transform.GetComponentsInChildren<Image>())
        //{
        //    _imagesDictionary.Add(image, image.color.a);
        //    image.DOFade(0, 0);
        //}
        foreach (TMP_Text text in MenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
        {
            _textsDictionary.Add(text, text.color.a);
            text.DOFade(0, 0);
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

    public override void SetMenu(bool isActive, bool isUsable)
    {
        base.SetMenu(isActive, isUsable);
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
        //foreach (var pair in _imagesDictionary)
        //{
        //    pair.Key.DOKill();
        //    pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
        //}
        foreach (var pair in _textsDictionary)
        {
            pair.Key.DOKill();
            pair.Key.DOFade(IsActive ? pair.Value : 0, fadeTime);
        }

        IsUsable = IsActive;
    }
}
