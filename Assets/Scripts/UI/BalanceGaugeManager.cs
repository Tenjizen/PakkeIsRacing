using UnityEngine;

namespace UI
{
    public class BalanceGaugeManager : MonoBehaviour
    {
        [SerializeField] private GameObject _BalanceGaugeUI;
        [SerializeField] private Transform _cursor;
        [SerializeField] private float _cursorAngleMultiplier = 4;

        private float _currentAngle;

        public void SetBalanceGaugeActive(bool isActive)
        {
            _BalanceGaugeUI.SetActive(isActive);
        }

        public void SetBalanceCursor(float angle)
        {
            Debug.Log(angle);
            angle *= _cursorAngleMultiplier;
            Debug.Log(angle);
            Vector3 rotation = _cursor.transform.rotation.eulerAngles;
            _currentAngle = Mathf.Lerp(_currentAngle, angle, 0.1f);
            _cursor.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, _currentAngle);
        }
    }
}
