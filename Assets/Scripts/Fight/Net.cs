using System;
using UnityEngine;
using FMODUnity;

namespace Fight
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class Net : Projectile
    {
        [Header("VFX"), SerializeField] private ParticleSystem _dieParticles;
        [Header("Sound"), SerializeField] private AudioClip _launchSound;
        [SerializeField] private AudioClip _dieSound;

        [Header("Parameters"), SerializeField, Range(0,30)] private float _rotationSpeed = 0.01f;
        [SerializeField] private ShootArcMovementParameters _arcMovementParameters;
        
        private Rigidbody _rigidbody;
        private StudioEventEmitter _emitter;
        
        private Transform _target;
        private float _currentTime;
        private float _timeToReachTarget;
        private float _timeToReachApexFromBase, _timeToReachTargetFromApex;
        private float _apexHeight;

        protected override void Update()
        {
            base.Update();
            transform.Rotate(new Vector3(0,1,0), _rotationSpeed);
        }

        private void FixedUpdate()
        {
            if (_target == null)
            {
                return;
            }

            _currentTime += Time.deltaTime;
            
            //height
            float percentage = _currentTime <= _timeToReachApexFromBase
                    ? _currentTime / _timeToReachApexFromBase
                    : (_currentTime - _timeToReachApexFromBase) / _timeToReachTargetFromApex;
            percentage = Mathf.Clamp01(percentage);
            percentage = _currentTime <= _timeToReachApexFromBase
                ? _arcMovementParameters.BaseToApexCurve.Evaluate(percentage)
                : _arcMovementParameters.ApexToTargetCurve.Evaluate(percentage);

            Vector3 position = transform.position;
            float height = _target.position.y + percentage * _apexHeight;
            transform.position = new Vector3(position.x, height, position.z);
            
            //velocity
            Vector3 direction = (_target.position - position).normalized;
            Vector3 desiredVelocity = direction * _arcMovementParameters.ArcMovementSpeed;
            _rigidbody.velocity = desiredVelocity;
        }

        protected override void Die()
        {
            base.Die();
            
            _dieParticles.transform.parent = null;
            _dieParticles.Play();
            
            Destroy(gameObject);
        }
        
        public override void Launch(Vector3 direction, float power)
        {
            base.Launch(direction, power);
            
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _rigidbody.AddForce(direction * (Data.LaunchForce * power));
        }

        public override void Launch(Transform hittable)
        {
            base.Launch(hittable);
            
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            
            _target = hittable;
            _currentTime = 0;

            float distance = Vector3.Distance(transform.position, _target.position);
            _apexHeight = distance * _arcMovementParameters.BaseApexHeightForDistance1;
            _timeToReachTarget = distance / _arcMovementParameters.ArcMovementSpeed;
            _timeToReachApexFromBase = _timeToReachTarget * _arcMovementParameters.PercentOfFlyTimeToReachApex;
            _timeToReachTargetFromApex = _timeToReachTarget - _timeToReachApexFromBase;
        }
    }

    [Serializable]
    public struct ShootArcMovementParameters
    {
        public float BaseApexHeightForDistance1;
        public float ArcMovementSpeed;
        [Range(0,1)] public float PercentOfFlyTimeToReachApex;
        public AnimationCurve BaseToApexCurve;
        public AnimationCurve ApexToTargetCurve;
    }
}
