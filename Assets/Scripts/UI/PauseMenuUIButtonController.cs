using TMPro;
using UnityEngine;

namespace UI
{
    public class PauseMenuUIButtonController : MonoBehaviour
    {
        [SerializeField] private Color _selectedColor, _unselectedColor;
        [SerializeField] private UnityEngine.UI.Image _image;
        [SerializeField] private TMP_Text _text;
        
        public void Set(bool isSelected)
        {
            _image.color = isSelected ? _selectedColor : _unselectedColor;
            _text.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
