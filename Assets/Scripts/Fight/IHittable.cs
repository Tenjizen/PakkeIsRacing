using UnityEngine;

namespace Fight
{
    public interface IHittable
    {
        public virtual void Hit(Projectile projectile, GameObject owner)
        {
        }
    }
}