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