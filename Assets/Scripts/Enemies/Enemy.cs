using Fight;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IHittable
    {
        public Transform Transform
        {
            get { return transform;}
            set {}
        }
        
        public virtual void Hit(Projectile projectile, GameObject owner) { }
    }
}