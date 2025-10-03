using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CoffeeMachine : FoodMachine
{
    public Transform coffeeTrans;
    Food food;
    Food createdFood;
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
            createdFood.transform.position = foodTransform.position + (foodStack.foodStack.Count - 1) * height * Vector3.up;
            createdFood = null;
        }
        audioSource.Stop();
    }
    public void Shake(float timer, CancellationToken cancellationToken)
    {
        createdFood = null;
        food = FoodManager.GetFood(foodMesh, machineType);
        food.transform.SetParent(coffeeTrans);
        food.transform.position = coffeeTrans.position;
        food.transform.localScale = Vector3.zero;

       // StartCoroutine(CreateCoffee(food, timer));
        CreateCoffee(food, timer, cancellationToken).Forget();
    }

    public async void Done(CancellationToken cancellationToken)
    {
        audioSource.Stop();
      //  StartCoroutine(CreateCoffeeDone());
        await CreateCoffeeDone(cancellationToken); 
        cancellationToken.ThrowIfCancellationRequested();
        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.CreateFood(), 0.4f, 100, 5, transform.position);
    }

    public async UniTask CreateCoffee(Food food, float timer, CancellationToken cancellationToken)
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
     //   yield return StartCoroutine(Utility.CustomCoroutineDelay(0.5f));
        await Utility.CustomDelayTask(0.5f, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        await Shaking(timer, cancellationToken);
    //    yield return StartCoroutine(Shaking(timer));
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

    public async UniTask CreateCoffeeDone(CancellationToken cancellationToken)
    {
        food.transform.SetParent(WorkSpaceManager.foodCollects.transform);
        //  yield return CoroutneManager.waitForzerothree;
        //  yield return StartCoroutine(Utility.CustomCoroutineDelay(0.3f));
        await Utility.CustomDelayTask(0.3f, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        createdFood = food;
        CreatedFood(createdFood);
        food = null;

        foodStack.foodStack.Push(createdFood);

        createdFood.transform.DOJump(foodTransform.position + (foodStack.foodStack.Count - 1) * height * Vector3.up, 2, 1, 0.4f);
    }
}
