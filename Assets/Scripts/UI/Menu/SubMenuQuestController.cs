using System;
using System.Collections.Generic;
using Character;
using Json;
using TMPro;
using UI.Dialog;
using UI.Dialog.Data;
using UI.Quest;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class SubMenuQuestController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private TMP_Text _questTitleText, _questDescriptionText;
        [SerializeField] private Image _questLogoImage;
        [SerializeField] private Transform _questUIObjectLayout;
        [SerializeField] private QuestUIObject _questUIObjectPrefab;

        private int _index;

        public override void SetMenu(bool isActive, bool isUsable)
        {
            base.SetMenu(isActive, isUsable);

            CreateQuestsUIObject();

            if (isUsable == false)
            {
                return;
            }

            SetTile();
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
            if (_objectsList.Count == 0)
            {
                _questLogoImage.gameObject.SetActive(false);
                _questTitleText.text = CharacterManager.Instance.Parameters.Language
                    ? "No quest available for the moment"
                    : "Pas de quête pour le moment";
                _questDescriptionText.text = String.Empty;
                return;
            }

            for (int i = 0; i < _objectsList.Count; i++)
            {
                _objectsList[i].Set(i == _index);
            }

            if (_questTitleText == null || _questLogoImage == null || _questDescriptionText == null)
            {
                return;
            }

            QuestUIObject questUIObject = _objectsList[_index].GetComponent<QuestUIObject>();
            _questTitleText.text = questUIObject == null ? String.Empty : questUIObject.GetTitle();
            _questDescriptionText.text = questUIObject == null ? String.Empty : questUIObject.GetDescription();
            _questLogoImage.gameObject.SetActive(true);
            _questLogoImage.sprite = questUIObject.Data.QuestLogo;
        }

        private void CreateQuestsUIObject()
        {
            foreach (Transform child in _questUIObjectLayout.transform)
            {
                Destroy(child.gameObject);
            }

            _objectsList.Clear();
            Height = 0;

            List<CollectedQuestData> list = JsonFilesManagerSingleton.Instance.QuestJsonFileManagerProperty.CollectedQuests;
            for (int i = 0; i < list.Count; i++)
            {
                CollectedQuestData collectedQuestData = list[i];
                QuestData data = collectedQuestData.QuestCreatorGameObject.QuestCreatorData;

                if (collectedQuestData.IsCollected == false || data == null)
                {
                    continue;
                }

                QuestUIObject questUIObject = Instantiate(_questUIObjectPrefab, _questUIObjectLayout);
                questUIObject.Data = list[i].QuestCreatorGameObject.QuestCreatorData;
                
                _objectsList.Add(questUIObject);
                Height++;
            }

            SetTile();
        }
    }
}