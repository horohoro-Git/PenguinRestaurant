using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static FoodMachine;

public class HamburgerPackaging : MonoBehaviour
{
    public Transform startPos; // 햄버거 출발 위치
    public Transform hambergerSetStartPos; // 햄버거 세트 출발 위치
    public GameObject box; // 박스 오브젝트
    public GameObject[] targetPositions; // 박스 안의 목표 위치 배열
    public GameObject a, b, c, d; // 순차적으로 활성화될 오브젝트들
    public GameObject hamburgerSetPrefab; // 햄버거 세트 프리팹
    public Transform destinationTrans; // 햄버거 세트의 최종 도착 위치
    public Mesh hamburgerSetMesh;
    public MachineType machineType;

    private PackingTable packingTable;
    private PackageFood packageBox;
    private Food f; // 햄버거
    public Action packingAction;

    private float moveSpeed = 0.2f;

    private int currentPositionIndex = 0;
    private int CurrentPackedNumber { get { return currentPositionIndex; } set { currentPositionIndex = value; packingTable.packingNumber = value; } }
  //  private bool allFilled = false;
    private bool isHamburgerSetMoving = false; // 세트 이동 중 여부를 확인할 변수


    float coroutineTimer = 0;
    private void Start()
    {
        packingTable = GetComponent<PackingTable>();
        PackageFood hamburgerSet = (PackageFood)FoodManager.GetPackageBox();
        hamburgerSet.parentType = MachineType.PackingTable;
        packageBox = hamburgerSet;
        packageBox.transform.position = packingTable.smallTable.position;
    }

    void Update()
    {
        // 필요에 따라 조건 확인
    }
    float elapsed = 0;
    public void StartMove(Food food, bool bStart)
    {
     
        StartCoroutine(MoveFood(food, false));
    }
    public IEnumerator MoveFood(Food food, bool bStart)
    {

        if (currentPositionIndex < targetPositions.Length && !isHamburgerSetMoving) // 플래그 확인
        {
            if (bStart)
            {
                elapsed = 0;
                coroutineTimer = 0;
            }
            //
            //f = food;
                    //   yield return StartCoroutine(MoveToPosition(f, targetPositions[currentPositionIndex].transform.position, moveSpeed));

            Vector3 startPosition = food.transform.position;
         
            while (elapsed < moveSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveSpeed;
                float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // 포물선 높이
                food.transform.position = Vector3.Lerp(startPosition, packageBox.packageTrans[currentPositionIndex].transform.position, t) + Vector3.up * height;
                yield return null;
            }

            food.transform.SetParent(packageBox.packageTrans[currentPositionIndex].transform);
            food.transform.localPosition = Vector3.zero;
            //FoodManager.EatFood(f);
            // obj.SetActive(false); // 도착 후 햄버거 비활성화

            // 도착 후 해당 위치에 따라 a, b, c, d 오브젝트 활성화
            currentPositionIndex++;
            ActivatePositionObject();
            packingAction = null;
            if (currentPositionIndex == 4)// targetPositions.Length)
            {
                //    allFilled = true;
         //       packingAction += () => { ResumeCreateHamburgerSet(); };
                yield return StartCoroutine(CreateHamburgerSet());
              //  Invoke("CreateHamburgerSet", moveSpeed); // 햄버거 세트 생성 지연
            }
        }
    }

   /* System.Collections.IEnumerator MoveToPosition(Food obj, Vector3 targetPosition, float duration)
    {
        Debug.Log("MoveToPosition");
        Vector3 startPosition = obj.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // 포물선 높이
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;
            yield return null;
        }

       // obj.transform.SetParent()

     //   FoodManager.EatFood(obj);
       // obj.SetActive(false); // 도착 후 햄버거 비활성화

        // 도착 후 해당 위치에 따라 a, b, c, d 오브젝트 활성화
        ActivatePositionObject();
    }*/

    void ActivatePositionObject()
    {
        if (currentPositionIndex - 1 == 0) a.SetActive(true);
        else if (currentPositionIndex - 1 == 1) b.SetActive(true);
        else if (currentPositionIndex - 1 == 2) c.SetActive(true);
        else if (currentPositionIndex - 1 == 3) d.SetActive(true);
    }

    public void ResumeCreateHamburgerSet()
    {
        StartCoroutine(CreateHamburgerSet());
    }
    IEnumerator CreateHamburgerSet()
    {
        while(coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }
      //  yield return new WaitForSeconds(0.2f);
        // a, b, c, d 오브젝트 및 박스 비활성화
        a.SetActive(false);
        b.SetActive(false);
        c.SetActive(false);
        d.SetActive(false);
        box.SetActive(false); // 박스 비활성화

        PackageFood pf = packageBox;
        PackageFood hamburgerSet = (PackageFood)FoodManager.GetPackageBox();
        hamburgerSet.parentType = MachineType.PackingTable;
        hamburgerSet.transform.position = packingTable.smallTable.position;
        packageBox = hamburgerSet;
        packingAction = null;

     //   packingAction += () => { ResumeMoveHamburgerSetToDestination(pf, destinationTrans.position, moveSpeed); };
        coroutineTimer = 0;
        yield return StartCoroutine(MoveHamburgerSetToDestination(pf, destinationTrans.position, moveSpeed));

        packingAction = null;
        // 다음 햄버거 세트 준비를 위한 변수 초기화
        currentPositionIndex = 0;
      //  allFilled = false;
    }

    public void ResumeMoveHamburgerSetToDestination(Food food, Vector3 targetPosition, float duration, bool destroy = false)
    {
        StartCoroutine(MoveHamburgerSetToDestination(food, targetPosition, duration, destroy));
    }
    System.Collections.IEnumerator MoveHamburgerSetToDestination(Food food, Vector3 targetPosition, float duration, bool destroy=false)
    {
        isHamburgerSetMoving = true; // 세트 이동 시작 시 플래그 활성화
        Vector3 startPosition = box.transform.position;

        // Count에 따라 targetPosition의 y 값 설정
        targetPosition = new Vector3(destinationTrans.position.x, destinationTrans.position.y + 0.63f * packingTable.packageStack.foodStack.Count, destinationTrans.position.z);

       // float elapsed = 0;

        while (coroutineTimer < duration)
        {
            coroutineTimer += Time.deltaTime;
            float t = coroutineTimer / duration;
            float height = 2 * (1 - Mathf.Pow(2 * t - 1, 2)); // 포물선 높이

            // 목표 위치로 포물선을 그리며 이동
            food.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        food.transform.position = targetPosition;
       
        // 스택에 food 추가
        packingTable.packageStack.foodStack.Push(food);
       // Debug.Log("Stack Count: " + packingTable.packageStack.foodStack.Count);

     //   if(destroy)FoodManager.EatFood(food);
        // 박스 활성화 및 플래그 비활성화
  //      box.SetActive(true);
        isHamburgerSetMoving = false;
        packingAction = null;
        coroutineTimer = 0;
        currentPositionIndex = 0;
        // packingAction = null;
    }


}
