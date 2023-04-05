using Fight;
using UnityEngine;

namespace Collectible
{
    public class Collectible : MonoBehaviour, IHittable
    {
        public void Hit(Projectile projectile, GameObject owner)
        {
            Debug.Log("hit");
            SetCollected();
        }

        public void SetCollected()
        {
            Destroy(gameObject);
        }
    }
}
