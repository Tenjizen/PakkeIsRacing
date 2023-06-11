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

        [Header("Big"), SerializeField] private GameObject _uIGameObjectBig;
        [SerializeField] private Image _experienceGauge;
        [SerializeField] private TMP_Text _levelText;
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
            _cooldownUntilHideSmallUI = 0;
        }

        private void Update()
        {
            ManageSmallUI();
        }

        public void SetMaxLevel(int level)
        {
            _maxLevel = level;
        }

        public void SetGauge(float value, float maxValue, float appearanceTime)
        {
            float percentage = value / maxValue;
            const float time = 0.5f;
            
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

            _cooldownUntilHideSmallUI = appearanceTime;
            _uIGameObjectSmall.SetActive(true);
        }

        private void SetExperienceGaugeAfterLevelUp()
        {
            _levelText.text = _level.ToString();
            _smallLevelText.text = _level.ToString();

            _uIGameObjectSmall.transform.DOComplete();
            _uIGameObjectSmall.transform.DOPunchScale(Vector3.one * 0.05f, 0.2f);
            _experienceGaugeSmall.fillAmount = 0;
            _experienceGauge.fillAmount = 0;
            SetGauge(_percentageToSetAfterLevelUp, 1f, 2f);
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