using UnityEngine;

namespace Fight
{
    public class Harpoon : Projectile
    {
        public void LaunchHarpoon()
        {
            
        }
        
        protected override void HitHittable()
        {
            base.HitHittable();
        }

        protected override void HitNonHittable()
        {
            base.HitNonHittable();
        }
    }
}
