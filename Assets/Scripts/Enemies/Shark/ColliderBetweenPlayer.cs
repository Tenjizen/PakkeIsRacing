using System;
using Character;
using Kayak;
using UnityEngine;

namespace Enemies.Shark
{
    public class ColliderBetweenPlayer : MonoBehaviour
    {
        private SharkManager _sharkManager;

        private void OnTriggerEnter(Collider other)
        {
            KayakController kayakController = other.gameObject.GetComponent<KayakController>();
            if (kayakController != null)
            {
                _sharkManager.KayakControllerRef = kayakController;
            }
        
        }
        
        private void OnTriggerExit(Collider other)
        {
            KayakController kayakController = other.gameObject.GetComponent<KayakController>();

            if (kayakController != null)
            {
                _sharkManager.KayakControllerRef = null;
            }
        }
    }
}
