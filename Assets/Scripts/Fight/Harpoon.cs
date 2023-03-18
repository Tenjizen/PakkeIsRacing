using System;
using UnityEngine;

namespace Fight
{
    public class Harpoon : Projectile
    {
        [Header("VFX"), SerializeField] private ParticleSystem _dieParticles;
        [Header("Sound"), SerializeField] private AudioClip _launchSound;
        [SerializeField] private AudioClip _dieSound;
        
        private Rigidbody _rigidbody;

        private void FixedUpdate()
        {
            if (_rigidbody != null)
            {
                Vector3 directionOfMotion = _rigidbody.velocity.normalized;
                if (directionOfMotion != Vector3.zero)
                {
                    transform.LookAt(transform.position + directionOfMotion);
                }
            }
        }

        protected override void Die()
        {
            base.Die();
            
            SoundManager.Instance.PlaySound(_dieSound);
            
            _dieParticles.transform.parent = null;
            _dieParticles.Play();
            
            Destroy(gameObject);
        }
        
        public override void Launch(Vector3 direction)
        {
            base.Launch(direction);
            
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _rigidbody.AddForce(direction * Data.LaunchForce);
            
            SoundManager.Instance.PlaySound(_launchSound);
        }
    }
}
