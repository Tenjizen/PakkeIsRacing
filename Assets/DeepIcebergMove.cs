using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepIcebergMove : MonoBehaviour
{
    [SerializeField] private float posYMax;
    [SerializeField] private float posYMin;
    [SerializeField] private float moveY;
    private bool moveUp;

    // Start is called before the first frame update
    void Start()
    {
        moveUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        var posY = GetComponent<Transform>().position.y;
        if (posY <= posYMax && moveUp == true)
        {
            transform.position += new Vector3(0, moveY, 0); 
        }
        if (posY >= posYMax)
        {
            moveUp = false;
        }
        if (posY >= posYMin && moveUp == false)
        {
            transform.position -= new Vector3(0, moveY, 0);
        }
        if (posY <= posYMin)
        {
            moveUp = true;
        }
    }
}
