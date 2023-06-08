using System;
using System.Collections.Generic;
using Character;
using Collectible.Data;
using Dialog;
using TMPro;
using UI.Dialog.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Menu
{
    public class MemoryUIObject : MenuUIObject
    {
        public HashSet<DialogData> Datas = new HashSet<DialogData>();
        public TMP_Text MemoryTitleText;
        public string MemoryText { get; private set; }

        private string _categoryTitle;

        public void AddDialogData(DialogData data)
        {
            Datas.Add(data);
            MemoryText = string.Empty;
            foreach (var dialogData in Datas)
            {
                MemoryText += $"\n \n {dialogData.Summary}";
            }
        }
        
        public void SetCategoryName(MemoryCategory category)
        {
            _categoryTitle = CharacterManager.Instance.Parameters.Language ? category.Category_EN : category.Category_FR;
        }
        
        public override void Set(bool isActive)
        {
            base.Set(isActive);

            MemoryTitleText.text = _categoryTitle;
            MemoryTitleText.fontSize = isActive ? 40 : 32;
        }
    }
}