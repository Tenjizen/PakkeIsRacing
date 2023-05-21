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
        
        private StudioEventEmitter _emitter;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (RigidbodyProjectile == null)
            {
                return;
            }
            
            Vector3 directionOfMotion = RigidbodyProjectile.velocity.normalized;
            if (directionOfMotion != Vector3.zero)
            {
                transform.LookAt(transform.position + directionOfMotion);
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
            
            RigidbodyProjectile.useGravity = true;
            RigidbodyProjectile.AddForce(direction * (Data.LaunchForce * power));
        }
    }
}
