using System;
using System.Collections.Generic;
using Enemies.Data;
using Fight;
using GPEs;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Enemies.Seal
{
    public class SealManager : Enemy
    {
        [Header("Player detection"), SerializeField] private PlayerTriggerManager _playerTrigger;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField, Range(0,1)] private List<float> _sealCheckpoints;
        [Header("Data"), SerializeField] private SealData _data;

        private float _currentSplinePosition;
        private int _checkpointsIndex;
        
        private void Start()
        {
            _currentSplinePosition = 0;
            _checkpointsIndex = 0;

            if (_playerTrigger == null)
            {
                return;
            }
            
            _playerTrigger.OnPlayerEntered.AddListener(MoveToNextCheckpoint);

            if (_splinePath == null)
            {
                return;
            }

            _sealCheckpoints.Add(0);
            _sealCheckpoints.Sort();
            
            Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
            transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
        }

        private void Update()
        {
            HandleMovement();
        }

        #region Seal Controller

        private void MoveToNextCheckpoint()
        {
            if (_checkpointsIndex >= _sealCheckpoints.Count - 1)
            {
                return;
            }
            _checkpointsIndex++;
        }

        private void HandleMovement()
        {
            if (_currentSplinePosition >= _sealCheckpoints[_checkpointsIndex])
            {
                return;
            }

            _currentSplinePosition += _data.MovingSpeed;
            
            Transform t = transform;

            //position
            Vector3 point = _splinePath.GetPoint(_currentSplinePosition);
            t.position = Vector3.Lerp(t.position, new Vector3(point.x, t.position.y,point.z), _data.SealSpeedLerpToMovingValue);

            //rotation
            Vector3 rotation = t.rotation.eulerAngles;
            Vector3 direction = _splinePath.GetDirection(_currentSplinePosition);
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            t.rotation = Quaternion.Euler(rotation.x,angle, rotation.z);
        }
        
        #endregion

        public override void Hit(Projectile projectile, GameObject owner)
        {
            
        }
        
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < _sealCheckpoints.Count; i++)
            {
                Gizmos.DrawCube(_splinePath.GetPoint(_sealCheckpoints[i]), Vector3.one);
            }
        }

#endif
    }
}
