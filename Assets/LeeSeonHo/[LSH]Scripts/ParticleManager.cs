using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
//ÇÑ±Û

public static class ParticleManager
{
    static Dictionary<ParticleType, Queue<GameObject>> deActivatedParticles = new Dictionary<ParticleType, Queue<GameObject>>();
    static Dictionary<ParticleType, List<GameObject>> activatedParticles = new Dictionary<ParticleType, List<GameObject>>();

  
    public static void NewParticle(GameObject particle, int count, ParticleType particleType)
    {
        activatedParticles[particleType] = new List<GameObject>();
        deActivatedParticles[particleType] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject p = GameObject.Instantiate(particle, WorkSpaceManager.particleCollects.transform);

            p.SetActive(false);
            deActivatedParticles[particleType].Enqueue(p);
        }
    }

    public static GameObject CreateParticle(ParticleType particleType)
    {
        GameObject p = deActivatedParticles[particleType].Dequeue();
        p.SetActive(true);
        activatedParticles[particleType].Add(p);
        return p;
    }

    public static void ClearParticle(GameObject particle, ParticleType particleType)
    {
        particle.SetActive(false);
        deActivatedParticles[particleType].Enqueue(particle);
        activatedParticles[particleType].Remove(particle);
    }
   /* public static void AllClear()
    {
        while (activatedParticles.Count > 0)
        {
            GameObject particle = activatedParticles[activatedParticles.Count - 1];
            activatedParticles.Remove(particle);
            deActivatedParticles.Enqueue()
        }
    }*/
}