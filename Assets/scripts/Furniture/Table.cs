
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;

public class Table : Furniture
{
    //  public Transform Transforms;
    public Transform Transforms;
    [NonSerialized] public bool isDirty = false;
    public AnimalController controller;
    [NonSerialized] public AnimalController employeeContoller;

    [NonSerialized] public List<FoodStack>foodStacks = new List<FoodStack>();
    //public Stack<Garbage> garbages = new Stack<Garbage>();
    [NonSerialized] public List<Garbage> garbageList = new List<Garbage>();
    public Garbage garbageInstance;
    public Transform animalSeat;
    public Transform cleanSeat;
    public Transform up;
    [NonSerialized] public int numberOfGarbage;
    [NonSerialized] public int numberOfFoods;
    public Seat[] seats;
    public Plate trashPlate;
    public Transform plateLoc;
    [NonSerialized] public bool interacting;
    public int seatNum;

    public float weight;
    public float height;

    [NonSerialized] public bool hasProblem;
    [NonSerialized] public bool stealing;
    [NonSerialized] public bool stolen;
    [NonSerialized] public GameObject[] placedFoods = new GameObject[4];
    public void Awake()
    {
        //transforms = transform;
        // plateLoc = trashPlate.gameObject.transform.position;
    }
    public override void Start()
    {
        weight = 2;
        height = 40;
        plateLoc = trashPlate.transforms;
        foodStacks.Add(new FoodStack());

        GameInstance.GameIns.workSpaceManager.unlockTable = true;
        GameInstance.GameIns.workSpaceManager.tables.Add(this);
        base.Start();
        //test
        /* numberOfGarbage = 8;
         for (int i = 0; i < numberOfGarbage; i++)
         {
             Garbage go = GarbageManager.CreateGarbage();
             go.transforms.SetParent(trashPlate.transforms);
             garbageList.Add(go);

             float x = UnityEngine.Random.Range(-1f, 1f);
             float z = UnityEngine.Random.Range(-1f, 1f);
             go.transforms.position = up.position + GameInstance.GetVector3(x, 0, z);
         }
         interacting = false;
         isDirty = true;*/
    }

    private void OnMouseEnter()
    {
        if (isDirty)
        {
           
            GameInstance.GameIns.inputManager.clickedTable = this;
        }
    }

    public void CleanTableManually()
    {
        if(employeeContoller != null) employeeContoller.reCalculate = true;
        employeeContoller = null;
        Clean(App.GlobalToken).Forget();
        
       //StartCoroutine(Clean());
    }
    public Transform transforms { 
        
        get {
            if (Transforms == null) Transforms = transform;    
            return Transforms; 
        } }

    async UniTask Clean(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            interacting = true;

            List<UniTask> tasks = new List<UniTask>();

            while (garbageList.Count > 0)
            {
                Garbage g = garbageList[garbageList.Count - 1];
                garbageList.Remove(g);
                numberOfGarbage--;
                tasks.Add(Rising(g, App.GlobalToken));
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            await UniTask.WhenAll(tasks);
            isDirty = false;
            interacting = false;
            canTouchable = true;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Clean task was cancelled");
            throw; 
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Clean: {ex.Message}");
            throw;
        }
    }

    async UniTask Rising(Garbage go, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool bstart = true;
            float elapsedTime = 0;
            Vector3 startPoint = go.transforms.position;
            Vector3 screenTarget = RectTransformUtility.WorldToScreenPoint(
            InputManger.cachingCamera,
            GameInstance.GameIns.applianceUIManager.rewardChest.transform.position
            );

           // Vector3 endPoint = InputManger.cachingCamera.ScreenToWorldPoint(GameInstance.GameIns.applianceUIManager.rewardChest.transform.position);
            Vector3 endPoint = InputManger.cachingCamera.ScreenToWorldPoint(screenTarget);
            Vector3 controlVector = (startPoint + endPoint) / weight + Vector3.up * height;

            while (bstart)
            {
                endPoint = InputManger.cachingCamera.ScreenToWorldPoint(GameInstance.GameIns.applianceUIManager.rewardChest.transform.position);
                controlVector = (startPoint + endPoint) / weight + Vector3.up * height;
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / 0.5f);

                Vector3 origin = go.transforms.position;
                Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint);

                Vector3 dir = targetLoc - origin;
                Debug.DrawLine(go.transforms.position, targetLoc, Color.red, 5f);
                go.transforms.position = targetLoc;

                if (t >= 1.0f)
                {
                    go.transforms.position = endPoint;
                    GarbageManager.ClearGarbage(go);
                    bstart = false;
                }

                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            int r = Random.Range(1, 4);
            int prev = GameInstance.GameIns.restaurantManager.trashData.trashPoint;
            GameInstance.GameIns.restaurantManager.trashData.trashPoint += r;
            if (GameInstance.GameIns.restaurantManager.trashData.trashPoint > 100)
            {
                GameInstance.GameIns.restaurantManager.trashData.trashPoint = 100;
                if(prev < 100) GameInstance.GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
            }
            else
            {
                if (GameInstance.GameIns.restaurantManager.trashData.trashPoint == 100)
                {
                    GameInstance.GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
                }
                else
                {
                    GameInstance.GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(false);
                }
            }

            GameInstance.GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = GameInstance.GameIns.restaurantManager.trashData.trashPoint * 0.01f;
            GameInstance.GameIns.restaurantManager.trashData.changed = true;
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Rising task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Rising: {ex.Message}");
            throw;
        }
    }
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // 시작점
        point += 2 * u * t * p1; // 제어점
        point += tt * p2; // 끝점

        return point;
    }
}
