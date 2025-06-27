using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerInfoPopup : MonoBehaviour
{
    public Sprite[] animalTextures;
    public Sprite[] foodTextures;
    public Sprite[] profileTextures;
    public Color[] backGlowColor;
    public Image animalFace;
    public Image likeFoodImg;
    public Image hateFoodImg;
    public Image profileBg;
    public Image backGlow;
    public int id;
  //  public string name;    // ���� �̸�
    public int tier;
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;

    public TextMeshProUGUI name_text;
    public TextMeshProUGUI speed_text;
    public TextMeshProUGUI eatSpeed_text;
    public TextMeshProUGUI order_text;
    public TextMeshProUGUI friendly;
    public TextMeshProUGUI personality;
    public Slider speedSlider;
    public Slider eatSpeedSlider;
    public Slider orderSlider;
    public TextMeshProUGUI speedSlider_text;
    public TextMeshProUGUI eatSpeedSlider_text;
    public TextMeshProUGUI orderSlider_text;
    StringBuilder personalityText;
    public void SetCustomerInfo()
    {
        animalFace.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[id].Name];// animalTextures[id];
        name_text.text = name;
        speed_text.text = speed.ToString();
        eatSpeed_text.text = eatSpeed.ToString();
        order_text.text = maxOrder.ToString();

        speedSlider.value = speed;
        eatSpeedSlider.value = eatSpeed;
        orderSlider.value = maxOrder;

        speedSlider_text.text = $"{speed}/20";
        eatSpeedSlider_text.text = $"{eatSpeed}/20";
        orderSlider_text.text = $"{maxOrder}/20";

        if (maxOrder < 5) order_text.text = "����";
        else if (maxOrder >= 5 && maxOrder < 10) order_text.text = "����";
        else if (maxOrder >= 10 && maxOrder < 15) order_text.text = "����";
        else if (maxOrder >= 15) order_text.text = "�ſ� ����";

        (int, List<int>) tier = AnimalManager.gatchaTiers[id];

        switch(tier.Item1)
        {
            case 1:
                friendly.text = "ȣ���";
                break;
            case 2:
                friendly.text = "����";
                break;
            case 3:
                friendly.text = "�ŷ�";
                break;
            case 4:
                friendly.text = "�ܰ�";
                break;
        }

        if (personalityText == null) personalityText = new StringBuilder();
        personalityText.Clear();
        int count = 0;
        for (int i = 0; i < tier.Item2.Count; i++)
        {
            if (tier.Item2[i] == 1)
            {
                count++;
                if(personalityText.Length > 0) personalityText.Append(", ");
                if (count == 5) if (personalityText.Length > 0) personalityText.Append("\n");
              
                personalityText.Append(AssetLoader.animalPersonalities[i].Name);
            }
        }
        personality.text = personalityText.ToString();  
       /* switch (likeFood)
        {
            case 1: likeFood_text.text = "�ܹ���"; likeFoodImg.sprite = foodTextures[1]; break;
            case 2: likeFood_text.text = "�ݶ�"; likeFoodImg.sprite = foodTextures[2]; break;
            case 3: likeFood_text.text = "Ŀ��"; likeFoodImg.sprite = foodTextures[3]; break;
            case 4: likeFood_text.text = "����"; likeFoodImg.sprite = foodTextures[4]; break;

            default: likeFood_text.text = "����"; likeFoodImg.sprite = foodTextures[0]; break;
        }

        switch (hateFood)
        {
            case 1: hateFood_text.text = "�ܹ���"; hateFoodImg.sprite = foodTextures[1]; break;
            case 2: hateFood_text.text = "�ݶ�"; hateFoodImg.sprite = foodTextures[2]; break;
            case 3: hateFood_text.text = "Ŀ��"; hateFoodImg.sprite = foodTextures[3]; break;
            case 4: hateFood_text.text = "����"; hateFoodImg.sprite = foodTextures[4]; break;

            default: hateFood_text.text = "����"; hateFoodImg.sprite = foodTextures[0]; break;
        }*/

        switch (tier.Item1)
        {
            case 1: profileBg.sprite = profileTextures[1]; backGlow.color = backGlowColor[1]; break;
            case 2: profileBg.sprite = profileTextures[2]; backGlow.color = backGlowColor[2]; break;
            case 3: profileBg.sprite = profileTextures[3]; backGlow.color = backGlowColor[3]; break;
            case 4: profileBg.sprite = profileTextures[4]; backGlow.color = backGlowColor[4]; break;

            default: profileBg.sprite = profileTextures[0]; backGlow.color = backGlowColor[0]; break;
        }
    }

    public void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }
}
