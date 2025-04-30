using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
//ÇÑ±Û

public static class ParticleManager
{
    static Queue<GameObject> deActivatedParticles = new Queue<GameObject>();
    static List<GameObject> activatedParticles = new List<GameObject>();

    public static void NewParticle(GameObject particle, int count)
    {
        GameObject particleCollects = new GameObject();
        particleCollects.name = "ParticleCollects";
        particleCollects.transform.position = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            GameObject p = GameObject.Instantiate(particle, particleCollects.transform);

            p.gameObject.SetActive(false);
            deActivatedParticles.Enqueue(p);
        }
    }

    public static GameObject CreateParticle()
    {
        GameObject p = deActivatedParticles.Dequeue();
        p.gameObject.SetActive(true);
        activatedParticles.Add(p);
        return p;
    }

    public static void ClearParticle(GameObject particle)
    {
        particle.gameObject.SetActive(false);
        deActivatedParticles.Enqueue(particle);
        activatedParticles.Remove(particle);
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