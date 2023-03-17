using System;
using UnityEngine;

namespace Fight
{
    public abstract class Projectile : MonoBehaviour
    {
        public GameObject Owner;
        public WeaponData Data;

        private void OnTriggerEnter(Collider other)
        {
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.Hit(this, Owner);
                HitHittable();
                return;
            }
            HitNonHittable();
        }

        protected virtual void HitHittable() { }
        
        protected virtual void HitNonHittable() { }
    }
}