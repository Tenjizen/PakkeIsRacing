using System;
using UnityEngine;

namespace Fight
{
    public abstract class Projectile : MonoBehaviour
    {
        public GameObject Owner;

        private void OnTriggerEnter(Collider other)
        {
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.Hit(this);
            }
        }
    }
}