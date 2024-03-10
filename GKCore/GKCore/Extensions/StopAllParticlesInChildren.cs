using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAllParticlesInChildren : MonoBehaviour
{
    public float delay = 0;
    public void Start(){
        Debug.Log("Will Stop All Particles in " + delay + " seconds");
        Invoke("StopAllParticles", delay);
    }
    public void StopAllParticles(){
        Debug.Log("Stop All Particles");
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        foreach(ParticleSystem particleSystem in particleSystems){
            particleSystem.Stop();
            Debug.Log("Stop Particle System: " + particleSystem.name);
        }
    }
}
