using UnityEngine;
using UnityEngine.Events;

namespace Fight
{
    public interface IHittable
    {
        public Transform Transform { get; set; }

        [field:SerializeField] public UnityEvent OnHit { get; set; }
        
        public virtual void Hit(Projectile projectile, GameObject owner)
        {
            OnHit.Invoke();
        }
    }
}