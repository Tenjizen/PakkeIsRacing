using Fight;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IHittable
    {
        public Transform Transform
        {
            get { return transform;}
            set {}
        }

        [field:SerializeField] public UnityEvent OnHit { get; set; }
        [field:SerializeField] public UnityEvent OnDie { get; set; }
        
        [field:SerializeField, ReadOnly] public float CurrentLife { get; set; }

        public virtual void Hit(Projectile projectile, GameObject owner)
        {
            OnHit.Invoke();
            CurrentLife -= 1;
            if (CurrentLife <= 0)
            {
                OnDie.Invoke();
            }
        }
    }
}