using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEnemyManager : MonoBehaviour
    {
        [SerializeField] private List<Image> _uiImages;
        [SerializeField] private RectTransform _gauge;
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Image _lifeGauge;
        [SerializeField] private Image _enemyIconImage;
        [ReadOnly] public bool IsActive;

        private Dictionary<Image, float> _imagesDictionary = new Dictionary<Image, float>();

        private void Start()
        {
            Image[] array = _gauge.GetComponentsInChildren<Image>();
            foreach (Image image in array)
            {
                _imagesDictionary.Add(image, image.color.a);
                image.DOFade(0, 0);
            }
        }

        public void SetScreenPositionFromEnemyPosition(Vector3 enemyPosition)
        {
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(enemyPosition);
            
            if (viewportPosition.x < 0 || viewportPosition.x > 1 ||
                viewportPosition.y < 0 || viewportPosition.y > 1 ||
                viewportPosition.z < 0)
            {
                DisableEnemyUI();
            }
            
            Vector2 canvasPosition = new Vector2(
                (viewportPosition.x - 0.5f) * _canvas.sizeDelta.x,
                (viewportPosition.y - 0.5f) * _canvas.sizeDelta.y
            );
            _gauge.anchoredPosition = canvasPosition + _offset;
        }

        public void SetGauge(float life, float maxLife)
        {
            float percent = life / maxLife;
            _lifeGauge.DOFillAmount(percent, 0.4f);
        }

        public void ActiveEnemyUI(Sprite icon)
        {
            if (IsActive)
            {
                return;
            }
            
            IsActive = true;
            foreach (var image in _imagesDictionary)
            {
                image.Key.DOKill();
                image.Key.DOFade(image.Value,0.3f);
            }
            _enemyIconImage.sprite = icon;

        }
        public void DisableEnemyUI()
        {
            if (IsActive == false)
            {
                return;
            }
            
            IsActive = false;
            foreach (var image in _imagesDictionary)
            {
                image.Key.DOKill();
                image.Key.DOFade(0,0.3f);
            }
        }
    }
}
