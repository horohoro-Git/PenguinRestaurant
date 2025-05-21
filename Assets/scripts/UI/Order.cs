using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//한글

public class Order : MonoBehaviour
{
    public Image foodImage1;
    public Image foodImage2;

    public GameObject secondFood;
    public TMP_Text foodNum1;
    public TMP_Text foodNum2;

    public AnimalController animalController;
    public Sprite[] sprites;

    // Start is called before the first frame update
    public void ShowOrder(MachineType type, int num, int count)
    {
        Vector3 p1 = GameInstance.GetVector3(0, 10, 0);
        transform.position = animalController.transform.position + p1;
       
        transform.rotation = Quaternion.Euler(60, 45, 0);
       
        if (count == 0)
        {
            Vector3 p2 = GameInstance.GetVector3(-0.8f, 0, 0);
            foodImage1.rectTransform.localPosition = p2;
            Vector3 p3 = GameInstance.GetVector3(0.95f, -0.2f, 0f);
            foodNum1.rectTransform.localPosition = p3;
          
            foodImage1.sprite = sprites[(int)type];
            foodNum1.text = $" {num}";
            foodImage2.gameObject.SetActive(false);
            foodNum2.gameObject.SetActive(false);
        }
        if (count == 1)
        {
            Vector3 p2 = GameInstance.GetVector3(-0.8f, -0.8f, 0);
            foodImage1.rectTransform.localPosition = p2;
            Vector3 p3 = GameInstance.GetVector3(0.95f, -1f, 0f);
            foodNum1.rectTransform.localPosition = p3;
            foodImage2.sprite = sprites[(int)type];
            foodNum2.text = $" {num}";
            foodImage2.gameObject.SetActive(true);
            foodNum2.gameObject.SetActive(true);
        }

  
    }
}
