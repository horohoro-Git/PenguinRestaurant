using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour
{
    public Image cardBg;
    public Image backGlow;
    public Image glow;

    public int id;
    //public string name;    // 동물 이름
    public int tier;
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;

    public CustomerInfoPopup customerInfoPopup;
    public AnimalType animalType;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Card 오브젝트의 Image 가져오기
        cardBg = gameObject.GetComponent<Image>();

        // BackGlow 오브젝트의 Image 가져오기
        backGlow = transform.Find("Mask/BackGlow").GetComponent<Image>();

        // Glow 오브젝트의 Image 가져오기
        glow = transform.Find("Mask/Glow").GetComponent<Image>();
    }

    public void OnClick()
    {
        customerInfoPopup.gameObject.SetActive(true);

        customerInfoPopup.id = id;
        customerInfoPopup.name = name;
        customerInfoPopup.tier = tier;
        customerInfoPopup.speed = speed;
        customerInfoPopup.eatSpeed = eatSpeed;
        customerInfoPopup.minOrder = minOrder;
        customerInfoPopup.maxOrder = maxOrder;
        customerInfoPopup.likeFood = likeFood;
        customerInfoPopup.hateFood = hateFood;

        customerInfoPopup.SetCustomerInfo();
    }
}
