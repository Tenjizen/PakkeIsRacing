using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using WaterAndFloating;

namespace IcebergFallingGPE
{
    public class IcebergFalling : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public Waves WavesManager { get; private set; }
        
        [SerializeField, ReadOnly] public bool Fall;
        [SerializeField, ReadOnly] public bool HasFallen;

        [Header("Parameters"), SerializeField] private float _fallDuration;
        [SerializeField] private Vector3 _endPosition;
        [SerializeField] private Vector3 _endRotation;
        [SerializeField] private AnimationCurve _fallSpeedCurve;
        [SerializeField] private float _timeToHitWater;
        [SerializeField] private CircularWave _circularWaveData;

        [Header("VFX"), SerializeField] private ParticleSystem _particleWhenHitWater;

        private float _timer;
        private Vector3 _beginPosition, _targetPosition;

        [Header("Events")] 
        public UnityEvent OnWaterCollision;
        public UnityEvent OnFallStarted;

        private void Start()
        {
            _targetPosition = transform.position + _endPosition;
            _beginPosition = transform.position;
        }

        private void Update()
        {
            if (Fall)
            {
                HandleFall();
            }

            if (Fall && _timeToHitWater > 0)
            {
                ManageWaveLaunch();
            }
        }

        /// <summary>
        /// manage and check the moment to launch the wave
        /// </summary>
        private void ManageWaveLaunch()
        {
            _timeToHitWater -= Time.deltaTime;
            if (_timeToHitWater <= 0)
            {
                _circularWaveData.Center = new Vector2(transform.position.x, transform.position.z);
                
                if (_particleWhenHitWater != null)
                {
                    _particleWhenHitWater.gameObject.transform.position = transform.position;
                    _particleWhenHitWater.Play();
                }
                
                WavesManager.LaunchCircularWave(_circularWaveData);
                OnWaterCollision.Invoke();
            }
        }

        /// <summary>
        /// Start the fall
        /// </summary>
        public void SetFall()
        {
            if (Fall == false && _timer < 1)
            {
                Fall = true;
                HasFallen = true;
                _timer = 0;
                OnFallStarted.Invoke();
            }
        }

        /// <summary>
        /// Manage the iceberg fall
        /// </summary>
        private void HandleFall()
        {
            _timer += Time.deltaTime;
            float fallProgress = _timer / _fallDuration;
            Vector3 targetPosition = Vector3.Lerp(_beginPosition, _targetPosition, _fallSpeedCurve.Evaluate(fallProgress));
            Quaternion targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_endRotation), _fallSpeedCurve.Evaluate(fallProgress));
            transform.position = targetPosition;
            transform.rotation = targetRotation;

            if (fallProgress >= 1)
            {
                Fall = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 startPosition = HasFallen ? _beginPosition : transform.position;
            Vector3 endPosition = HasFallen ?  _targetPosition : transform.position + _endPosition;
            
            //line
            Gizmos.DrawSphere(endPosition, 1f);
            Handles.DrawDottedLine(startPosition, endPosition, 0.5f);
            
            //wave
            Gizmos.color = Color.red;
            Handles.DrawWireDisc(endPosition, Vector3.up, _circularWaveData.Distance);
            Handles.DrawWireDisc(endPosition + Vector3.up*_circularWaveData.Amplitude, Vector3.up, _circularWaveData.Distance);
            
            //points
            Gizmos.color = new Color(1f, 1f, 1f, 0.28f);
            float angleDifference = 360 / (float)_circularWaveData.NumberOfPoints;
            for (int i = 1; i <= _circularWaveData.NumberOfPoints; i++)
            {
                float angle = i * angleDifference;
                Vector3 endPoint = MathTools.GetPointFromAngleAndDistance(endPosition, angle, _circularWaveData.Distance);
                Gizmos.DrawLine(endPosition,endPoint);
            }
        }
#endif
    }
}