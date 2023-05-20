using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIEnemyManager : MonoBehaviour
    {
        [SerializeField] private List<Image> _uiImages;
        [SerializeField] private Image _lifeGauge;
        [SerializeField] private Image _enemyIconImage;
        [ReadOnly] public bool IsActive;
    
        private void Start()
        {
            _uiImages.ForEach(x => x.DOFade(0,0));
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
            _uiImages.ForEach(x => x.DOKill());
            _uiImages.ForEach(x => x.DOFade(1,0.3f));
            _enemyIconImage.sprite = icon;

        }
        public void DisableEnemyUI()
        {
            if (IsActive == false)
            {
                return;
            }
            
            IsActive = false;
            _uiImages.ForEach(x => x.DOKill());
            _uiImages.ForEach(x => x.DOFade(0,0.15f));
        }
    }
}
