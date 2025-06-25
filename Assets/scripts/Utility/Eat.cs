using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : MonoBehaviour
{
    ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    public void PlayEating()
    {
        StartCoroutine(Eating());
    }


    IEnumerator Eating()
    {
        particle.Play();

        yield return CoroutneManager.waitForone;

        particle.Stop();
        ParticleManager.ClearParticle(gameObject, ParticleType.Eating);
    }


}
