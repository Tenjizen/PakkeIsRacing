using System;
using Collectible.Data;
using Dialog;
using TMPro;
using UnityEngine;

namespace UI.Menu
{
    public class DialogUIObject : MenuUIObject
    {
        public DialogData Data;
        public TMP_Text DialogTitleText;

        public override void Set(bool isActive)
        {
            base.Set(isActive);
            if (Data == null)
            {
                return;
            }
            DialogTitleText.text = Data.Title;
        }
    }
}