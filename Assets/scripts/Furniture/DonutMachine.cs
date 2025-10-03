using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    CancellationTokenSource cancellationTokenSource;
    // Start is called before the first frame update
    public override void Start()
    {
      
        base.Start();
        height = 0.7f;
    //    donutTrans.rotation = transforms.rotation;
       
    }
    public override void OnEnable()
    {
        if(!placed) return;
        base.OnEnable();
        frieds = new bool[3, 2];
        while (friedQueue.Count > 0)
        {
            Food f;
            Vector2Int a;
            (f, a) = friedQueue.Dequeue();
            FoodManager.EatFood(f);
        }
        cancellationTokenSource = new CancellationTokenSource();
        AddDonuts(cancellationTokenSource.Token).Forget();
      
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
        audioSource.Stop();
        if(cancellationTokenSource != null) cancellationTokenSource.Cancel();
    }
    async UniTask AddDonuts(CancellationToken cancellationToken)
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
            DonutVitlity(food, cancellationToken).Forget();
          //  cancellationToken.ThrowIfCancellationRequested();
            //   StartCoroutine(DonutVitlity(food));
            //  yield return CoroutneManager.waitForzeroone;
            //  yield return StartCoroutine(Utility.CustomCoroutineDelay(0.1f));
            await Utility.CustomDelayTask(0.1f, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        while (true)
        {
            //  yield return CoroutneManager.waitFortwo;
            //  yield return StartCoroutine(Utility.CustomCoroutineDelay(2f));
            await Utility.CustomDelayTask(2f, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (friedQueue.Count < 6)
            {
                if (friedQueue.Count >= 2) await Utility.CustomDelayTask(2f, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();//yield return CoroutneManager.waitFortwo;
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
                            DonutVitlity(food, cancellationToken).Forget();
                            goto Escape;
                        }
                    }
                }

            }
        Escape: continue;
        }
        
    }

    async UniTask DonutVitlity(Food food, CancellationToken cancellationToken)
    {
        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.CreateFood(), 0.4f, 100, 5, food.transform.position);
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
            await UniTask.NextFrame(cancellationToken);
        }
        f = 0;
        while (f <= 0.1f)
        {
            if (App.restaurantTimeScale == 1)
            {
                food.transform.localScale = Vector3.Lerp(v3, v2, f * 10);
                f += Time.deltaTime;
            }
            await UniTask.NextFrame(cancellationToken);
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


    public void FryDonut(float timer, CancellationToken cancellationToken)
    {
     //   StartCoroutine(FryingDonut(timer));
        FryingDonut(timer, cancellationToken).Forget();
    }
    public void Done(CancellationToken cancellationToken)
    {
        audioSource.Stop();
       // StartCoroutine(CreateDonutDone());
        CreateDonutDone(cancellationToken).Forget();
    }

    async UniTask FryingDonut(float timer, CancellationToken cancellationToken)
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
                if (App.restaurantTimeScale == 1)
                {
                    current.transform.rotation = Quaternion.Lerp(q1, q2, f * (1 / timer));

                    f += Time.deltaTime;
                }
                await UniTask.NextFrame(cancellationToken);
            }
            f = 0;
            while (f <= timer)
            {
                if (App.restaurantTimeScale == 1)
                {
                    current.transform.rotation = Quaternion.Lerp(q2, q3, f * (1 / timer));

                    f += Time.deltaTime;
                }
                await UniTask.NextFrame(cancellationToken);
            }
            current.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    async UniTask CreateDonutDone(CancellationToken cancellationToken)
    {
       
        Vector2Int v;
        (current, v) = friedQueue.Dequeue();
        frieds[v.x, v.y] = false;
        current.transform.SetParent(WorkSpaceManager.foodCollects.transform);
        //   yield return CoroutneManager.waitForzerothree;
        //yield return StartCoroutine(Utility.CustomCoroutineDelay(0.3f));
        await Utility.CustomDelayTask(0.3f, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        createdFood = current;
        CreatedFood(createdFood);
        current = null;
        foodStack.foodStack.Push(createdFood);

        createdFood.transform.DOJump(foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height, 2, 1, 0.4f);

    }
}
