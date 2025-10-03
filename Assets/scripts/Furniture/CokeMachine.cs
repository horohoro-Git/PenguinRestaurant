using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CokeMachine : FoodMachine
{
    public Transform cokeTrans;

    Food food;
    Food createdFood;
    Coroutine createCokeCoroutine;
    public override void Start()
    {
        base.Start();

        height = 0.9f;
    }

    public override void OnDisable()
    {
        if (isQuitting) return;
        base.OnDisable();
        if (food)
        {
            FoodManager.EatFood(food);
            food = null;    
        }
        if(createdFood)
        {
            createdFood.transform.position = foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height;
            createdFood = null;
        }
        audioSource.Stop();
    }
    public void Shake(float timer, CancellationToken cancellationToken)
    {
        createdFood = null;
        if (gameObject.activeSelf)
        {

            food = FoodManager.GetFood(foodMesh, machineType);
            food.transform.SetParent(cokeTrans);
            food.transform.position = cokeTrans.position;
            food.transform.localScale = Vector3.zero;
            CreateCoke(food, timer, cancellationToken).Forget();
          //  createCokeCoroutine = StartCoroutine(CreateCoke(food, timer));
        }
    }

    public async UniTask CreateCoke(Food food, float timer, CancellationToken cancellationToken)
    {
        Vector3 v1 = food.transform.localScale;
        Vector3 v2 = new Vector3(1, 1, 1);
        Vector3 v3 = new Vector3(1.2f, 1.2f, 1.2f);
        float f = 0;
        while (f <= 0.2f)
        {
            if (App.restaurantTimeScale == 1)
            {
                food.transform.localScale = Vector3.Lerp(v1, v3, f * 5);
                f += Time.deltaTime;
            }
            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        f = 0;
        while (f <= 0.1f)
        {
            if (App.restaurantTimeScale == 1)
            {
                food.transform.localScale = Vector3.Lerp(v3, v2, f * 10);
                f += Time.deltaTime;
            }
            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        food.transform.localScale = v2;
        /*
                yield return waitForzeoroone;

                foodStack.foodStack.Push(food);

                food.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * 0.7f, 2, 1, 0.4f);*/
        //  yield return CoroutneManager.waitForzerofive;
        //yield return StartCoroutine(Utility.CustomCoroutineDelay(0.5f));
        await Utility.CustomDelayTask(0.5f, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await Shaking(timer, cancellationToken: cancellationToken);
        //yield return StartCoroutine(Shaking(timer));
    }

    public void Done(CancellationToken cancellationToken)
    {
       // if(createCokeCoroutine != null) StopCoroutine(createCokeCoroutine);
        audioSource.Stop();
        CreateCokeDone(cancellationToken).Forget();
        //StartCoroutine(CreateCokeDone());
        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.CreateFood(), 0.4f, 100, 5, transform.position);
    }

    public async UniTask Shaking(float timer, CancellationToken cancellationToken)
    {
        timer = (timer - 0.8f);
        Vector3 cur = transform.position;
        float f = 0;
        while (f <= timer)
        {
            if (App.restaurantTimeScale == 1)
            {
                Vector3 v2 = cur + transform.right * UnityEngine.Random.Range(-0.1f, 0.1f);
                transform.position = v2;
                f += Time.deltaTime;
            }
            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        transform.position = cur;
    }

    public async UniTask CreateCokeDone(CancellationToken cancellationToken)
    {
        food.transform.SetParent(WorkSpaceManager.foodCollects.transform);
        //  yield return CoroutneManager.waitForzerothree;
    //    yield return StartCoroutine(Utility.CustomCoroutineDelay(0.3f));
        await Utility.CustomDelayTask(0.3f, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        createdFood = food;
        CreatedFood(createdFood);
        food = null;
        foodStack.foodStack.Push(createdFood);

        createdFood.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height, 2, 1, 0.4f);
    }
}
