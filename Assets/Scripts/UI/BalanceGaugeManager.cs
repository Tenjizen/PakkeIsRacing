using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class BalanceGaugeManager : MonoBehaviour
    {
        [SerializeField] private GameObject _BalanceGaugeUI;
        [SerializeField] private Transform _cursor;
        [SerializeField] private float _cursorAngleMultiplier = 4;
        [SerializeField] private GameObject _LT, _RT;

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
    }
}
