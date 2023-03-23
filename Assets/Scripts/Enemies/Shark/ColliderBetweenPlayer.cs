using Kayak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBetweenPlayer : MonoBehaviour
{

    public SharkManager ManagerRef;
    private void OnTriggerEnter(Collider other)
    {
        KayakController kayakController = other.gameObject.GetComponent<KayakController>();
        if (kayakController != null)
        {
            ManagerRef.kayak = kayakController;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        KayakController kayakController = other.gameObject.GetComponent<KayakController>();

        if (kayakController != null)
        {
            ManagerRef.kayak = null;
        }
    }
}
