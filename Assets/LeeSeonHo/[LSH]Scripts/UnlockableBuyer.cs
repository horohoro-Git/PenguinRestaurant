using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnlockableBuyer : MonoBehaviour
{
    public MoneyManager MoneyManager;
    public RestaurantManager restaurantManager;
    public Image ProgressFill; // ProgressFill 이미지
    public TextMeshProUGUI PriceLabel; // PriceLabel 텍스트
    public float maxPrice = 10.00f; // 최대 가격
    public float fillSpeed = 1.0f; // Fill Amount 증가 속도

    private float fillAmount = 0f; // 현재 Fill Amount
    private float currentPrice; // 현재 가격
    private float firstBalance;
    private float number;
    private Coroutine fillCoroutine; // Fill Progress 코루틴

    public Vector3 blockExtends;
    bool unlock;
    void Start()
    {
      
       // firstBalance = MoneyManager.money;
        currentPrice = maxPrice; // 시작 가격 설정
        UpdatePriceLabel(); // 초기 가격 표시
    }

    void OnMouseDown()
    {
        // 마우스 버튼을 눌렀을 때 Fill Progress 코루틴 시작
      //  fillCoroutine = StartCoroutine(FillProgress());
        //inputManger.inOtherAction = true;

        
    }
    public void MouseDown()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }
        fillCoroutine = StartCoroutine(FillProgress());
        GameInstance.GameIns.inputManager.inOtherAction = true;

    }
    public void MouseUp()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        GameInstance.GameIns.inputManager.inOtherAction = false;
        GameInstance.GameIns.inputManager.manyFingers = true;
    }
    private void OnMouseOver()
    {
       // inputManger.inOtherAction = true;
        
    }
    private void OnMouseExit()
    {
      /*  if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        inputManger.inOtherAction = false;
        inputManger.manyFingers = true;*/
    }
    private void OnDisable()
    {
      //  GameInstance.GameIns.inputManager.inOtherAction = false;
        //GameInstance.GameIns.inputManager.manyFingers = true;
    }
    void OnMouseUp()
    {
        // 마우스 버튼을 놓았을 때 Fill Progress 코루틴 중지
      /*  if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
        inputManger.inOtherAction = false;
        inputManger.manyFingers = true;*/
        // inputManger.a = true;
    }

    IEnumerator FillProgress()
    {
     
        // 0.2초 대기
        yield return new WaitForSeconds(0.2f);

        while (fillAmount < 1f && GameInstance.GameIns.moneyManager.money > 0)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f); // Fill Amount 제한
            ProgressFill.fillAmount = fillAmount;

            // 가격 감소
            if (currentPrice > 0 && GameInstance.GameIns.moneyManager.money > 0)
            {
                float priceDecrease = maxPrice * Time.deltaTime; // maxPrice로 가격 감소

                // 가격과 현재 보유 금액 모두 감소
                if (GameInstance.GameIns.moneyManager.money >= priceDecrease)
                {
                    if (currentPrice >= priceDecrease)
                    {
                        currentPrice -= priceDecrease;
                        GameInstance.GameIns.moneyManager.money -= priceDecrease; // 보유 금액 감소    
                    }
                    else
                    {
                        GameInstance.GameIns.moneyManager.money -= currentPrice;
                        currentPrice = 0;

                    }
                                  
                }
                else
                {
                    if(currentPrice > GameInstance.GameIns.moneyManager.money)
                    {
                        currentPrice -= GameInstance.GameIns.moneyManager.money;
                        GameInstance.GameIns.moneyManager.money = 0;
                    }
                    else if(GameInstance.GameIns.moneyManager.money > currentPrice)
                    {
                        GameInstance.GameIns.moneyManager.money -= currentPrice;
                        currentPrice = 0;
                    }
                    else
                    {
                        currentPrice = 0;
                        GameInstance.GameIns.moneyManager.money = 0;
                    }
                   
                }
                UpdatePriceLabel(); // 가격 업데이트
            }
            yield return null; // 다음 프레임까지 대기
        }

        // Fill이 완료되면 추가 처리
        if (fillAmount >= 1f && unlock == false)
        {
            unlock = true;
            BoxCollider bc = GameInstance.GameIns.restaurantManager.levels[GameInstance.GameIns.restaurantManager.level].GetComponent<BoxCollider>();
            if (bc != null)
            {
                Collider[] colliders = Physics.OverlapBox(bc.transform.position + bc.center, bc.size, bc.transform.rotation, 1 << 10);

                for(int i=0; i<colliders.Length; i++)
                {
                    Animal animal = colliders[i].GetComponentInParent<Animal>();
                    AnimalController ac = animal.GetComponentInChildren<AnimalController>();
                    if (ac)
                    {
                      
                        Vector3 loc = ac.trans.position;
                        while(true)
                        {
                    
                            float randX = UnityEngine.Random.Range(3, 5);
                            float randY = UnityEngine.Random.Range(3, 5);
                            Vector3 newLoc = GameInstance.GetVector3(loc.x + randX, loc.y, loc.z + randY);
                            bool blockCheck = Physics.CheckBox(newLoc, GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7);
                            bool validCheck = Physics.CheckBox(newLoc, GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1);
                            if(!blockCheck && validCheck)
                            {
                                MoveObjectOutsideBuilding(ac.trans, bc);
                              //  ac.knockback = true;
                             //   ac.kockbackVector = GameInstance.GetVector3(randX, 0, randY);
                                break;

                            }
                        }

                    }
                }
                PurchaseItem();
                if (colliders.Length > 0) { Debug.Log(colliders.Length); }
                //if (checkAnimal || checkFlyingAnimal)
                //{
                //    Debug.Log("Animal");
                //}
             //   else PurchaseItem();
            }
            else
            {
                PurchaseItem();
            }
                    //    bool checkAnimal = Physics.CheckBox(bc.bounds.center, bc.bounds.size, bc.transform.rotation, 1 << 10);
                 //       if (!checkAnimal) PurchaseItem();
                     //   else Debug.Log("Animal Exists");
        }
    }
    void MoveObjectOutsideBuilding(Transform objTransform, BoxCollider bb)
    {    // 건물의 크기에서 X, Z 축을 추출
        Vector3 buildingSize = bb.size;

        Vector3 pushDirection = Vector3.zero;

        // 건물의 X, Z 크기에 맞게 객체를 밀어낼 방향 설정
        if (objTransform.position.x > bb.transform.position.x + bb.bounds.center.x + buildingSize.x / 2)
        {
            pushDirection.x = -1; // 건물 오른쪽에 있으면 왼쪽으로 밀어냄
        }
        else if (objTransform.position.x < bb.transform.position.x + bb.bounds.center.x - buildingSize.x / 2)
        {
            pushDirection.x = 1; // 건물 왼쪽에 있으면 오른쪽으로 밀어냄
        }
        else
        {
            pushDirection.x = 1;
        }

        if (objTransform.position.z > bb.transform.position.z + bb.bounds.center.z + buildingSize.z / 2)
        {
            pushDirection.z = -1; // 건물 뒤쪽에 있으면 앞쪽으로 밀어냄
        }
        else if (objTransform.position.z < bb.transform.position.z + bb.bounds.center.z - buildingSize.z / 2)
        {
            pushDirection.z = 1; // 건물 앞쪽에 있으면 뒤쪽으로 밀어냄
        }
        else
        {
            pushDirection.z = 1;
        }
        // 객체를 건물의 범위 밖으로 밀어냄
        if (pushDirection != Vector3.zero)
        {
            // 밀어낼 방향으로 일정 거리 이동
            objTransform.position += pushDirection.normalized * buildingSize.magnitude ;
            Collider[] walls = Physics.OverlapBox(objTransform.position, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 7);
            for (int i = 0; i < walls.Length; i++)
            {
                if(walls[i].gameObject.TryGetComponent(out NoGoArea noGoArea))
                {
                    objTransform.position += noGoArea.moveVector;
                }
            }

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
     //   Gizmos.DrawWireCube(transform.position, new Vector3(6.5f, 6, 4f));
    }

    void PurchaseItem()
    {
        GameInstance.GameIns.inputManager.inOtherAction = false;
        GameInstance.GameIns.inputManager.manyFingers = true;
        restaurantManager.LevelUp();
    }

    void UpdatePriceLabel()
    {
        if (currentPrice >= 1_000_000_000)
        {
            // 1_000_000_000 이상일 경우 B 단위로 표시
            PriceLabel.text = ((currentPrice) / 1_000_000_000).ToString("0.00") + "B";
        }
        else if (currentPrice >= 1_000_000)
        {
            // 1000000 이상일 경우 M 단위로 표시
            PriceLabel.text = ((currentPrice) / 1_000_000).ToString("0.00") + "M";
        }
        else if (currentPrice >= 1_000)
        {
            // 1000 이상일 경우 K 단위로 표시
            PriceLabel.text = ((currentPrice) / 1000).ToString("0.00") + "K";
        }
        else
        {
            // 1000 미만일 경우 일반 표시
            PriceLabel.text = (currentPrice).ToString("0");
        }
    }
}
