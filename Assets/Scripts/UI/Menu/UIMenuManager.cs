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
        [SerializeField, ReadOnly] protected bool IsActive = false;

        private Dictionary<Image,float> _imagesDictionnary = new Dictionary<Image,float>();
        private Dictionary<TMP_Text,float> _textsDictionnary = new Dictionary<TMP_Text,float>();

        private void Start()
        {
            foreach (Image image in PauseMenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                _imagesDictionnary.Add(image,image.color.a);
                image.DOFade(0, 0);
            }
            foreach (TMP_Text text in PauseMenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionnary.Add(text,text.color.a);
                text.DOFade(0, 0);
            }
            
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowPauseMenus.started += Set;
        }

        protected virtual void Set(InputAction.CallbackContext context)
        {
            CharacterManager characterManager = CharacterManager.Instance;
            characterManager.CurrentStateBaseProperty.CanCharacterMove = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = IsActive;
            characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons = IsActive;

            IsActive = IsActive == false;

            const float fadetime = 0.2f;
            foreach (var pair in _imagesDictionnary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadetime);
            }
            foreach (var pair in _textsDictionnary)
            {
                pair.Key.DOFade(IsActive ? pair.Value : 0, fadetime);
            }
            
            PauseMenuTopSelection.Set(IsActive,IsActive);
        }
    }
}