using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMachine : FoodMachine
{
    public Transform coffeeTrans;
    Food food;
    public void Shake(float timer)
    {
        food = FoodManager.GetFood(foodMesh, machineType);
        food.transform.SetParent(coffeeTrans);
        food.transform.position = coffeeTrans.position;
        food.transform.localScale = Vector3.zero;

        StartCoroutine(CreateCoffee(food, timer));
    }

    public void Done()
    {
        StartCoroutine(CreateCoffeeDone());
    }

    public IEnumerator CreateCoffee(Food food, float timer)
    {
        Vector3 v1 = food.transform.localScale;
        Vector3 v2 = new Vector3(1, 1, 1);
        Vector3 v3 = new Vector3(1.2f, 1.2f, 1.2f);
        float f = 0;
        while (f < 0.2f)
        {
            food.transform.localScale = Vector3.Lerp(v1, v3, f * 5);
            f += Time.deltaTime;
            yield return null;
        }
        f = 0;
        while (f < 0.1f)
        {
            food.transform.localScale = Vector3.Lerp(v3, v2, f * 10);
            f += Time.deltaTime;
            yield return null;
        }
        food.transform.localScale = v2;
        /*
                yield return waitForzeoroone;

                foodStack.foodStack.Push(food);

                food.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * 0.7f, 2, 1, 0.4f);*/
        yield return CoroutneManager.waitForzerofive;

        yield return StartCoroutine(Shaking(timer));
    }

    public IEnumerator Shaking(float timer)
    {
        timer = (timer - 0.8f);
        Vector3 cur = transform.position;
        float f = 0;
        while (f < timer)
        {
            Vector3 v2 = cur + transform.right * UnityEngine.Random.Range(-0.1f, 0.1f);
            transform.position = v2;
            f += Time.deltaTime;
            yield return null;
        }
        transform.position = cur;
    }

    public IEnumerator CreateCoffeeDone()
    {
        food.transform.SetParent(FoodManager.foodCollects.transform);
        yield return CoroutneManager.waitForzerothree;

        foodStack.foodStack.Push(food);

        food.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * 0.7f, 2, 1, 0.4f);
    }
}
