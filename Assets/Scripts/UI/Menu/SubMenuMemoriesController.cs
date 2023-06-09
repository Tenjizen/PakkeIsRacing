using System;
using System.Collections.Generic;
using Character;
using Json;
using TMPro;
using UI.Dialog;
using UI.Dialog.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    [Serializable]
    public struct MemoryCategory
    {
        public string CategoryID;
        public string Category_FR;
        public string Category_EN;
    }
    
    public class SubMenuMemoriesController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private TMP_Text _dialogText;
        [SerializeField] private MemoryUIObject _memoryUIObjectPrefab;
        [SerializeField] private Transform _memoriesUIObjectLayout;
        [SerializeField] private List<MemoryCategory> _memoryCategories = new List<MemoryCategory>();

        private int _index;

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

            MemoryUIObject memoryUIObject = _objectsList[_index].GetComponent<MemoryUIObject>();
            _dialogText.text = memoryUIObject == null ? _objectsList[_index].GetName() : memoryUIObject.MemoryText;
        }

        private void CreateDialogsUIObject()
        {
            foreach (Transform child in _memoriesUIObjectLayout.transform)
            {
                Destroy(child.gameObject);
            }

            _objectsList.Clear();
            Height = 0;
            
            for (int i = 0; i < _memoryCategories.Count; i++)
            {
                MemoryUIObject currentObject = null;
                
                List<CollectedDialogsData> list = JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs;
                for (int j = 0; j < list.Count; j++)
                {
                    CollectedDialogsData collectedDialogData = list[j];
                    DialogData data = CharacterManager.Instance.Parameters.Language ? 
                        collectedDialogData.DialogCreatorGameObject.Dialog_EN : 
                        collectedDialogData.DialogCreatorGameObject.Dialog_FR;

                    if (collectedDialogData.IsCollected == false || data == null ||
                        _memoryCategories[i].CategoryID != collectedDialogData.CategoryID) 
                    {
                        continue;
                    }
                 
                    //create object
                    if (currentObject == null)
                    {
                        currentObject = Instantiate(_memoryUIObjectPrefab, _memoriesUIObjectLayout);
                        MemoryCategory category = _memoryCategories.Find(x => x.CategoryID == collectedDialogData.DialogCreatorGameObject.CategoryID);
                        currentObject.SetCategoryName(category);
                        _objectsList.Add(currentObject);
                        Height++;
                    }
                    
                    DialogCreator dialog = JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs[j].DialogCreatorGameObject;
                    currentObject.AddDialogData(CharacterManager.Instance.Parameters.Language ? dialog.Dialog_EN : dialog.Dialog_FR);
                }
            }
            
            SetTile();
        }
    }
}