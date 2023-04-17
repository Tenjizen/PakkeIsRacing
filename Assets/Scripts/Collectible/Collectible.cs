using Collectible.Data;
using Fight;
using UnityEngine;
using UnityEngine.Events;

namespace Collectible
{
    public class Collectible : MonoBehaviour, IHittable
    {
        public UnityEvent OnCollected = new UnityEvent();
        public CollectibleData Data;
        
        public void Hit(Projectile projectile, GameObject owner)
        {
            Debug.Log("hit");
            SetCollected();
        }

        public void SetCollected()
        {
            gameObject.SetActive(false);
            OnCollected.Invoke();
            
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
