using System;
using System.Collections.Generic;
using System.Linq;
using Enemies.Data;
using Fight;
using GPEs;
using Tools.HideIf;
using UnityEditor;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Enemies.Seal
{
    [Serializable]
    public enum PointType
    {
        Speed = 0,
        Stop = 1
    }
    
    [Serializable]
    public struct ControlPoint
    {
        [Range(0, 1)] public float Position;
        public PointType Type;

        [Range(0.25f, 4f)] public float SpeedMultiplier;
        [Range(0, 30f)] public float StopTime;
    }
    
    public class SealManager : Enemy
    {
        [Header("Player detection"), SerializeField] private PlayerTriggerManager _playerTrigger;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] private List<ControlPoint> _sealCheckpoints;
        [Header("Data"), SerializeField] private SealData _data;

        private float _currentSplinePosition;
        private int _checkpointsIndex;
        private bool _isMoving;
        private float _currentStopTimer;
        private float _currentSpeedMultiplier;
        
        private void Start()
        {
            _currentSplinePosition = 0;
            _checkpointsIndex = 0;
            _currentSpeedMultiplier = 1;

            if (_playerTrigger == null)
            {
                return;
            }
            
            _playerTrigger.OnPlayerEntered.AddListener(LaunchMovement);

            if (_splinePath == null)
            {
                return;
            }

            List<ControlPoint> controlPoints = _sealCheckpoints.OrderBy(x => x.Position).ToList();
            _sealCheckpoints = controlPoints;

            Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
            transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);

            CurrentLife = _data.Life;
        }

        private void Update()
        {
            if (_isMoving == false)
            {
                return;
            }

            CheckForCheckPoint();
            
            if (_currentStopTimer > 0)
            {
                HandleStop();
                return;
            }

            if (_currentSplinePosition <= 1 - _data.MovingSpeed * _currentSpeedMultiplier)
            {
                HandleMovement();
                return;
            }
        }

        #region Seal Controller

        private void LaunchMovement()
        {
            _isMoving = true;
        }

        private void CheckForCheckPoint()
        {
            if (_checkpointsIndex >= _sealCheckpoints.Count - 1)
            {
                return;
            }
            
            ControlPoint controlPoint = _sealCheckpoints[_checkpointsIndex];
            if (_currentSplinePosition < controlPoint.Position)
            {
                return;
            }

            switch (controlPoint.Type)
            {
                case PointType.Speed:
                    _currentSpeedMultiplier = controlPoint.SpeedMultiplier;
                    break;
                case PointType.Stop:
                    _currentStopTimer = controlPoint.StopTime;
                    break;
            }

            _checkpointsIndex++;
        }
        
        private void HandleStop()
        {
            _currentStopTimer -= Time.deltaTime;
            
            //stop behavior
            transform.Rotate(Vector3.up,1f);
        }

        private void HandleMovement()
        {
            _currentSplinePosition += _data.MovingSpeed * _currentSpeedMultiplier;
            
            Transform t = transform;

            //position
            Vector3 point = _splinePath.GetPoint(_currentSplinePosition);
            t.position = point;

            //rotation
            Vector3 rotation = t.rotation.eulerAngles;
            Vector3 direction = _splinePath.GetDirection(_currentSplinePosition);
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            t.rotation = Quaternion.Euler(rotation.x,angle, rotation.z);
        }
        
        #endregion

        public override void Hit(Projectile projectile, GameObject owner)
        {
            base.Hit(projectile,owner);
            
            if (CurrentLife <= 0)
            {
                //DIE
            }
        }
        
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            for (int i = 0; i < _sealCheckpoints.Count; i++)
            {
                Vector3 position = _splinePath.GetPoint(_sealCheckpoints[i].Position);
                Gizmos.color = _sealCheckpoints[i].Type == PointType.Speed ? new Color(0.53f, 1f, 0.37f) : new Color(0.69f, 0.33f, 0.28f);
                Gizmos.DrawCube(position, Vector3.one);
                Handles.Label(position + Vector3.up * 2,
                    _sealCheckpoints[i].Type == PointType.Speed ? $"{_sealCheckpoints[i].SpeedMultiplier}" : $"{_sealCheckpoints[i].StopTime}");
            }
        }

#endif
    }
}
