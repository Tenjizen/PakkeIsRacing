using System;
using System.Collections.Generic;
using Character;
using Collectible.Data;
using Dialog;
using TMPro;
using UI.Dialog.Data;
using UI.Quest;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Menu
{
    public class QuestUIObject : MenuUIObject
    {
        public QuestData Data;
        public TMP_Text QuestTitleText { get; private set; }
        public Image QuestLogo { get; private set; }

        public string GetTitle()
        {
            return CharacterManager.Instance.Parameters.Language ? Data.QuestTitle_EN : Data.QuestTitle_FR;
        }

        public string GetDescription()
        {
            return CharacterManager.Instance.Parameters.Language ? Data.QuestDescription_EN : Data.QuestDescription_FR;
        }

        public override void Set(bool isActive)
        {
            base.Set(isActive);

            if (Data == null)
            {
                return;
            }

            QuestTitleText.text = GetTitle();
            QuestLogo.sprite = Data.QuestLogo;
        }
    }
}