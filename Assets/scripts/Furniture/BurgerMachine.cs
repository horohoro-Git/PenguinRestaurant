using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerMachine : FoodMachine
{
    public Transform pattyTrans;

    public GameObject testPatty;

    public GameObject currentFood;
    Food tempBurger;
    Food createdBurger;
    Coroutine bake;
    int coroutineStep = 0;

    public override void Start()
    {
        base.Start();
        height = 0.7f;
    }

    public override void OnDisable()
    {
        if(isQuitting) return;
        base.OnDisable();
        if(currentFood) Destroy(currentFood);
        if (tempBurger)
        {
            FoodManager.EatFood(tempBurger);
            tempBurger = null;
        }
        if (createdBurger)
        {
            createdBurger.transform.position = foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height;
            createdBurger = null;
        }
        audioSource.Stop();
    }
  
    public void GetPatty(float t)
    {
        createdBurger = null;
        currentFood = Instantiate(testPatty);
        currentFood.transform.position = pattyTrans.position;
        bake = StartCoroutine(BakePatty(t));
    }

    public void Done()
    {
        StopCoroutine(bake);
        Destroy(currentFood);
        audioSource.Stop();
        currentFood = null;

        Food f = FoodManager.GetFood(foodMesh, machineType);
        //currentFood = Instantiate(testPatty);
        f.transform.position = pattyTrans.position;
        f.transform.localScale = Vector3.zero;
        StartCoroutine(CreateBurger(f));

        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.CreateFood(), 0.4f, 100, 5, f.transform.position);
    }


    public IEnumerator CreateBurger(Food food)
    {
        Vector3 v1 = food.transform.localScale;
        Vector3 v2 = new Vector3(2, 2, 2);
        Vector3 v3 = new Vector3(2.4f, 2.4f, 2.4f);
        tempBurger = food;
        float f = 0;
        while (f <= 0.2f)
        {
            if (App.restaurantTimeScale == 1)
            {
                food.transform.localScale = Vector3.Lerp(v1, v3, f * 5);
                f += Time.deltaTime;
            }
            yield return null;
        }
        f = 0;
        while (f <= 0.1f)
        {
            if (App.restaurantTimeScale == 1)
            {
                food.transform.localScale = Vector3.Lerp(v3, v2, f * 10);
                f += Time.deltaTime;
            }
            yield return null;
        }
        food.transform.localScale = v2;

        //    yield return CoroutneManager.waitForzeroone;

        yield return StartCoroutine(Utility.CustomCoroutineDelay(0.1f));
        tempBurger = null;
        createdBurger = food;
        foodStack.foodStack.Push(food);

        food.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height, 2,1, 0.4f);
        coroutineStep = 0;
    }


    IEnumerator BakePatty(float t)
    {
        t = (t-1.5f) / 2;
        while (true)
        {
            currentFood.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //  yield return CoroutneManager.waitForzerofive;
            yield return StartCoroutine(Utility.CustomCoroutineDelay(0.5f));
            if (currentFood == null) yield break;
            Vector3 v1 = currentFood.transform.position;
            Vector3 t1 = currentFood.transform.position + Vector3.up * 3;

            float f = 0;
            while (f <= t * 0.2f)
            {
                if (App.restaurantTimeScale == 1)
                {
                    if (currentFood == null) yield break;


                    currentFood.transform.position = Vector3.Lerp(v1, t1, f * (1 / (t * t * 0.2f)) * t);
                    f += Time.deltaTime;// * App.restaurantTimeScale;
                }
                yield return null;
            }
            f = 0;
            Quaternion q1 = currentFood.transform.rotation;

            Quaternion qt1 = Quaternion.AngleAxis(180, transforms.right) * currentFood.transform.rotation; //Quaternion.Euler(180, 0, 0) * currentFood.transform.rotation;
            Quaternion qt2 = Quaternion.AngleAxis(360, transforms.right) * currentFood.transform.rotation;

            while (f <= t * 0.2f)
            {
                if (App.restaurantTimeScale == 1)
                {
                    if (currentFood == null) yield break;

                    currentFood.transform.rotation = Quaternion.Lerp(q1, qt1, f * (1 / (t * t * 0.2f)) * t);
                    f += Time.deltaTime;
                }
                yield return null;
            }

            f = 0;
            while (f <= t * 0.2f)
            {
                if (App.restaurantTimeScale == 1)
                {
                    if (currentFood == null) yield break;

                    currentFood.transform.rotation = Quaternion.Lerp(qt1, qt2, f * (1 / (t * t * 0.2f)) * t);
                    f += Time.deltaTime;
                }
                yield return null;
            }
          

            f = 0;


            while (f <= t * 0.4f)
            {
                if (App.restaurantTimeScale == 1)
                {
                    if (currentFood == null) yield break;

                    currentFood.transform.position = Vector3.Lerp(t1, v1, f * (1 / (t * t * 0.4f)) * t);
                    f += Time.deltaTime;
                }
                yield return null;
            }
            currentFood.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
