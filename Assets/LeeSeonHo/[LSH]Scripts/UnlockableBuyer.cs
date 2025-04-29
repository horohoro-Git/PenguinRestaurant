using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnlockableBuyer : MonoBehaviour
{
    public MoneyManager MoneyManager;
    public RestaurantManager restaurantManager;
    public Image ProgressFill; // ProgressFill �̹���
    public TextMeshProUGUI PriceLabel; // PriceLabel �ؽ�Ʈ
    public float maxPrice = 10.00f; // �ִ� ����
    public float fillSpeed = 1.0f; // Fill Amount ���� �ӵ�

    private float fillAmount = 0f; // ���� Fill Amount
    private float currentPrice; // ���� ����
    private float firstBalance;
    private float number;
    private Coroutine fillCoroutine; // Fill Progress �ڷ�ƾ

    public Vector3 blockExtends;
    bool unlock;
    void Start()
    {
      
       // firstBalance = MoneyManager.money;
        currentPrice = maxPrice; // ���� ���� ����
        UpdatePriceLabel(); // �ʱ� ���� ǥ��
    }

    void OnMouseDown()
    {
        // ���콺 ��ư�� ������ �� Fill Progress �ڷ�ƾ ����
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
        // ���콺 ��ư�� ������ �� Fill Progress �ڷ�ƾ ����
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
     
        // 0.2�� ���
        yield return new WaitForSeconds(0.2f);

        while (fillAmount < 1f && GameInstance.GameIns.moneyManager.money > 0)
        {
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1f); // Fill Amount ����
            ProgressFill.fillAmount = fillAmount;

            // ���� ����
            if (currentPrice > 0 && GameInstance.GameIns.moneyManager.money > 0)
            {
                float priceDecrease = maxPrice * Time.deltaTime; // maxPrice�� ���� ����

                // ���ݰ� ���� ���� �ݾ� ��� ����
                if (GameInstance.GameIns.moneyManager.money >= priceDecrease)
                {
                    if (currentPrice >= priceDecrease)
                    {
                        currentPrice -= priceDecrease;
                        GameInstance.GameIns.moneyManager.money -= priceDecrease; // ���� �ݾ� ����    
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
                UpdatePriceLabel(); // ���� ������Ʈ
            }
            yield return null; // ���� �����ӱ��� ���
        }

        // Fill�� �Ϸ�Ǹ� �߰� ó��
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
    {    // �ǹ��� ũ�⿡�� X, Z ���� ����
        Vector3 buildingSize = bb.size;

        Vector3 pushDirection = Vector3.zero;

        // �ǹ��� X, Z ũ�⿡ �°� ��ü�� �о ���� ����
        if (objTransform.position.x > bb.transform.position.x + bb.bounds.center.x + buildingSize.x / 2)
        {
            pushDirection.x = -1; // �ǹ� �����ʿ� ������ �������� �о
        }
        else if (objTransform.position.x < bb.transform.position.x + bb.bounds.center.x - buildingSize.x / 2)
        {
            pushDirection.x = 1; // �ǹ� ���ʿ� ������ ���������� �о
        }
        else
        {
            pushDirection.x = 1;
        }

        if (objTransform.position.z > bb.transform.position.z + bb.bounds.center.z + buildingSize.z / 2)
        {
            pushDirection.z = -1; // �ǹ� ���ʿ� ������ �������� �о
        }
        else if (objTransform.position.z < bb.transform.position.z + bb.bounds.center.z - buildingSize.z / 2)
        {
            pushDirection.z = 1; // �ǹ� ���ʿ� ������ �������� �о
        }
        else
        {
            pushDirection.z = 1;
        }
        // ��ü�� �ǹ��� ���� ������ �о
        if (pushDirection != Vector3.zero)
        {
            // �о �������� ���� �Ÿ� �̵�
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
            // 1_000_000_000 �̻��� ��� B ������ ǥ��
            PriceLabel.text = ((currentPrice) / 1_000_000_000).ToString("0.00") + "B";
        }
        else if (currentPrice >= 1_000_000)
        {
            // 1000000 �̻��� ��� M ������ ǥ��
            PriceLabel.text = ((currentPrice) / 1_000_000).ToString("0.00") + "M";
        }
        else if (currentPrice >= 1_000)
        {
            // 1000 �̻��� ��� K ������ ǥ��
            PriceLabel.text = ((currentPrice) / 1000).ToString("0.00") + "K";
        }
        else
        {
            // 1000 �̸��� ��� �Ϲ� ǥ��
            PriceLabel.text = (currentPrice).ToString("0");
        }
    }
}
