using System;
using UnityEngine;

namespace Fight
{
    public class Harpoon : Projectile
    {
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

        protected override void HitHittable()
        {
            base.HitHittable();
        }

        protected override void HitNonHittable()
        {
            base.HitNonHittable();
        }
        
        public override void Launch(Vector3 direction)
        {
            base.Launch(direction);
            
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            
            _rigidbody.AddForce(direction * Data.LaunchForce);
        }
    }
}
