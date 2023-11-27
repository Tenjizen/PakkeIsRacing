using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bump : MonoBehaviour
{
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _radius;

    public void Explode(Collider mainCollider)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            if (collider != mainCollider && collider.CompareTag("Kayak"))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    var dir = collider.transform.position - transform.position;
                    dir.y = 0;
                    rb.AddRelativeForce(dir.normalized * _explosionForce, ForceMode.Impulse);
                    //rb.AddExplosionForce(_explosionForce, transform.position, _radius);
                }
            }
        }
        
        Destroy(gameObject, 5);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}