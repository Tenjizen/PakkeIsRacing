using Fight;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, IHittable
    {
        public virtual void Hit(Projectile projectile, GameObject owner) { }
    }
}