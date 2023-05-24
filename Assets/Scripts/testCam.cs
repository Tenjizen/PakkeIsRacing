using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCam : MonoBehaviour
{
    public float speed = 5;

    public Rigidbody rb;


    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            Vector3 movement = new Vector3(0, 0f, 1) * speed;
            rb.velocity = movement;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 movement = new Vector3(0, 0f, .5f) * speed;
            rb.velocity = movement;
        }

        Debug.Log(rb.velocity.magnitude);
        //else
        //{
        //    Vector3 movement = new Vector3(0, 0f, 0) * speed;
        //    rb.velocity = movement;
        //}

    }
}
