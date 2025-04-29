using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FoodMachine;

public class UIUpGradeView : MonoBehaviour
{
    public GameObject contents;
    public GameObject uiListPrefab;
    public int boxNumber = 10; // 시작할때 생성되는 박스 갯수
    public float y = -30f; // 박스와 박스의 y축 간격 차이
    public float initialYPosition = 130f;  // 첫번째 프리팩 박스 시작 높이 위치

    // Start is called before the first frame update
    void Start()
    {
        boxPos();
    }

    void boxPos()
    {
        int foodMachinesCount = GameInstance.GameIns.workSpaceManager.foodMachines.Count;
        for (int i = 0; i < foodMachinesCount; i++)
        {
            //Instantiate<GameObject>(this.uiListPrefab, contents.transform);
            // 첫 프리팹 위치 계산
            float yPos = initialYPosition + (y * i);
            // 새로운 위치 계산
            Vector3 newPosition = new Vector3(contents.transform.position.x, yPos, contents.transform.position.z);
            // 프리팹 인스턴스 생성 및 위치 설정
            GameObject newUI = Instantiate<GameObject>(this.uiListPrefab, newPosition, Quaternion.identity, contents.transform);
            // 부모 자식 관계 설정
            newUI.transform.SetParent(contents.transform, false);            
        }
    }
}
