using System;
using System.Collections.Generic;
using TMPro;
using UI.Menu;
using UnityEngine;

namespace UI
{
    [Serializable]
    public struct TextLanguage
    {
        public TMP_Text Text;
        public string TextFr, TextEn;
    }

    public enum Language
    {
        Francais,
        English
    }
    
    public class LanguageManager : MonoBehaviour
    {
        [SerializeField] private List<ParametersUIObject> _languageParameterObjects = new List<ParametersUIObject>();
        [SerializeField] private List<TextLanguage> _textsToChange = new List<TextLanguage>();

        private void Start()
        {
            foreach (ParametersUIObject parameter in _languageParameterObjects)
            {
                parameter.OnOn.AddListener(SetTextsEn);
                parameter.OnOff.AddListener(SetTextsFr);
            }
        }

        private void SetTextsFr()
        {
            SetTexts(Language.Francais);
        }
        
        private void SetTextsEn()
        {
            SetTexts(Language.English);
        }
        
        private void SetTexts(Language language)
        {
            foreach (TextLanguage text in _textsToChange)
            {
                text.Text.text = language == Language.Francais ? text.TextFr : text.TextEn;
            }
        }
    }
}
