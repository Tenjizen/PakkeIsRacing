using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuUIObject : MonoBehaviour
    {
        [SerializeField] protected Image OverlayImage;
        [SerializeField] protected Image IconImage;

        public void Initialize(Sprite image)
        {
            IconImage.sprite = image;
        }
        
        public virtual void Set(bool isActive)
        {
            OverlayImage.DOKill();
            OverlayImage.DOFade(isActive ? 1f : 0f, 0.1f);
        }

        public virtual string GetName()
        {
            return string.Empty;
        }

        public virtual string GetDescription()
        {
            return string.Empty;
        }
    }
}