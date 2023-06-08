using System;
using System.Collections.Generic;
using Character;
using Collectible;
using Dialog;
using Json;
using TMPro;
using UI.Dialog;
using UI.Dialog.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        [SerializeField] private Transform _dialogsUIObjectLayout;
        [SerializeField] private List<MemoryCategory> _memoryCategories = new List<MemoryCategory>();

        private int _index;

        protected override void Start()
        {
            base.Start();
            JsonFilesManagerSingleton.Instance.CollectibleJsonFileManagerProperty.OnNewCollectibleGet.AddListener(SetTilesData);
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

            MemoryUIObject memoryUIObject = _objectsList[_index].GetComponent<MemoryUIObject>();
            _dialogText.text = memoryUIObject == null ? _objectsList[_index].GetName() : memoryUIObject.MemoryText;
        }

        private void SetTilesData()
        {
            for (int i = 0; i < _objectsList.Count; i++)
            {
                MemoryUIObject memoryUIObject = _objectsList[i].GetComponent<MemoryUIObject>();
                
                if (memoryUIObject == null ||
                    i >= JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs.Count || 
                    JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs[i] == null)
                {
                    continue;
                }

                DialogCreator dialog = JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs[i].DialogCreatorGameObject;
                memoryUIObject.AddDialogData(CharacterManager.Instance.Parameters.Language ? dialog.Dialog_EN : dialog.Dialog_FR);
            }
        }
        
        private void CreateDialogsUIObject()
        {
            int count = JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs.Count;
            Height = count;
            
            for (int i = 0; i < _memoryCategories.Count; i++)
            {
                bool isCategoryCreated = false;
                MemoryUIObject currentObject = new MemoryUIObject();
                
                List<CollectedDialogsData> list = JsonFilesManagerSingleton.Instance.MemoriesJsonFileManagerProperty.CollectedDialogs;
                for (int j = 0; j < list.Count; j++)
                {
                    CollectedDialogsData collectedDialogData = list[j];
                    DialogData data = CharacterManager.Instance.Parameters.Language ? 
                        collectedDialogData.DialogCreatorGameObject.Dialog_EN : 
                        collectedDialogData.DialogCreatorGameObject.Dialog_FR;

                    if (collectedDialogData.IsCollected == false || data == null)
                    {
                        continue;
                    }
                 
                    //create object
                    if (isCategoryCreated == false)
                    {
                        isCategoryCreated = true;
                        currentObject = Instantiate(_memoryUIObjectPrefab, _dialogsUIObjectLayout);
                        MemoryCategory category = _memoryCategories.Find(x => x.CategoryID == collectedDialogData.DialogCreatorGameObject.CategoryID);
                        currentObject.SetCategoryName(category);
                    }
                    
                    currentObject.AddDialogData(data);
                }

            }

            SetTilesData();
            SetTile();
        }
    }
}