using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BalanceGaugeManager : MonoBehaviour
    {
        [SerializeField] private GameObject _BalanceGaugeUI;
        [SerializeField] private Transform _cursor;
        public Transform Cursor => _cursor;
        [SerializeField] private float _cursorAngleMultiplier = 4;
        [SerializeField] private GameObject _LT, _RT;

        [SerializeField] private Image GaugeLeft, GaugeRight;

        private float _currentAngle;

        public void SetBalanceGaugeActive(bool isActive)
        {
            _BalanceGaugeUI.SetActive(isActive);
        }

        public void SetBalanceCursor(float angle)
        {
            angle *= _cursorAngleMultiplier;
            Vector3 rotation = _cursor.transform.rotation.eulerAngles;
            _currentAngle = Mathf.Lerp(_currentAngle, angle, 0.1f);
            _cursor.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, _currentAngle);
        }

        public void MakeCursorFeedback()
        {
            _cursor.DOComplete();
            _cursor.DOPunchScale(Vector3.one * 0.1f, 0.4f);
        }

        public void ShowTrigger(bool showLT, bool showRT)
        {
            _LT.SetActive(showLT);
            _RT.SetActive(showRT);
        }

        public void ReduceGauge(float timer)
        {
            GaugeLeft.fillAmount = 0.25f- timer;
            GaugeRight.fillAmount = 0.25f - timer;
        }
        public void ResetGauge()
        {
            GaugeLeft.fillAmount = 0.25f;
            GaugeRight.fillAmount = 0.25f;
        }

        public float PercentGauge()
        {
            return GaugeLeft.fillAmount / .25f;
        }

    }
}
