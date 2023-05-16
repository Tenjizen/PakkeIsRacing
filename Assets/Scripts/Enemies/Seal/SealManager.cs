using System;
using System.Collections.Generic;
using System.Linq;
using Enemies.Data;
using Fight;
using GPEs;
using Tools.HideIf;
using UnityEditor;
using UnityEngine;
using WaterAndFloating;
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
        [Header("References"), SerializeField] private Waves _waves;
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

            _sealCheckpoints.Add(new ControlPoint(){Position = 1f, SpeedMultiplier = 1f, Type = PointType.Speed});
            _sealCheckpoints.Add(new ControlPoint(){Position = 0f, SpeedMultiplier = 1f, Type = PointType.Speed});
            List<ControlPoint> controlPoints = _sealCheckpoints.OrderBy(x => x.Position).ToList();
            _sealCheckpoints = controlPoints;

            Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
            transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);

            CurrentLife = _data.Life;
            MaxLife = CurrentLife;
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
            SetUpStartEnemyUI();
        }

        private void CheckForCheckPoint()
        {
            if (_checkpointsIndex >= _sealCheckpoints.Count-1)
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
            
            if (_checkpointsIndex < _sealCheckpoints.Count-1)
            {
                _checkpointsIndex++;
            }
        }
        
        private void HandleStop()
        {
            _currentStopTimer -= Time.deltaTime;
            
            Vector3 center = _splinePath.GetPoint(_sealCheckpoints[_checkpointsIndex-1].Position);
            float angle = Time.time * 1000 * _data.MovingSpeed * _currentSpeedMultiplier;
            Vector3 targetPosition = new Vector3(
                center.x + Mathf.Sin(angle) * _data.StopMovementRadius,
                transform.position.y,
                center.z + Mathf.Cos(angle) * _data.StopMovementRadius
            );
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
            
            float angleNext = (Time.time + 0.01f) * 1000 * _data.MovingSpeed * _currentSpeedMultiplier;
            Vector3 targetPositionNext = new Vector3(
                center.x + Mathf.Sin(angleNext) * _data.StopMovementRadius,
                transform.position.y,
                center.z + Mathf.Cos(angleNext) * _data.StopMovementRadius
            );
            //rotation
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.LookAt(targetPositionNext);
            transform.rotation = Quaternion.Euler(new Vector3(rotation.x,transform.rotation.eulerAngles.y,rotation.z));
        }

        private void HandleMovement()
        {
            _currentSplinePosition += _data.MovingSpeed * _currentSpeedMultiplier;
            
            Transform t = transform;

            //position
            Vector3 pointA = _splinePath.GetPoint(_currentSplinePosition);
            Vector3 positionTarget = new Vector3(pointA.x, 
                _waves.GetHeight(t.position) + _data.UpAndDownMovements.Evaluate(_currentSplinePosition) * _data.UpAndDownMultiplier,
                pointA.z);
            transform.position = Vector3.Lerp(transform.position,positionTarget,0.5f);

            //rotation
            Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition+0.01f));
            Vector3 rotation = t.transform.rotation.eulerAngles;
            t.LookAt(pointB);
            t.transform.rotation = Quaternion.Euler(new Vector3(rotation.x,t.rotation.eulerAngles.y,rotation.z));
        }
        
        #endregion

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
                if (_sealCheckpoints[i].Type == PointType.Stop)
                {
                    Handles.color = new Color(0.69f, 0.33f, 0.28f, 0.43f);
                    Handles.DrawSolidDisc(position,Vector3.up, _data.StopMovementRadius);
                }
            }
        }

#endif
    }
}
