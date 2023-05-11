using Collectible.Data;
using Fight;
using Json;
using UnityEngine;
using UnityEngine.Events;

namespace Collectible
{
    public class Collectible : MonoBehaviour, IHittable
    {
        public UnityEvent OnCollected = new UnityEvent();
        public CollectibleData Data;

        public Transform Transform
        {
            get { return transform; }
            set { }
        }

        public void Hit(Projectile projectile, GameObject owner)
        {
            Debug.Log("hit");
            SetCollected();
        }

        private void SetCollected()
        {
            gameObject.SetActive(false);
            OnCollected.Invoke();
            
            CollectibleJsonFileManager instance = JsonFilesManagerSingleton.Instance.CollectibleJsonFileManagerProperty;
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
