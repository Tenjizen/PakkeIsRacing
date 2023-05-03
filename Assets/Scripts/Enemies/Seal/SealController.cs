using System;
using Enemies.Data;
using Fight;
using GPEs;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Enemies.Seal
{
    public enum MovingDirection
    {
        Forward = 0,
        Backward = 1
    }
    
    public class SealController : MonoBehaviour, IHittable
    {
        [Header("Player detection"), SerializeField] private PlayerTriggerManager _frontPlayerTrigger;
        [SerializeField] private PlayerTriggerManager _backPlayerTrigger;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [Header("Data"), SerializeField] private SealData _data;

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

        private void Update()
        {
            ManageMovement();
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

        private void ManageMovement()
        {
            if (_playerIsAtBack)
            {
                Move(MovingDirection.Forward);
            }

            if (_playerIsAtFront)
            {
                Move(MovingDirection.Backward);
            }

            Transform t = transform;
            
            Vector3 position = Vector3.Lerp(t.position, _splinePath.GetPoint(_currentSplinePosition), _data.SealSpeedLerpToMovingValue);
            t.position = position;

            Vector3 rotation = t.rotation.eulerAngles;
            //TODO convert direction to rotation z
            //Debug.Log(_splinePath.GetDirection(_currentSplinePosition));
            t.rotation = Quaternion.Euler(rotation.x,_splinePath.GetDirection(_currentSplinePosition).y, rotation.z);
        }
        
        private void Move(MovingDirection direction)
        {
            float value = direction == MovingDirection.Forward ? 1 : -1;
            _currentSplinePosition += value * _data.MovingValueAtPlayerDetected;
        }

        #endregion

        public void Hit(Projectile projectile, GameObject owner)
        {
            
        }
    }
}
