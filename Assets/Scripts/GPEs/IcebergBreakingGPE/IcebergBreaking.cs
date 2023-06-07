using Fight;
using Fight.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IcebergBreaking : MonoBehaviour
{
    
    [Header("References"),SerializeField] GameObject _normalObject;
    [SerializeField] GameObject _breakingObject;

    [Header("VFX"), SerializeField] ParticleSystem _hitParticles;

    private bool _startLifeTimer = false;

    void Start()
    {
        _normalObject.SetActive(true);
        _breakingObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DestroyIceberg();
        }

        if (_startLifeTimer == true)
        {
            Destroy(this.gameObject, 3.0f);
        }
    }
   
    public void DestroyIceberg()
    {
        _normalObject.SetActive(false);
        _breakingObject.SetActive(true);
        _startLifeTimer = true;

        if (_hitParticles != null)
        {
            _hitParticles.transform.parent = null;
            _hitParticles.Play();
        }
    }


}
