using System;
using Character;
using Sound;
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
        
        private Rigidbody _rigidbody;

        private StudioEventEmitter emitter;

        protected override void Update()
        {
            base.Update();
            transform.Rotate(new Vector3(0,1,0), _rotationSpeed);
        }
        
        protected override void Die()
        {
            base.Die();
            
            //CharacterManager.Instance.SoundManagerProperty.PlaySound(_dieSound);
            
            _dieParticles.transform.parent = null;
            _dieParticles.Play();
            
            Destroy(gameObject);

            //AudioManager.instance.PlayOneShot(FMODEvents.instance.netHit, this.transform.position);
        }
        
        public override void Launch(Vector3 direction, float power)
        {
            base.Launch(direction, power);
            
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _rigidbody.AddForce(direction * (Data.LaunchForce * power));

            //AudioManager.instance.PlayOneShot(FMODEvents.instance.netThrow, this.transform.position);

            //CharacterManager.Instance.SoundManagerProperty.PlaySound(_launchSound);
        }
    }
}
