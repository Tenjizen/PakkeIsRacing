using System;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WaterFlowGPE.Bezier;

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
        
        [Header("Big"),SerializeField] private GameObject _uIGameObjectBig;
        [SerializeField] private Image _experienceGauge, _combatGauge, _navigationGauge;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Transform _combatGaugeIcon, _navigationGaugeIcon;
        [SerializeField] private BezierSpline _combatIconSpline, _navigationIconSpline;
        [Header("Small"), SerializeField] private GameObject _uIGameObjectSmall;
        [SerializeField] private Image _experienceGaugeSmall;
        [SerializeField] private TMP_Text _smallLevelText;

        private int _level;
        private int _maxLevel;
        private float _percentageToSetAfterLevelUp;
        private float _cooldownUntilHideSmallUI;

        private void Start()
        {
            _levelText.text = _level.ToString();
            _smallLevelText.text = _level.ToString();
            _uIGameObjectSmall.SetActive(false);
        }

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

            ManageSmallUI();
        }
        
        public void SetMaxLevel(int level)
        {
            _maxLevel = level;
        }

        public void SetGauge(Gauge gauge, float value, float maxValue)
        {
            float percentage = value / maxValue;
            const float time = 0.5f;
            
            switch(gauge)
            {
                case Gauge.Experience:
                    if (percentage < 1 || (percentage >= 1 && _level >= _maxLevel))
                    {
                        _experienceGauge.DOFillAmount(percentage, time);
                        _experienceGaugeSmall.DOFillAmount(percentage, time);
                    }
                    else
                    {
                        _percentageToSetAfterLevelUp = percentage - 1;
                        _level++;
                        _experienceGauge.DOFillAmount(1, time).OnComplete(SetExperienceGaugeAfterLevelUp);
                        _experienceGaugeSmall.DOFillAmount(1, time).OnComplete(SetExperienceGaugeAfterLevelUp);
                    }
                    _cooldownUntilHideSmallUI = 2f;
                    _uIGameObjectSmall.SetActive(true);
                    break;
                case Gauge.Combat:
                    _combatGauge.DOFillAmount(percentage, time);
                    _combatGaugeIcon.position = _combatIconSpline.GetPoint(percentage);
                    break;
                case Gauge.Navigation:
                    _navigationGauge.DOFillAmount(percentage, time);
                    _navigationGaugeIcon.position = _navigationIconSpline.GetPoint(percentage);
                    break;
            }
        }

        private void SetExperienceGaugeAfterLevelUp()
        {
            _levelText.text = _level.ToString();
            _smallLevelText.text = _level.ToString();
            
            _uIGameObjectSmall.transform.DOPunchScale(Vector3.one * 0.05f, 0.2f);
            _experienceGaugeSmall.fillAmount = 0;
            _experienceGauge.fillAmount = 0;
            SetGauge(Gauge.Experience,_percentageToSetAfterLevelUp,1f);
        }

        public void SetActive(bool isActive)
        {
            _uIGameObjectBig.SetActive(isActive);
            
            if (_uIGameObjectSmall.activeInHierarchy && isActive)
            {
                _uIGameObjectSmall.SetActive(false);
            }
        }

        private void ManageSmallUI()
        {
            _cooldownUntilHideSmallUI -= Time.deltaTime;
            if (_cooldownUntilHideSmallUI < 0 && _uIGameObjectSmall.activeInHierarchy)
            {
                _uIGameObjectSmall.SetActive(false);
            }
        }
    }
}
