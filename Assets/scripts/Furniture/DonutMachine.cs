using DG.Tweening;
using SRF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DonutMachine : FoodMachine
{
    public Transform donutTrans;
    bool[,] frieds = new bool[3,2];
    Queue<(Food, Vector2Int)> friedQueue = new Queue<(Food, Vector2Int)>();
   /* WaitForSeconds waitForzerofive = new WaitForSeconds(0.5f);
    WaitForSeconds waitForzeroone = new WaitForSeconds(0.1f);
    WaitForSeconds waitForzerothree = new WaitForSeconds(0.3f);
    WaitForSeconds waitFortwo = new WaitForSeconds(2f);
    WaitForSeconds waitForone = new WaitForSeconds(1f);*/
    Food current;
    Food createdFood;
    
    // Start is called before the first frame update
    public override void Start()
    {
      
        base.Start();
        height = 0.7f;
    //    donutTrans.rotation = transforms.rotation;
       
    }
    public override void OnEnable()
    {
        base.OnEnable();
        frieds = new bool[3, 2];
        while (friedQueue.Count > 0)
        {
            Food f;
            Vector2Int a;
            (f, a) = friedQueue.Dequeue();
            FoodManager.EatFood(f);
        }
        StartCoroutine(AddDonuts());
      
    }
    public override void OnDisable()
    {
        if (isQuitting) return;
        base.OnDisable();

        if(current)
        {
            FoodManager.EatFood(current);
            current = null;
        }
        if (createdFood)
        {
            createdFood.transform.position = foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height;
            createdFood = null;
        }
    }
    IEnumerator AddDonuts()
    {
        List<int> ints = new List<int> { 1, 2, 3, 4, 5, 6 };
        Shuffle(ints); 
        for (int i=0; i< ints.Count; i++)
        {
            Food food = FoodManager.GetFood(foodMesh, machineType);
            food.transform.SetParent(donutTrans);
            int pos = ints[i] - 1;
            int x = pos % 3;
            int y = pos / 3;
            // food.transform.rotation = transforms.rotation;
            Vector3 target = new Vector3(x * 0.6f, 0, y * 0.5f); //transforms.forward * (y) * 0.5f + transforms.right * (x) * 0.6f;
            food.transform.localPosition = target;
            food.transform.localScale = Vector3.zero;
            frieds[x, y] = true;
            friedQueue.Enqueue((food, new Vector2Int(x,y)));
            StartCoroutine(DonutVitlity(food));
            yield return CoroutneManager.waitForzeroone;
        }

        while (true)
        {
            yield return CoroutneManager.waitFortwo;

            if(friedQueue.Count < 6)
            {
                if(friedQueue.Count >= 2) yield return CoroutneManager.waitFortwo;
                for (int i=0; i < 3; i++)
                {
                    for (int j=0; j<2; j++)
                    {
                        if (frieds[i, j] == false)
                        {
                            Food food = FoodManager.GetFood(foodMesh, machineType);
                            food.transform.SetParent(donutTrans);
                            food.transform.localScale = Vector3.zero;
                            frieds[i, j] = true;
                          
                            Vector3 target = new Vector3(i * 0.6f, 0, j * 0.5f);
                            food.transform.localPosition = target;

                            friedQueue.Enqueue((food, new Vector2Int(i, j)));
                            StartCoroutine(DonutVitlity(food));
                            goto Escape;
                        }
                    }
                }

            }
        Escape: continue;
        }
        
    }

    IEnumerator DonutVitlity(Food food)
    {
        Vector3 v1 = food.transform.localScale;
        Vector3 v2 = new Vector3(1, 1, 1);
        Vector3 v3 = new Vector3(1.2f, 1.2f, 1.2f);
        float f = 0;
        while (f <= 0.2f)
        {
            food.transform.localScale = Vector3.Lerp(v1, v3, f * 5);
            f += Time.deltaTime;
            yield return null;
        }
        f = 0;
        while (f <= 0.1f)
        {
            food.transform.localScale = Vector3.Lerp(v3, v2, f * 10);
            f += Time.deltaTime;
            yield return null;
        }
        food.transform.localScale = v2;
    }


    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    public void FryDonut(float timer)
    {
        StartCoroutine(FryingDonut(timer));
    }
    public void Done()
    {
        StartCoroutine(CreateDonutDone());
    }

    IEnumerator FryingDonut(float timer)
    {
        Vector2 pos;
        (current, pos) = friedQueue.Peek();
        float f = 0;
        timer = timer / 6;
        Quaternion q1 = current.transform.rotation;
        Quaternion q2 = Quaternion.Euler(180, 0, 0);
        Quaternion q3 = Quaternion.Euler(360, 0, 0);

        for (int i = 0; i < 3; i++)
        {
            f = 0;
            while (f <= timer)
            {
                current.transform.rotation = Quaternion.Lerp(q1, q2, f * (1 / timer));

                f += Time.deltaTime;
                yield return null;
            }
            f = 0;
            while (f <= timer)
            {
                current.transform.rotation = Quaternion.Lerp(q2, q3, f * (1 / timer));

                f += Time.deltaTime;
                yield return null;
            }
            current.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    IEnumerator CreateDonutDone()
    {
       
        Vector2Int v;
        (current, v) = friedQueue.Dequeue();
        frieds[v.x, v.y] = false;
        current.transform.SetParent(FoodManager.foodCollects.transform);
        yield return CoroutneManager.waitForzerothree;

        createdFood = current;
        current = null;
        foodStack.foodStack.Push(createdFood);

        createdFood.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height, 2, 1, 0.4f);

    }
}
