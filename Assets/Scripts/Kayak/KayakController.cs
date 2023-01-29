using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kayak
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class KayakController : MonoBehaviour
    {
        public KayakParameters KayakValues;
        public Rigidbody Rigidbody;

        [Header("References"), SerializeField] private List<ParticleSystem> _frontParticles;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();

        }
        private void Update()
        {
            ClampVelocity();
            ParticleManagement();
        }

        private void ClampVelocity()
        {
            Vector3 velocity = Rigidbody.velocity;
            float velocityZ = velocity.z;
            velocityZ = Mathf.Clamp(velocityZ, -KayakValues.MaximumFrontVelocity, KayakValues.MaximumFrontVelocity);
            Rigidbody.velocity = new Vector3(velocity.x, velocity.y, velocityZ);
        }

        private void ParticleManagement()
        {
            if (Rigidbody.velocity.magnitude > 1)
            {
                _frontParticles.ForEach(x => x.Play());
            }
            else
            {
                _frontParticles.ForEach(x => x.Stop());
            }
        }
    }

    [Serializable]
    public struct KayakParameters
    {
        [Header("Values")]
        [Range(0,20)] public float MaximumFrontVelocity;

        [Header("Paddle")]
        [Range(0,200)] public float PaddleFrontForce;
        [Range(0,2)] public float PaddleSideRotationForce;
        [Range(0,3)] public float PaddleCooldown;
        [Range(0, 0.25f)] public float PaddleRotationDeceleration;

        [Header("Static Rotation")] 
        [Range(0, 0.1f)] public float StaticRotationForce;
        [Range(0, 0.25f)] public float StaticRotationDeceleration;
        [Range(0,0.1f)] public float VelocityDecelerationLerp;
        [Range(0, 0.1f)] public float VelocityDecelerationRotationForce;
    }
}