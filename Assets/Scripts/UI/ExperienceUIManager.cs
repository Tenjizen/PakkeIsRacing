using System;
using Character;
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
        
        [SerializeField] private GameObject _uIGameObject;
        [SerializeField] private Image _experienceGauge, _combatGauge, _navigationGauge;
        [SerializeField] private TMP_Text _levelText;

        private int _level;

        private void Update()
        {
            InputManagement inputManagement = CharacterManager.Instance.InputManagementProperty;
            if (inputManagement.Inputs.OpenWeaponMenu && CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons)
            {
                SetActive(true);
            }
            else
            {
                SetActive(false);
            }
        }

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
        
        public void SetActive(bool isActive)
        {
            _uIGameObject.SetActive(isActive);
        }
    }
}
