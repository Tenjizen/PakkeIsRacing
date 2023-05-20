using GPEs;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Sedna
{
    public class CompanionFollowManager : MonoBehaviour
    {
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] Transform _playerTransform;
        private float _currentSplinePosition;
        private Quaternion _targetRotation;

        public float MovingValue = 0.005f;
        public float SpeedLerpToMovingValue  = 0.1f;


        private void Start()
        {

            _currentSplinePosition = 0;

            if (_splinePath == null)
            {
                return;
            }

            Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
            transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
        }

        private void Update()
        {
            ManageMovement();
        }
        private void ManageMovement()
        {
            _currentSplinePosition += MovingValue * Time.deltaTime;
            _currentSplinePosition = Mathf.Clamp01(_currentSplinePosition);


            Transform t = transform;

            Vector3 position = Vector3.Lerp(t.position, _splinePath.GetPoint(_currentSplinePosition), SpeedLerpToMovingValue * Time.deltaTime *100);
            t.position = position;

            if (_currentSplinePosition >= 1)
            {
                //_parent.SetActive(false);
                t.LookAt(_playerTransform);
            }
            else
            {
                //rotation
                Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
                Vector3 rotation = t.transform.rotation.eulerAngles;
                t.LookAt(pointB);
                t.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f * Time.deltaTime*100), rotation.z));
            }
        }
    }
}
