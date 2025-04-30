
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Table : MonoBehaviour
{
    //  public Transform Transforms;
    public Transform Transforms;
    public bool isDirty = false;
    public AnimalController controller;
    public AnimalController employeeContoller;
  
    public List<FoodStack>foodStacks = new List<FoodStack>();
    //public Stack<Garbage> garbages = new Stack<Garbage>();
    public List<Garbage> garbageList = new List<Garbage>();
    public Garbage garbageInstance;
    public Transform animalSeat;
    public Transform cleanSeat;
    public Transform up;
    public int numberOfGarbage;
    public Seat[] seats;
    public Plate trashPlate;
    public Transform plateLoc;
    public bool interacting;
    public int seatNum;
    GameInstance gameInstance = new GameInstance();

    public float weight;
    public float height;
    public void Awake()
    {
        //transforms = transform;
        // plateLoc = trashPlate.gameObject.transform.position;
    }
    private void Start()
    {
       
        plateLoc = trashPlate.transforms;
        foodStacks.Add(new FoodStack());
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
      
        StartCoroutine(Clean());
    }
    public Transform transforms { 
        
        get {
            if (Transforms == null) Transforms = transform;    
            return Transforms; 
        } }
    IEnumerator Clean()
    {
        while (garbageList.Count > 0)
        {
            Garbage g = garbageList[garbageList.Count - 1];
            garbageList.Remove(g);
            numberOfGarbage--;
            StartCoroutine(Rising(g));
          //  GarbageManager.ClearGarbage(g);
            yield return null;
        }
        interacting = false;
        isDirty = false;
        /* GameInstance instance = new GameInstance();
         TrashCan tc = instance.GameIns.workSpaceManager.trashCans[0];
         //     instance.GameIns.inputManager.inOtherAction = true;

         while (garbageList.Count > 0)
         {
             Garbage g = garbageList[garbageList.Count - 1];
             garbageList.Remove(g);

             numberOfGarbage--;
             // trashPlate.transform.DO
             g.transform.DOJump(tc.transform.position, 2, 1, 0.2f).OnComplete(() =>
             {
                 Destroy(g.gameObject);
                 // GarbageManager.ClearGarbage(g);
             });


             yield return new WaitForSecondsRealtime(0.2f);
         }

         interacting = false;
         isDirty = false;
         trashPlate.transform.position = new Vector3(transform.position.x, transform.position.y + 0.44f, transform.position.z);*/
        //  yield return StartCoroutine(instance.GameIns.inputManager.BecomeToOrgin());
        //      instance.GameIns.inputManager.inOtherAction = false;
    }
    IEnumerator Rising(Garbage go)
    {
     
        bool bstart = true;
        float elapsedTime = 0;
        Vector3 startPoint = go.transforms.position;
        Vector3 endPoint = Camera.main.ScreenToWorldPoint(GameInstance.GameIns.applianceUIManager.rewardChest.transform.position);
    
        Vector3 controlVector = (startPoint + endPoint) / weight + Vector3.up * height;

        while (bstart)
        {
            endPoint = Camera.main.ScreenToWorldPoint(GameInstance.GameIns.applianceUIManager.rewardChest.transform.position);
            controlVector = (startPoint + endPoint) / weight + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 0.5f);

            Vector3 origin = go.transforms.position;
            // Vector3 targetLoc = Vector3.Lerp(startPoint, endPoint, t);// CalculateBezierPoint(t, startPoint, controlVector, endPoint);
            Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint);

            Vector3 dir = targetLoc - origin;
           // float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
           // modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Debug.DrawLine(go.transforms.position, targetLoc, Color.red, 5f);
            go.transforms.position = targetLoc;


            if (t >= 1.0f)
            {
                go.transforms.position = endPoint;
                GarbageManager.ClearGarbage(go);
                bstart = false;                           
            }

            yield return null;
        }
        yield return null;

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
