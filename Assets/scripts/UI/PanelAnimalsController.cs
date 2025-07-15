using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimalsController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public AnimalCard animalCard;
    Dictionary<int, AnimalCard> cardDic = new Dictionary<int, AnimalCard>();
    [SerializeField] RectTransform contents;
    [SerializeField] CustomerInfoPopup customers_popup;
//   public AnimalCard[] animalCard;
    public Sprite[] cardBg;
    public Color[] backGlowColor;
    public Color[] glowColor;
   
    private PlayerAnimalDataManager playerAnimalDataManager;

    private void OnEnable()
    {
        playerAnimalDataManager = GameInstance.GameIns.playerAnimalDataManager;

        CardUpdate();

        scrollRect.normalizedPosition = new Vector2(0, 0);
    }


    // Update is called once per frame
    public void CardUpdate()
    {
        foreach (var type in AnimalManager.gatchaTiers)
        {
            (int, List<int>) val = type.Value;
            if(val.Item1 > 0)
            {
                AnimalStruct animal = AssetLoader.animals[type.Key];
                int index = type.Key - 100;
                if (cardDic.ContainsKey(index))
                {
                    //키드 업데이트
                    cardDic[index].tier = val.Item1;
                    cardDic[index].speed = animal.speed;
                    cardDic[index].eatSpeed = animal.eat_speed;
                    cardDic[index].minOrder = animal.min_order;
                    cardDic[index].maxOrder = animal.max_order;
                    cardDic[index].cardBg.sprite = cardBg[val.Item1 - 1];
                    cardDic[index].backGlow.color = backGlowColor[val.Item1 - 1];
                    cardDic[index].glow.color = glowColor[val.Item1 - 1];
                }
                else
                {
                    //카드 추가
                    AnimalCard card = Instantiate(animalCard);
                    card.GetComponent<RectTransform>().SetParent(contents);
                    card.id = animal.id;
                    card.character.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[type.Key].Name];
                    card.customerInfoPopup = customers_popup;
                    card.name = App.gameSettings.language == Language.KOR ? animal.name_kor : animal.name_eng;
                    card.textName.text = App.gameSettings.language == Language.KOR ? animal.name_kor : animal.name_eng;
                    card.tier = val.Item1;
                    card.speed = animal.speed;
                    card.eatSpeed = animal.eat_speed;
                    card.minOrder = animal.min_order;
                    card.maxOrder = animal.max_order;
                    card.cardBg.sprite = cardBg[val.Item1 - 1];
                    card.backGlow.color = backGlowColor[val.Item1 - 1];
                    card.glow.color = glowColor[val.Item1 - 1];
                    cardDic[index] = card;
                }
            }
        }
        
    }


}
