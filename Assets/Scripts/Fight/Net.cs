using System;
using Sound;
using UnityEngine;

namespace Fight
{
    public class Net : Projectile
    {
        [Header("VFX"), SerializeField] private ParticleSystem _dieParticles;
        [Header("Sound"), SerializeField] private AudioClip _launchSound;
        [SerializeField] private AudioClip _dieSound;

        [Header("Parameters"), SerializeField, Range(0,30)] private float _rotationSpeed = 0.01f;
        
        private Rigidbody _rigidbody;

        protected override void Update()
        {
            base.Update();
            transform.Rotate(new Vector3(0,1,0), _rotationSpeed);
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
