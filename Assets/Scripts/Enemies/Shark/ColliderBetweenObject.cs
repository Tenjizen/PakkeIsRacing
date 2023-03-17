using Kayak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBetweenObject : MonoBehaviour
{

    public SharkManager SharkManagerRef;
    private void OnTriggerEnter(Collider other)
    {
        KayakController kayakController = other.gameObject.GetComponent<KayakController>();
        if (kayakController != null)
        {
            SharkManagerRef.kayak = kayakController;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        KayakController kayakController = other.gameObject.GetComponent<KayakController>();

        if (kayakController != null)
        {
            SharkManagerRef.kayak = null;
        }
    }
}
