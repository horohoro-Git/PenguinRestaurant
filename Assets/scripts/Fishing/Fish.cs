using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Fish : Animal
{
    public AnimalStruct animalStruct;
    public SkinnedMeshRenderer skinnedMesh;
    [NonSerialized] public bool bDead;
    [NonSerialized] public bool bFloating;
    public Animator modelAnimator;

    public void Setup(AnimalStruct animalStruct)
    {
        this.animalStruct = animalStruct;
    }


    public void Swim()
    {
        skinnedMesh.enabled = false;
        Swimming(App.GlobalToken).Forget();
    }


    async UniTask Swimming(CancellationToken cancellationToken = default)
    {
        Vector3 target = GameInstance.GameIns.fishingManager.RandomLocation();
        Vector3 current = trans.position;
        modelAnimator.SetInteger(AnimationKeys.state, 1);
        PlayAnimation(AnimationKeys.Swim);
        foreach(var v in animationDic)
        {
            Debug.Log(v.Key);
        }
        while (true)
        {
            if (bDead)
            {
                await UniTask.NextFrame(cancellationToken: cancellationToken);
                Dead();
                return;
            }
            trans.position = Vector3.MoveTowards(trans.position, target, animalStruct.speed * Time.deltaTime);
            if (Vector3.Distance(trans.position, target) <= 0.01f) break;
            Vector3 dir = target - trans.position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }

        PlayAnimation(AnimationKeys.Idle);
        modelAnimator.SetInteger(AnimationKeys.state, 0);
        await UniTask.Delay(300, cancellationToken: cancellationToken);
        Swim();
    }

    void Dead()
    {
        CheckVisible(false, true);
        skinnedMesh.enabled = true;
      
        StartCoroutine(DeadFish());
    }

    IEnumerator DeadFish()
    {
        Quaternion startRot = modelTrans.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, 90);
        float t = 0f;

        while (t < 1f)
        {
            PlayAnimation(AnimationKeys.None);
            modelAnimator.SetTrigger(AnimationKeys.Dead);
            modelTrans.rotation = Quaternion.Lerp(startRot, endRot, t);
            t += Time.unscaledDeltaTime / 0.3f;
            yield return null;
        }

        modelTrans.rotation = endRot;

        yield return new WaitForSeconds(0.5f); //CoroutneManager.waitForzerofive_real;


        t = 0;
        float targetPos = 1f;
        while (t < 1f)
        {
            float height = Mathf.Lerp(-10, targetPos, t);
            Vector3 pos = modelTrans.position;
            pos.y = height; 
            modelTrans.position = pos;
            t += Time.unscaledDeltaTime / 2f;
            yield return null;
        }


        bFloating = true;
        StartCoroutine(Floating());
    }


    IEnumerator Floating()
    {
        bool up = false;
        while (bFloating)
        {
            float origin = 1f;
            float target = 0.5f;
            float timer = 0f;
            while (timer < 1)
            {
                float height = Mathf.Lerp(origin, target, timer);
                Vector3 pos = modelTrans.position;
                pos.y = height;
                modelTrans.position = pos;
                timer += Time.unscaledDeltaTime / 1.5f;
                yield return null;
            }

            timer = 0;
            while (timer < 0.5f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            while (timer < 1)
            {
                float height = Mathf.Lerp(target, origin, timer);
                Vector3 pos = modelTrans.position;
                pos.y = height;
                modelTrans.position = pos;
                timer += Time.unscaledDeltaTime / 1.5f;
                yield return null;
            }

            timer = 0;
            while (timer < 0.5f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    public void Caught()
    {
        bFloating = false;
        GameInstance.GameIns.fishingManager.RemoveFish(this);
    }
}
