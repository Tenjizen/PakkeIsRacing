using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSednaFollow : MonoBehaviour
{
    public float speed;

    public Rigidbody kayak;


    public GameObject target;

    public float distTarget;


    public Rigidbody rb;

    void Update()
    {
        distTarget = Vector3.Distance(target.transform.position, transform.position);



        Vector3 direction = target.transform.position - transform.position;
        //direction.z = 0;



        if (kayak.velocity.magnitude > 1)
        {
            if (distTarget < 0.5f)
            {
                speed = kayak.velocity.magnitude - 0.5f;
            }
            else if(distTarget > 5)
            {
                speed = kayak.velocity.magnitude + 0.5f;
            }

            direction.Normalize();
            Vector3 movement = direction * speed;
            rb.velocity = movement;


            //Vector3 movement = direction * speed * Time.deltaTime;
            //transform.position += movement;
        }

    }
}
