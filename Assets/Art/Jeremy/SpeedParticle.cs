using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class SpeedParticle : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] float _emissionMin = 0f;
    [SerializeField] float _emissionMax = 15f;

    float _maxSpeed;

    private void Start()
    {
        _maxSpeed = CharacterManager.Instance.KayakControllerProperty.Data.KayakValues.MaximumFrontVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
        var vel = _rigidbody.velocity.magnitude;

        var emission = _particleSystem.emission;
        emission.rateOverDistance = Mathf.Lerp(_emissionMin, _emissionMax, vel/ _maxSpeed);

    }
}
