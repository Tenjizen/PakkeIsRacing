using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GPEs
{
    public class ImageFadeController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _imageToFade;
        [SerializeField, Range(0,5)] private float _fadeTime;

        public void FadeOut()
        {
            _imageToFade.DOFade(0, _fadeTime);
        }
    }
}
