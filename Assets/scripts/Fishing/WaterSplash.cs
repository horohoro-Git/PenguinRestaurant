using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    ParticleSystem particle;
    public ParticleSystem subParticleSystem;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }



    public void PlayParticle()
    {
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.material.SetColor("_Color_1", GameInstance.GameIns.fishingManager.water.material.GetColor("_ShallowColor"));
        renderer.material.SetColor("_Color_2", GameInstance.GameIns.fishingManager.water.material.GetColor("_ShallowColor"));

        ParticleSystemRenderer renderer2 = subParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer2.material.SetColor("_Color_1", GameInstance.GameIns.fishingManager.water.material.GetColor("_ShallowColor"));
        renderer2.material.SetColor("_Color_2", GameInstance.GameIns.fishingManager.water.material.GetColor("_ShallowColor"));
        particle.Play();
        subParticleSystem.Play();

        StartCoroutine(LifeSpan());
    }


    IEnumerator LifeSpan()
    {
        float timer = 0;
        while(timer < 3)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        ParticleManager.ClearParticle(gameObject, ParticleType.Fishing);
        //GameInstance.GameIns.fishingManager.RemoveSplash(this);
    }
}
