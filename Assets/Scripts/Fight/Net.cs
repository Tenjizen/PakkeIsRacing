using System;
using UnityEngine;
using FMODUnity;

namespace Fight
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class Net : Projectile
    {
        [Header("VFX"), SerializeField] private ParticleSystem _dieParticles;

        [Header("Parameters"), SerializeField, Range(0,30)] private float _rotationSpeed = 0.01f;
        
        private StudioEventEmitter _emitter;

        protected override void Update()
        {
            base.Update();
            transform.Rotate(new Vector3(0,1,0), _rotationSpeed);
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
            
            RigidbodyProjectile.useGravity = true;
            RigidbodyProjectile.AddForce(direction * (Data.LaunchForce * power));
        }
    }

    
}
