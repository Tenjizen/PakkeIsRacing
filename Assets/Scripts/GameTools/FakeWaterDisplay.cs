using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWaterDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _fakeWater;
    // Start is called before the first frame update
    void Start()
    {
        _fakeWater.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
