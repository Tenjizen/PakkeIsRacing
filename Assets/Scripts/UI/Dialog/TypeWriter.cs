using System.Collections;
using TMPro;
using UnityEngine;

namespace Dialog
{
    public class TypeWriter : MonoBehaviour
    {
        [field:SerializeField] public TMP_Text DisplayText { get; set; }
        
        public float Delay = 0.1f;
        public string FullText;
        
        private string _currentText = string.Empty;

        public IEnumerator ShowText()
        {
            for (int i = 0; i <= FullText.Length; i++)
            {
                _currentText = FullText.Substring(0, i);
                DisplayText.text = _currentText;
                yield return new WaitForSeconds(Delay);
            }
        }
    }
}