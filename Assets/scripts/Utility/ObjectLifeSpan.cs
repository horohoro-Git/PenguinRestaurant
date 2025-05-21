using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class ObjectLifeSpan : MonoBehaviour
{
    float lifeTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (lifeTime < 5)
        {
            lifeTime += Time.deltaTime;
        }
        else
        {
            lifeTime = 0;
            ParticleManager.ClearParticle(this.gameObject);
        }
    }
}
