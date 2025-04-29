using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WorkSpace : MonoBehaviour
{
   /* public enum WorkType
    {
        Counter,
        Kitchen,
        Table
    }

    public MeshRenderer meshRenderer;
    public WorkType workType;
    public Transform counterTrans;
    public Transform foodTrans;
    public Transform foodCustomerTrans;
    public Food food;
    public Stack<Food> foodStack = new Stack<Food>();
    public Animal employee;
    public Customer customer;
    float lastTime = 0f;
    public float bet = 0.4f;
    public float distance; 
    
    void Update()
    {
        if (workType == WorkType.Kitchen)
        {
            if (foodStack.Count < 5)
            {
                if(lastTime + bet <= Time.time)
                {
              //      Food f = FoodManager.GetFood(meshRenderer); // Instantiate(food, foodTrans);
           //         f.transform.position = new Vector3(foodTrans.position.x, foodTrans.position.y + foodStack.Count * distance, foodTrans.position.z);
               //     foodStack.Push(f);
                    lastTime = Time.time;
                }
            }
            else lastTime = Time.time;
        }
    }

    public IEnumerator GetFoods(Animal animal)
    {
        int i = 0;
        while (foodStack.Count > 0)
        {
            Food f = foodStack.Pop();
            float r = Random.Range(1f, 2.5f);
            Vector3 loc =  new Vector3(animal.head.position.x, animal.head.position.y + i * 0.7f, animal.head.position.z);
            f.transform.DOJump(loc, r, 1, 0.2f).OnComplete(() =>
            {
                f.transform.position = loc;
                animal.foodList.Add(f);
            });
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }

    public IEnumerator GiveFoods(Animal animal)
    {
        int c = animal.foodList.Count - 1;
        for (int i = c; i >= 0; i--)   
        {
            Food f = animal.foodList[i];
            float r = Random.Range(1f, 2.5f);
            Vector3 loc = foodTrans.position + new Vector3(0, foodStack.Count * 0.7f, 0);
            f.transform.DOJump(loc, r, 1, 0.2f).OnComplete(() =>
            {
                Vector3 locc = foodTrans.position + new Vector3(0, foodStack.Count * 0.7f, 0);
                f.transform.position = locc;
                foodStack.Push(f);
            });

            animal.foodList.RemoveAt(i);
            yield return new WaitForSeconds(0.1f);
        }

    }

    public IEnumerator GetFoodsCustomer(Customer animal)
    {
        customer = animal;
        int i = 0;
        while (true)
        {
           
//          Debug.Log(foodStack.Count);
            if (foodStack.Count > 0 && employee)
            {
                if (i == animal.wantNum)
                {
                    animal.state = Customer.CustomerState.WalkToTable;
                    break;
                }
                Food f = foodStack.Pop();
                float r = Random.Range(1f, 2.5f);
                Vector3 loc = new Vector3(animal.head.position.x, animal.head.position.y + i * 0.7f, animal.head.position.z);
                f.transform.DOJump(loc, r, 1, 0.2f).OnComplete(() =>
                {
                    f.transform.position = loc;
                    animal.foodList.Add(f);
                });
                yield return new WaitForSeconds(0.1f);
                i++;
            }

            yield return null;
        }

        
    }

    public IEnumerator EatingFoodsCustomer(Customer animal)
    {
        Vector3 vv = transform.position - animal.transform.position;
        float angle = Mathf.Atan2(vv.x, vv.z) * Mathf.Rad2Deg;

        animal.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        int c = animal.foodList.Count - 1;
        for (int i = c; i >= 0; i--)
        {
            Food f = animal.foodList[i];
            float r = Random.Range(1f, 2.5f);
            Vector3 loc = transform.position + new Vector3(0, foodStack.Count * 0.7f, 0);
            f.transform.DOJump(loc, r, 1, 0.2f).OnComplete(() =>
            {
                Vector3 locc = transform.position + new Vector3(0, foodStack.Count * 0.7f, 0);
                f.transform.position = locc;
                foodStack.Push(f);
            });

            animal.foodList.RemoveAt(i);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        int e = 0;
        while(true)
        {

            if(foodStack.Count>0)
            {
                e++;
                animal.Eat();
                
                Food food = foodStack.Pop();
                FoodManager.EatFood(food);
                food.gameObject.SetActive(false);
                if (e == animal.wantNum) break;
                yield return new WaitForSeconds(0.417f);
            }

            yield return null;
        }
    }
*/
}
