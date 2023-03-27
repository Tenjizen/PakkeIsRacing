using System;
using TMPro;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class DialogManager : Singleton<DialogManager>
    {

        public TypeWriter TypeWriterText;
        public GameObject DialogUIGameObject;
        public Image PressButtonImage;

        private void Start()
        {
            DialogUIGameObject.SetActive(false);
        }

        public void ToggleDialog(bool setActive)
        {
            DialogUIGameObject.SetActive(setActive);
        }
    }
}