using GPEs;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Enemies.Seal
{
    public class SealController : MonoBehaviour
    {
        [Header("Player detection"), SerializeField] private PlayerTriggerManager _frontPlayerTrigger;
        [SerializeField] private PlayerTriggerManager _backPlayerTrigger;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;

        private float _currentSplinePosition;

        [Header("Debug"), SerializeField, ReadOnly] private bool _playerIsAtBack;
        [SerializeField, ReadOnly] private bool _playerIsAtFront;

        private void Start()
        {
            _currentSplinePosition = 0;
            _playerIsAtFront = false;
            _playerIsAtBack = false;

            if (_frontPlayerTrigger == null || _backPlayerTrigger == null)
            {
                return;
            }
            
            _frontPlayerTrigger.OnPlayerEntered.AddListener(SetPlayerAtFrontTrue);
            _backPlayerTrigger.OnPlayerEntered.AddListener(SetPlayerAtBackTrue);
            _frontPlayerTrigger.OnPlayerExited.AddListener(SetPlayerAtFrontFalse);
            _backPlayerTrigger.OnPlayerExited.AddListener(SetPlayerAtBackFalse);

            if (_splinePath == null)
            {
                return;
            }

            Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
            transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
        }

        #region Player Detection Methods

        private void SetPlayerAtFrontTrue()
        {
            _playerIsAtFront = true;
        }
        private void SetPlayerAtBackTrue()
        {
            _playerIsAtBack = true;
        }
        private void SetPlayerAtFrontFalse()
        {
            Debug.Log("front exited");
            _playerIsAtFront = false;
        }
        private void SetPlayerAtBackFalse()
        {
            Debug.Log("back exited");
            _playerIsAtBack = false;
        }

        #endregion

        #region Seal Controller

        

        #endregion
    }
}
