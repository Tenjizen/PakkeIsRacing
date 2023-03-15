using UnityEngine;

namespace Fight
{
    public interface IHittable
    {
        public void Hit(Projectile projectile);
    }
}