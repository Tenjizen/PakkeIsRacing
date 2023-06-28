using System.Collections.Generic;
using DG.Tweening;
using Dialog;
using TMPro;
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

        private List<Image> _images = new List<Image>();
        private List<TMP_Text> _texts = new List<TMP_Text>();

        private void Start()
        {
            foreach (Image image in DialogUIGameObject.GetComponentsInChildren<Image>())
            {
                _images.Add(image);
            }
            foreach (TMP_Text text in DialogUIGameObject.GetComponentsInChildren<TMP_Text>())
            {
                _texts.Add(text);
            }
            
            _images.ForEach( x=> x.DOFade(0,0));
            _texts.ForEach( x=> x.DOFade(0,0));
        }

        public void ToggleDialog(bool setActive)
        {
            const float fadeTime = 0.2f;
            _images.ForEach( x=> x.DOKill());
            _images.ForEach( x=> x.DOFade(setActive ? 1f : 0f,fadeTime));
            _texts.ForEach( x=> x.DOKill());
            _texts.ForEach( x=> x.DOFade(setActive ? 1f : 0f,fadeTime));
        }
    }
}