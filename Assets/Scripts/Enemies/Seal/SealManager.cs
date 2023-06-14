using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Enemies.Data;
using Fight;
using GPEs;
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
    public enum StopType
    {
        Time = 0,
        Trigger = 1
    }
    
    [Serializable]
    public struct ControlPoint
    {
        [Range(0, 1)] public float Position;
        public PointType Type;
        public StopType StopType;

        [Range(0f, 4f)] public float SpeedMultiplier;
        [Range(0, 60f)] public float StopTime;
    }
    
    public class SealManager : Enemy
    {
        [Header("References"), SerializeField] private Waves _waves;
        [Header("Player detection"), SerializeField] private PlayerTriggerManager _playerTrigger;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] private List<ControlPoint> _sealCheckpoints;
        [Header("Data"), SerializeField] private SealData _data;
        [field: SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }


        private float _currentSplinePosition;
        private int _checkpointsIndex;
        private bool _isMoving;
        private bool _isWaitingForTrigger;
        private bool _launchedWave;
        private float _currentStopTimer;
        private float _speedMultiplier;
        private float _currentSpeedMultiplier;
        private Quaternion _targetRotation;
        private Transform _player;
        
        private void Start()
        {
            _currentSplinePosition = 0;
            _checkpointsIndex = 0;
            _currentSpeedMultiplier = 1;
            _speedMultiplier = 1;

            if (_playerTrigger == null)
            {
                return;
            }
            
            _playerTrigger.OnPlayerEntered.AddListener(LaunchMovement);
            _playerTrigger.OnPlayerEntered.AddListener(TriggerEnter);

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
            if (Time.deltaTime == 0)
            {
                return;
            }
            
            if (_isMoving == false)
            {
                RotateAroundPoint(_splinePath.GetPoint(_sealCheckpoints[_player == null ? 0 : _checkpointsIndex].Position));
                return;
            }

            HandlePlayerDistanceToSetUI(_player, _data.DistanceAtWhichPlayerIsNotInCombat);

            CheckForCheckPoint();
            _speedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, _speedMultiplier, 0.1f);
            
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
            
            RotateAroundPoint(_splinePath.GetPoint(_sealCheckpoints[^1].Position));
        }

        #region Seal Controller

        private void TriggerEnter()
        {
            _isWaitingForTrigger = false;
        }
        
        private void LaunchMovement()
        {
            if (_isMoving || CurrentLife <= 0)
            {
                return;
            }
            
            _isMoving = true;
            _player = CharacterManager.Instance.transform;
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
                    _speedMultiplier = controlPoint.SpeedMultiplier;
                    break;
                case PointType.Stop:
                    _currentStopTimer = controlPoint.StopTime;
                    if (controlPoint.StopType == StopType.Trigger)
                    {
                        _isWaitingForTrigger = true;
                        _currentStopTimer = 1f;
                    }
                    break;
            }
            
            if (_checkpointsIndex < _sealCheckpoints.Count-1)
            {
                _checkpointsIndex++;
            }
        }
        
        private void HandleStop()
        {
            if (_isWaitingForTrigger == false)
            {
                _currentStopTimer -= Time.deltaTime;
            }
            
            Vector3 center = _splinePath.GetPoint(_sealCheckpoints[_checkpointsIndex-1].Position);
            RotateAroundPoint(center);
        }

        private void HandleMovement()
        {
            _currentSplinePosition += _data.MovingSpeed * _currentSpeedMultiplier;
            
            Transform t = transform;

            //position
            float positionY = _data.UpAndDownMovements.Evaluate(_currentSplinePosition) + _data.WaveHeightOffset;
            Vector3 pointA = _splinePath.GetPoint(_currentSplinePosition);
            Vector3 positionTarget = new Vector3(pointA.x, 
                _waves.GetHeight(t.position) + positionY * _data.UpAndDownMultiplier,
                pointA.z);
            transform.position = Vector3.Lerp(transform.position,positionTarget,0.05f);

            //rotation
            Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition+0.01f));
            Vector3 rotation = t.transform.rotation.eulerAngles;
            t.LookAt(pointB);
            t.rotation = Quaternion.Euler(new Vector3(rotation.x,Mathf.Lerp(rotation.y,t.rotation.eulerAngles.y,0.1f),rotation.z));
            
            //wave
            if (positionY < 0.1f && positionY > -0.1f)
            {
                if (_launchedWave)
                {
                    return;
                }
                CircularWave wave = _data.Wave;
                wave.Center = new Vector2(t.position.x,t.position.z);
                _waves.LaunchCircularWave(wave);
                _launchedWave = true;
            }
            else
            {
                _launchedWave = false;
            }
        }

        private void RotateAroundPoint(Vector3 point)
        {
            Vector3 position = transform.position;
            
            float angle = Time.time * 1000 * _data.MovingSpeed * _currentSpeedMultiplier;
            Vector3 targetPosition = new Vector3(
                point.x + Mathf.Sin(angle) * _data.StopMovementRadius,
                _waves.GetHeight(transform.position) + _data.WaveHeightOffset,
                point.z + Mathf.Cos(angle) * _data.StopMovementRadius
            );
            position = Vector3.Lerp(position, targetPosition, 0.05f);
            transform.position = position;

            float angleNext = (Time.time + 0.01f) * 1000 * _data.MovingSpeed * _currentSpeedMultiplier;
            Vector3 targetPositionNext = new Vector3(
                point.x + Mathf.Sin(angleNext) * _data.StopMovementRadius,
                _waves.GetHeight(position) + _data.WaveHeightOffset,
                point.z + Mathf.Cos(angleNext) * _data.StopMovementRadius
            );
            
            //rotation
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.LookAt(targetPositionNext);
            _targetRotation = Quaternion.Euler(new Vector3(rotation.x,transform.rotation.eulerAngles.y,rotation.z));
            transform.rotation = Quaternion.Lerp(transform.rotation,_targetRotation,0.03f);
        }
        
        #endregion

        public override void Hit(Projectile projectile, GameObject owner, int damage)
        {
            base.Hit(projectile, owner, damage);
            
            if (projectile.Data.Type != WeaponThatCanHitEnemy || CurrentLife <= 0)
            {
                return;
            }

            HitParticles.Play();
        }

        protected override void Die()
        {
            base.Die();
            
            HitParticles.Play();
            _isMoving = false;
            
            SetPlayerExperience(CharacterManager.Instance.ExperienceManagerProperty.Data.ExperienceGainedAtEnemySeal);
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
                if (_sealCheckpoints[i].Type == PointType.Stop)
                {
                    Handles.color = _sealCheckpoints[i].StopType == StopType.Time ? new Color(0.69f, 0.33f, 0.28f, 0.43f) : new Color(0.55f, 1f, 0.4f, 0.24f);
                    Handles.DrawSolidDisc(position,Vector3.up, _data.StopMovementRadius);
                }
            }
        }

#endif
    }
}
