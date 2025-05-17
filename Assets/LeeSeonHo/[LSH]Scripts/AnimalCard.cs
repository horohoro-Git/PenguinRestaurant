using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCard : MonoBehaviour
{
    public TMP_Text textName;
    public Image cardBg;
    public Image backGlow;
    public Image glow;
    public Image character;

    public int id;
    //public string name;    // 동물 이름
    public int tier;
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;

    [NonSerialized] public CustomerInfoPopup customerInfoPopup;
    public AnimalType animalType;

    // Start is called before the first frame update

    public void Awake()
    {
        textName.font = AssetLoader.font;
        textName.fontSharedMaterial = AssetLoader.font_mat;
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
