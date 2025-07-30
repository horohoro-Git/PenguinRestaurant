using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static App;
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
  //  public string name;    // 동물 이름
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
        eatSpeed_text.text = (8.5f - eatSpeed).ToString();
        order_text.text = maxOrder.ToString();

        speedSlider.value = (speed /6f);
        eatSpeedSlider.value = ((8.5f - eatSpeed) / 5f);
        orderSlider.value = (maxOrder / 6f);

        /*speedSlider_text.text = $"{speed}/10";
        eatSpeedSlider_text.text = $"{eatSpeed}/20";
        orderSlider_text.text = $"{maxOrder}/20";*/

        if (maxOrder < 3) order_text.text = gameSettings.language == Language.KOR ? "적음" : "Low";
        else if (maxOrder >= 3 && maxOrder < 4) order_text.text = gameSettings.language == Language.KOR ? "보통" : "Normal";
        else if (maxOrder >= 4 && maxOrder < 5) order_text.text = gameSettings.language == Language.KOR ? "많음" : "High";
        else if (maxOrder >= 5) order_text.text = gameSettings.language == Language.KOR ? "매우 많음" : "Very High";

        (int, List<int>) tier = AnimalManager.gatchaTiers[id];

        switch(tier.Item1)
        {
            case 1:
                friendly.text = gameSettings.language == Language.KOR ? "호기심" : "Curious";
                break;
            case 2:
                friendly.text = gameSettings.language == Language.KOR ? "애정" : "Fond";
                break;
            case 3:
                friendly.text = gameSettings.language == Language.KOR ? "신뢰" : "Trusting";
                break;
            case 4:
                friendly.text = gameSettings.language == Language.KOR ? "단골" : "Regular";
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

                string n = gameSettings.language == Language.KOR ? AssetLoader.animalPersonalities[i].name_kor : AssetLoader.animalPersonalities[i].name_eng;
                personalityText.Append(n);
            }
        }
        personality.text = personalityText.ToString();  
      
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
