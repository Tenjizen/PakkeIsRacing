using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExperienceUIManager : MonoBehaviour
    {
        public enum Gauge
        {
            Experience,
            Combat,
            Navigation
        }
        
        [SerializeField] private Image _experienceGauge, _combatGauge, _navigationGauge;
        [SerializeField] private TMP_Text _levelText;

        private int _level;

        public void SetGauge(Gauge gauge, float value, float maxValue)
        {
            float percentage = value / maxValue;
            const float time = 0.5f;
            
            switch(gauge)
            {
                case Gauge.Experience:
                    _experienceGauge.DOFillAmount(percentage, time);
                    break;
                case Gauge.Combat:
                    _combatGauge.DOFillAmount(percentage, time);
                    break;
                case Gauge.Navigation:
                    _navigationGauge.DOFillAmount(percentage, time);
                    break;
            }
        }

        public void SetLevelUp()
        {
            _level++;
            _levelText.text = _level.ToString();
        }
    }
}
