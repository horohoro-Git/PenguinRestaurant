using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimalsController : MonoBehaviour
{
    public GameObject bg;
    public RectTransform scrollRoot;
    public StageGuidePanel stageGuidePanel;
    public AnimalCard animalCard;
    Dictionary<int, Dictionary<int, AnimalCard>> cardDic = new();
    List<StageGuidePanel> guidePanels = new();
    [SerializeField] RectTransform contents;
    [SerializeField] CustomerInfoPopup customers_popup;
//   public AnimalCard[] animalCard;
    public Sprite[] cardBg;
    public Color[] backGlowColor;
    public Color[] glowColor;
   
    private PlayerAnimalDataManager playerAnimalDataManager;
    bool setup;
    private void Awake()
    {
        GameInstance.GameIns.panelAnimalsController = this;
       // bg.gameObject.SetActive(false);
       // scrollRect.gameObject.SetActive(true);
    }
    private void Start()
    {
        playerAnimalDataManager = GameInstance.GameIns.playerAnimalDataManager;
        //    setup = true;
        // CardUpdate(true);
    }
    private void OnEnable()
    {
       /* if (setup)
        {
           

            CardUpdate(true);
        }
        else setup = true;
       // scrollRect.normalizedPosition = new Vector2(0, 0);*/
    }


    // Update is called once per frame
    public void CardUpdate(bool init)
    {
        HashSet<int> newIDs = new();
        for (int i = 0; i <= App.gameSettings.clearStage; i++)
        {
            if (i >= guidePanels.Count)
            {
                StageGuidePanel stagePanel = Instantiate(stageGuidePanel);
                guidePanels.Add(stagePanel);
                stagePanel.transform.SetParent(contents);
                stagePanel.id = i;
            }

            int[] animal_keys = AssetLoader.maps[i].animals_id.Split(',').Select(int.Parse).ToArray();

            if (!cardDic.ContainsKey(i))
            {
                cardDic[i] = new();
            }
            for (int j = 0; j < animal_keys.Length; j++)
            {
                if (AnimalManager.gatchaTiers.ContainsKey(animal_keys[j]))
                {
                    (int, List<int>, bool) val = AnimalManager.gatchaTiers[animal_keys[j]];

                    if (val.Item1 > 0)
                    {
                        AnimalStruct animal = AssetLoader.animals[animal_keys[j]];
                        //int index = animal_keys[j] - 100;
                       
                        if (cardDic[i].ContainsKey(animal_keys[j]))
                        {
                            if (!init)
                            {
                                if (cardDic[i][animal_keys[j]].tier != val.Item1)
                                {
                                    cardDic[i][animal_keys[j]].tier = val.Item1;
                                }
                            }
                            else
                            {
                                cardDic[i][animal_keys[j]].tier = val.Item1;
                            }
                            cardDic[i][animal_keys[j]].speed = animal.speed;
                            cardDic[i][animal_keys[j]].eatSpeed = animal.eat_speed;
                            cardDic[i][animal_keys[j]].minOrder = animal.min_order;
                            cardDic[i][animal_keys[j]].maxOrder = animal.max_order;
                            cardDic[i][animal_keys[j]].cardBg.sprite = cardBg[val.Item1 - 1];
                            cardDic[i][animal_keys[j]].backGlow.color = backGlowColor[val.Item1 - 1];
                            cardDic[i][animal_keys[j]].glow.color = glowColor[val.Item1 - 1];

                            cardDic[i][animal_keys[j]].newCardText.gameObject.SetActive(val.Item3);
                        }
                        else
                        {
                            AnimalCard card = Instantiate(animalCard, guidePanels[i].content);
                            card.transform.SetSiblingIndex(j);
                         //   card.GetComponent<RectTransform>().SetParent(guidePanels[i].transform);
                            card.id = animal.id;
                            card.character.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[animal_keys[j]].Name];
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
                            card.newCardText.gameObject.SetActive(val.Item3);
                            cardDic[i][animal_keys[j]] = card;

                        }

                        
                        /*  if (cardDic.ContainsKey(index))
                          {
                              //키드 업데이트
                              if (!init)
                              {
                                  if (cardDic[index].tier != val.Item1)
                                  {
                                      cardDic[index].tier = val.Item1;
                                  }
                              }
                              else cardDic[index].tier = val.Item1;
                              cardDic[index].speed = animal.speed;
                              cardDic[index].eatSpeed = animal.eat_speed;
                              cardDic[index].minOrder = animal.min_order;
                              cardDic[index].maxOrder = animal.max_order;
                              cardDic[index].cardBg.sprite = cardBg[val.Item1 - 1];
                              cardDic[index].backGlow.color = backGlowColor[val.Item1 - 1];
                              cardDic[index].glow.color = glowColor[val.Item1 - 1];
                          }
                          else*/
                        /* {
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

                             if (!init)
                             {
                                 Debug.Log(index + " NEW");
                             }
                         }*/
                    }


                }
            }
        }
    /*    foreach (var type in AnimalManager.gatchaTiers)
        {
            (int, List<int>) val = type.Value;
            if(val.Item1 > 0)
            {
                AnimalStruct animal = AssetLoader.animals[type.Key];
                int index = type.Key - 100;

              //  foreach

                if (cardDic.ContainsKey(index))
                {
                    //키드 업데이트
                    if(!init)
                    {
                        if(cardDic[index].tier != val.Item1)
                        {
                            cardDic[index].tier = val.Item1;
                        }
                    }
                    else cardDic[index].tier = val.Item1;
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

                    if (!init)
                    {
                        Debug.Log(index + " NEW");
                    }
                }
            }
        }*/
        
    }


}
