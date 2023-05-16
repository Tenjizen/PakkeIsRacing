using System;
using Character;
using Sound;
using Unity.Mathematics;
using UnityEngine;
using FMODUnity;

namespace Fight
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class Harpoon : Projectile
    {

        [Header("VFX"), SerializeField] private ParticleSystem _dieParticles;
        [Header("Sound"), SerializeField] private AudioClip _launchSound;
        [SerializeField] private AudioClip _dieSound;
        
        private Rigidbody _rigidbody;
        private StudioEventEmitter _emitter;

        private void FixedUpdate()
        {
            if (_rigidbody != null)
            {
                Vector3 directionOfMotion = _rigidbody.velocity.normalized;
                if (directionOfMotion != Vector3.zero)
                {
                    transform.LookAt(transform.position + directionOfMotion);
                }

                if (AutoAimHittable == null)
                {
                    return;
                }
                Vector3 direction = (AutoAimHittable.Transform.position - transform.position).normalized;
                Vector3 desiredVelocity = direction * _rigidbody.velocity.magnitude;
                _rigidbody.velocity = desiredVelocity;
            }
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
    }
}
