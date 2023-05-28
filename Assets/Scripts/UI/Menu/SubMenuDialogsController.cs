using System;
using System.Collections.Generic;
using Character;
using Collectible;
using Json;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class SubMenuDialogsController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private TMP_Text _dialogText;
        [SerializeField] private DialogUIObject _dialogUIObjectPrefab;
        [SerializeField] private Transform _dialogsUIObjectLayout;

        private int _index;

        protected override void Start()
        {
            base.Start();
            JsonFilesManagerSingleton.Instance.CollectibleJsonFileManagerProperty.OnNewCollectibleGet.AddListener(SetTilesData);
            SetupDialogDictionary();
        }

        public override void SetMenu(bool isActive, bool isUsable)
        {
            base.SetMenu(isActive, isUsable);

            CreateDialogsUIObject();
            
            if (isUsable == false)
            {
                return;
            }

            if (IsUsable)
            {
                SetTile();
            }
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex <= 0)
            {
                return;
            }
            
            base.Up(context);
            _index --;
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex >= Height || _index+1 >= _objectsList.Count)
            {
                return;
            }
            
            base.Down(context);
            _index ++;
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
            }

            if (_dialogText == null)
            {
                return;
            }

            DialogUIObject dialogUIObject = _objectsList[_index].GetComponent<DialogUIObject>();
            _dialogText.text = dialogUIObject == null ? _objectsList[_index].GetName() : dialogUIObject.Data.Summary;
        }

        private void SetTilesData()
        {
            for (int i = 0; i < _objectsList.Count; i++)
            {
                DialogUIObject dialogUIObject = _objectsList[i].GetComponent<DialogUIObject>();
                
                if (dialogUIObject == null ||
                    i >= JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs.Count || 
                    JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs[i] == null)
                {
                    continue;
                }
                
                dialogUIObject.Data = JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs[i].DialogCreatorGameObject.Dialog_FR;
            }
        }

        private Dictionary<int, bool> _dialogsDictionary = new Dictionary<int, bool>();

        private void SetupDialogDictionary()
        {
            for (int i = 0; i < JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs.Count; i++)
            {
                _dialogsDictionary.Add(i,false);
            }
        }
        private void CreateDialogsUIObject()
        {
            int count = JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs.Count;
            Height = count;
            
            for (int i = 0; i < count; i++)
            {
                if (JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs[i].IsCollected == false ||
                    _dialogsDictionary[i] == true || 
                    JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs[i].DialogCreatorGameObject.Dialog_FR == null)
                {
                    continue;
                }
                
                DialogUIObject dialogUIObject = Instantiate(_dialogUIObjectPrefab, _dialogsUIObjectLayout);
                dialogUIObject.Data = JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.CollectedDialogs[i].DialogCreatorGameObject.Dialog_FR;
                _dialogsDictionary[i] = true;
                dialogUIObject.Set(false);
                _objectsList.Add(dialogUIObject);
            }

            SetTilesData();
            SetTile();
        }
    }
}