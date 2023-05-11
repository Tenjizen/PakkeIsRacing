using UnityEngine;

namespace Fight
{
    public interface IHittable
    {
        public Transform Transform { get; set; }
        
        public virtual void Hit(Projectile projectile, GameObject owner)
        {
        }
    }
}