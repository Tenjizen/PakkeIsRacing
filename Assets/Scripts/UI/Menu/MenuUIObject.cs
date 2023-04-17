using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuUIObject : MonoBehaviour
    {
        [SerializeField] private Image _overlayImage;
        [SerializeField] private Image _iconImage;

        public void Initialize(Sprite image)
        {
            _iconImage.sprite = image;
        }
        
        public virtual void Set(bool isActive)
        {
            _overlayImage.DOFade(isActive ? 1f : 0f, 0.1f);
        }
    }
}