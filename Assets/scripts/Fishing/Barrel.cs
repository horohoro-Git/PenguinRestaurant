using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [NonSerialized] public Transform target;

    private void Update()
    {
        if(target)
        {
            transform.position = target.position;
        }
    }

    public void Throw(Vector3 target)
    {
        transform.DOJump(target, 10, 1, 1f).onComplete += ThrowComplete;
    }

    void ThrowComplete()
    {
        StartCoroutine(Falling());
    }

    IEnumerator Falling()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.fishingSoundManager.WaterSplashSound(), 0.2f);

        WaterSplash waterSplash = ParticleManager.CreateParticle(ParticleType.Fishing).GetComponent<WaterSplash>(); //GameInstance.GameIns.fishingManager.GetSplash();
        waterSplash.transform.position = transform.position;
        waterSplash.transform.localScale = new Vector3(3, 3, 3);
        waterSplash.PlayParticle();

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, 90);
    
        float f = 0;
        float cur = transform.position.y;
        float target = -10;
        while (f < 1)
        {
            transform.rotation = Quaternion.Lerp(startRot, endRot, f);
            float next = Mathf.Lerp(cur, target, f);
            Vector3 pos = transform.position;
            pos.y = next;
            transform.position = pos;
            f += Time.unscaledDeltaTime / 2f;
            yield return null;
        }
    }
}
