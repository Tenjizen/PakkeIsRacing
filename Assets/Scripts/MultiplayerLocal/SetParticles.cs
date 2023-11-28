using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SetParticles : MonoBehaviour
{
    [SerializeField] private List<ParticleType> _particles = new List<ParticleType>();

    public void StartParticle(string particleName)
    {
        foreach (ParticleType particle in _particles)
        {
            if (particleName == particle.Name)
            {
                Instantiate(particle.ParticleSystem, particle.Position);
            }
        }
    }

    public void StartColorParticle(ColorPlayer colorPlayer)
    {
        if (colorPlayer.CurrentColor.PrefabRespawnFX != null)
        {
            Instantiate(colorPlayer.CurrentColor.PrefabRespawnFX, transform.position + new Vector3(0,5,0), Quaternion.identity, transform);
        }
    }
}

[Serializable]
public class ParticleType
{
    public string Name;
    public ParticleSystem ParticleSystem;
    public Transform Position;
}