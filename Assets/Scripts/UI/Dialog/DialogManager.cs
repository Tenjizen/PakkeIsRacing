using System.Collections.Generic;
using DG.Tweening;
using Dialog;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialog
{
    public class DialogManager : Singleton<DialogManager>
    {
        [field:SerializeField] public TypeWriter TypeWriterText { get; set; }
        [field:SerializeField] public GameObject DialogUIGameObject { get; set; }
        [field:SerializeField] public Image PressButtonImage { get; set; }

        public Queue<DialogCreator> DialogQueue { get; set; } = new Queue<DialogCreator>();

        private void Start()
        {
            DialogUIGameObject.SetActive(false);
        }

        public void ToggleDialog(bool setActive)
        {
            DialogUIGameObject.transform.DOKill();
            DialogUIGameObject.SetActive(setActive);
        }
    }
}