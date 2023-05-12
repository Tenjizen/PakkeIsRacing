using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WeaponWheel
{
    public class AutoAimUIController : MonoBehaviour
    {
        [SerializeField] private Image _autoAimImage, _autoAimCircle;
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private float _baseRotationSpeed;
        [SerializeField] private float _lockedRotationSpeed;
        [SerializeField] private Color _baseColor, _endColor, _lockedColor;
        [SerializeField] private float _lockedFeedbackAmplitude;

        private void Start()
        {
            ShowAutoAimUI(false);
            ShowAutoAimCircle(false);
        }

        public void ShowAutoAimUI(bool show)
        {
            _autoAimImage.DOFade(show ? 1f : 0f, 0.2f);
        }

        public void ShowAutoAimCircle(bool show)
        {
            _autoAimCircle.DOFade(show ? 1f : 0f, 0.2f);
        }
        
        public void SetAutoAimUI(float percentage, Vector3 aimPosition)
        {
            transform.Rotate(new Vector3(0, 0, 1), percentage >= 1 ? _lockedRotationSpeed : _baseRotationSpeed);

            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(aimPosition);
            Vector2 canvasPosition = new Vector2(
                (viewportPosition.x - 0.5f) * _canvas.sizeDelta.x,
                (viewportPosition.y - 0.5f) * _canvas.sizeDelta.y
            );
            _autoAimImage.rectTransform.anchoredPosition = canvasPosition;

            if (percentage >= 1)
            {
                if (_autoAimImage.color != _lockedColor)
                { 
                    _autoAimImage.transform.DOPunchScale(Vector3.one * _lockedFeedbackAmplitude, 0.3f);
                }
                _autoAimImage.color = _lockedColor;
                return;
            }
            
            _autoAimImage.color = Color.Lerp(_baseColor, _endColor, percentage);
        }
    }
}