using System;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<Image, float> _images = new Dictionary<Image, float>();
        private Dictionary<TMP_Text, float> _texts = new Dictionary<TMP_Text, float>();

        private void Start()
        {
            _levelText.text = _level.ToString();
            _smallLevelText.text = _level.ToString();
            SetSmallUI(false);
            _cooldownUntilHideSmallUI = 0;

            foreach (Image image in _uIGameObjectSmall.GetComponentsInChildren<Image>())
            {
                _images.Add(image,image.color.a);
            }
            foreach (TMP_Text text in _uIGameObjectSmall.GetComponentsInChildren<TMP_Text>())
            {
                _texts.Add(text,text.color.a);
            }
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
            SetSmallUI(true);
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
                SetSmallUI(false);
            }
        }

        private void ManageSmallUI()
        {
            _cooldownUntilHideSmallUI -= Time.deltaTime;
            if (_cooldownUntilHideSmallUI < 0 && _uIGameObjectSmall.activeInHierarchy)
            {
                SetSmallUI(false);
            }
        }

        private void SetSmallUI(bool isActive)
        {
            _images.ToList().ForEach(x => x.Key.DOFade(isActive ? x.Value : 0, 0.4f));
            _texts.ToList().ForEach(x => x.Key.DOFade(isActive ? x.Value : 0, 0.4f));
        }
    }
}