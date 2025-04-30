using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class RewardsBox : MonoBehaviour
{
    public Transform transforms;
    public GameObject fishGO;
    public Stack<Food> foods = new Stack<Food>();
 //   public List<GameObject> rewards = new List<GameObject>();
    public Mesh meshFilter;

    [NonSerialized]
    public int boxIndex;
    private void Awake()
    {
        transforms = transform;
    }
    float fallSpeed = 0.5f;
    float lastTimer = 0f;
    bool removes=false;
    private void Update()
    {
        if (removes)
        {
            Vector3 testScale = GameInstance.GetVector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            //   transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            if (testScale.x < 0)
            {
                transform.localScale = GameInstance.GetVector3(2, 2, 2);
                //  removeCoroutine = null;
                fallSpeed = 0.5f;
                removes = false;
                if (GameInstance.GameIns.applianceUIManager.currentBox == this) GameInstance.GameIns.applianceUIManager.currentBox = null;
                GameInstance.GameIns.applianceUIManager.rewardsBoxes.Remove(this);
                FoodManager.RemoveRewardsBox(this);

            }
            else
            {
                transform.localScale = testScale;
            }
        }
     
        //  gameObject.SetActive(false);
       
    }


    public bool GetFish(bool touchdown = false)
    {
        bool result = false;
        if (lastTimer + fallSpeed < Time.time || touchdown)
        {

            lastTimer = Time.time;
            if (foods.Count < 30)
            {
                Food go = FoodManager.GetFood(meshFilter, MachineType.None, true);
                foods.Push(go);
                go.transform.localScale = GameInstance.GetVector3(2.5f, 2.5f, 2.5f);
                go.transform.rotation = Quaternion.Euler(90, 45, 0);
                go.transform.position = transform.position + Vector3.up * 10f;
                // StartCoroutine(DownFish());
                StartCoroutine(Falling(go));
                result = true;
            }
        }

        fallSpeed -= Time.deltaTime * 2f;
        if (fallSpeed < 0.1f) fallSpeed = 0.1f;

        return result;
    }

    public void StopFish()
    {
        fallSpeed = 0.5f;
    }

    Coroutine r;
    IEnumerator Falling(Food go)
    {
       
        float timer = 0;
        while(timer < 0.4f)
        {
            go.transform.Translate(Vector3.down * Time.deltaTime * 100f, Space.World);

            timer += Time.deltaTime;
            if(go.transform.position.y <= transform.position.y + 0.4f + 0.2f * foods.Count - 1)
            {
                go.transform.position = GameInstance.GetVector3(go.transform.position.x, transform.position.y + 0.4f + 0.2f * foods.Count, go.transform.position.z);

                if (r != null) StopCoroutine(r);
                 r = StartCoroutine(Rebound(go));
                break;
            }

            yield return null;
        }
    }

    IEnumerator Rebound(Food go)
    {
        float a = 2;
        while(true)
        {
            a+= Time.deltaTime * 10f;
            transform.localScale = GameInstance.GetVector3(a, 2, a);

            foreach(var item in foods)
            {
                item.transform.localScale = GameInstance.GetVector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
            }

         /*   for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i] != go)
                {
                    foods[i].transform.localScale = GameInstance.GetVector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
                }
            }*/

            yield return null;
            if (a > 2.2f)
            {
                a = 2.2f;
                break;
            }
        }


        while (true)
        {
            a -= Time.deltaTime*10;
            transform.localScale = GameInstance.GetVector3(a, 2, a);
            foreach (var item in foods)
            {
                item.transform.localScale = GameInstance.GetVector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
            }
           /* for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i] != go)
                {
                    foods[i].transform.localScale = GameInstance.GetVector3(2.5f + (a - 2) * 2, 2.5f, 2.5f + (a - 2) * 2);
                }
            }*/
            yield return null;
            if (a < 2)
            {
                a = 2f;
                transform.localScale = GameInstance.GetVector3(a, 2, a);
               /* for (int i = 0; i < foods.Count; i++)
                {
                    if (foods[i] != go)
                    {
                        foods[i].transform.localScale = GameInstance.GetVector3(2.5f, 2.5f, 2.5f);
                    }
                }*/
                foreach (var item in foods)
                {
                    item.transform.localScale = GameInstance.GetVector3(2.5f, 2.5f, 2.5f);
                }
                break;
            }
        }
    }

 //   Coroutine removeCoroutine;
    public bool ClearFishes()
    {
        if(foods.Count ==0)
        {
            removes = true;


           // if (removeCoroutine == null)
            {
              //  removeCoroutine = StartCoroutine(RemoveRewardBox());
                return true;
            }
        }
        return false;
    }

    public void EatFish(Food go)
    {
       // foods.RemoveAt(foods.Count - 1);
        FoodManager.EatFood(go, true);
    }
    public IEnumerator RemoveRewardBox()
    {
        while(true)
        {
            Vector3 testScale = GameInstance.GetVector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
         //   transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * 10f, transform.localScale.y - Time.deltaTime * 10f, transform.localScale.z - Time.deltaTime * 10f);
            if(testScale.x < 0)
            {
                transform.localScale = GameInstance.GetVector3(2, 2, 2);
                break;
            }
            else
            {
                transform.localScale = testScale;
            }

            yield return null;
        }
      //  gameObject.SetActive(false);
        //removeCoroutine = null;

        fallSpeed = 0.5f;
        if (GameInstance.GameIns.applianceUIManager.currentBox == this) GameInstance.GameIns.applianceUIManager.currentBox = null;
        GameInstance.GameIns.applianceUIManager.rewardsBoxes.Remove(this);
        FoodManager.RemoveRewardsBox(this);
    }
    public override int GetHashCode() => boxIndex.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj is not RewardsBox other) return false;
        return boxIndex == other.boxIndex;
    }
}
