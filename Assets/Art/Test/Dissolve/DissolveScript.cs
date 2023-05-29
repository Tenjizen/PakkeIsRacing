using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolveScript : MonoBehaviour
{
    [SerializeField] private float noiseStrength = 0.25f;
    [SerializeField] private float objectHeight = 1.0f;
    [SerializeField] private float invisiblevalue = 0.5f;
    [SerializeField] private float visiblevalue = 2.5f;
    [SerializeField] private float timetoshow = 1.5f;
    private float time;
    private bool launcheffect;
    


    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void Launch()
    {
        time = 0;
        launcheffect = true;
    }

    private void Update()
    {
        if(launcheffect = false)
        {
            return;
        }
        time += Time.deltaTime;
        SetHeight((time / timetoshow)*(visiblevalue-invisiblevalue)+invisiblevalue);


        //var time = Time.time * Mathf.PI * 0.25f;

        //float height = transform.position.y;
        //height += Mathf.Sin(time) * (objectHeight / 2.0f);
        //SetHeight(height);
    }

    private void SetHeight(float height)
    {
        Debug.Log(height);
        material.SetFloat("_CutoffHeight", height);
        material.SetFloat("_NoiseStrength", noiseStrength);
    }
}