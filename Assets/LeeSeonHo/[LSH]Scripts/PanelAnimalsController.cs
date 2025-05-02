using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimalsController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public AnimalCard[] animalCard;
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
            if(type.Value > 0)
            {
                AnimalStruct animal = AssetLoader.animals[type.Key];
                int index = type.Key - 100;
                animalCard[index].gameObject.SetActive(true);
                animalCard[index].id = index;
                animalCard[index].name = animal.name;
                animalCard[index].tier = type.Value;
                Debug.Log(type.Value);
                animalCard[index].speed = animal.speed;
                animalCard[index].eatSpeed = animal.eat_speed;//.eatSpeed;
                animalCard[index].minOrder = animal.min_order;
                animalCard[index].maxOrder = animal.max_order;
             /*   animalCard[index].cardBg.sprite = this.cardBg[type.Value - 1];
                animalCard[index].backGlow.color = this.backGlowColor[type.Value - 1];
                animalCard[index].glow.color = this.glowColor[type.Value - 1];*/
            }
        }
         
      /*  for(int i = 0; i < animalCard.Length; i++)
        {
            //HaveAnimal animal = playerAnimalDataManager.PlayerAnimal(i);


         *//*   AnimalStruct animal = GameInstance.GameIns.animalManager.animals
            if (animal.tier > 0)
            {
                animalCard[i].gameObject.SetActive(true);

                animalCard[i].id = animal.id;
                animalCard[i].name = animal.name;
                animalCard[i].tier = animal.tier;
                animalCard[i].speed = animal.speed;
                animalCard[i].eatSpeed = animal.eat_speed;//.eatSpeed;
                animalCard[i].minOrder = animal.min_order;
                animalCard[i].maxOrder = animal.max_order;
              //  animalCard[i].likeFood = animal.likeFood;
             //   animalCard[i].hateFood = animal.hateFood;

                for (int j = 0; j < 4; j++)
                {
                    if (animal.tier == j + 1)
                    {
                        animalCard[i].cardBg.sprite = this.cardBg[j];
                        animalCard[i].backGlow.color = this.backGlowColor[j];
                        animalCard[i].glow.color = this.glowColor[j];
                    }
                }
                
            }*//*
        }        */
    }


}
