using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : MonoBehaviour
{
    public List<ParticleSystem> particleSystems = new();

    public void Play()
    {
        foreach (ParticleSystem p in particleSystems) p.Play();
    }
}
