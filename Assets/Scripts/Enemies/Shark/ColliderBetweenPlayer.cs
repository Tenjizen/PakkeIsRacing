using Kayak;
using UnityEngine;

namespace Enemies.Shark
{
    public class ColliderBetweenPlayer : MonoBehaviour
    {
        [field:SerializeField] public SharkManager ManagerRef { get; private set; }
        
        private void OnTriggerEnter(Collider other)
        {
            KayakController kayakController = other.gameObject.GetComponent<KayakController>();
            if (kayakController != null)
            {
                ManagerRef.KayakControllerProperty = kayakController;
            }
        
        }
        
        private void OnTriggerExit(Collider other)
        {
            KayakController kayakController = other.gameObject.GetComponent<KayakController>();

            if (kayakController != null)
            {
                ManagerRef.KayakControllerProperty = null;
            }
        }
    }
}
