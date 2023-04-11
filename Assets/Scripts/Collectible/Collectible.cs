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
            gameObject.SetActive(false);

            CollectibleJsonFileManager instance = CollectibleJsonFileManager.Instance;
            if (instance != null)
            {
                instance.SetCollectibleCollected(this);
            }
        }

        public void SetCollectedAtStart()
        {
            gameObject.SetActive(false);
        }
    }
}
