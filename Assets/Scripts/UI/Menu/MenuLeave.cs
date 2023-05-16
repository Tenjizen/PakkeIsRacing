using Character;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuLeave : MenuController
{
    [SerializeField] protected GameObject PauseMenuGameObject;
    [SerializeField, ReadOnly] public bool CanBeOpened = true;
    [SerializeField] public UIMenuManager menuManager;

    [SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();


    private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();
    private Dictionary<TMP_Text, float> _textsDictionary = new Dictionary<TMP_Text, float>();

    private int _index;

    [SerializeField] TMP_Text _text;
    private void Awake()
    {
        Color color = Color.white;
        _text.color = color;
    }

    private void Start()
    {
        _index = 0;
        foreach (Image image in PauseMenuGameObject.transform.GetComponentsInChildren<Image>())
        {
            _imagesDictionary.Add(image, image.color.a);
            image.DOFade(0, 0);
        }
        foreach (TMP_Text text in PauseMenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
        {
            _textsDictionary.Add(text, text.color.a);
            text.DOFade(0, 0);
        }

        CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowLeaveMenu.started += AbleDisable;
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
    private void Update()
    {
        for (int i = 0; i < _objectsList.Count; i++)
        {
            Debug.Log(_objectsList[i].IsSelected + " " + i);
        }
    }

    private void AbleDisable(InputAction.CallbackContext context)
    {
        if (CanBeOpened == false)
        {
            return;
        }

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
            Time.timeScale = 0.1f;
            menuManager.CanBeOpened = false;
        }
        else if (IsActive == false)
        {
            Time.timeScale = 1;
            menuManager.CanBeOpened = true;
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

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_STANDALONE
        Application.Quit();
#endif
    }

    public void Resume()
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
            Time.timeScale = 0.1f;
            menuManager.CanBeOpened = false;
        }
        else if (IsActive == false)
        {
            Time.timeScale = 1;
            menuManager.CanBeOpened = true;
        }
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

        //if (_objectsList.Count <= 0)
        //{
        //    return;
        //}

        //for (int i = 0; i < _objectsList.Count; i++)
        //{
        //    _objectsList[i].Set(i == _index);
        //}

        //_objectsList[_index].Set(true);
    }

}
