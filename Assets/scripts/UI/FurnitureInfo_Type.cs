
using System;
using System.Collections;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AssetLoader;
public class FurnitureInfo_Type : MonoBehaviour
{
    FurnitureInfo furnitureInfo;
    public Image gageBar;
    public Image furnitureImage;
    public TMP_Text furnitureName;

    public TMP_Text upgradablesStatus;
    public TMP_Text upgradeCost;
    public TMP_Text placedNum;
    public TMP_Text placedText;

    public Slider fuelSlider;
    public TMP_Text fishNum;
    public TMP_Text gettableGold;
    public Furniture currentFurniture;
    public FoodMachine currentFoodMachine;

    public RefillFishes fishPlus;
    public RefillFishes fishMinus;
    public Button fishMax;
    public Button getFishes;

    public Image fishImage;

    Color red = Color.red;
    Color green = Color.green;
    Color yellow = Color.yellow;
    Color yellowGreen = new Color(0.8f, 1f, 0.6f);
    StringBuilder stringBuilder = new StringBuilder();
    float rewardTimer = 0;
    BigInteger rewardAmount;
    [NonSerialized] public int increaseFishes;
    [NonSerialized] public bool holding;
    public Coroutine refill;
    bool gageCheck;
    bool currentGageState;

    Coroutine insufficientFish;
    private void Awake()
    {
        furnitureInfo = GetComponentInParent<FurnitureInfo>();
    }

    private void Start()
    {/*
        if(fishPlus != null)
        {
            fishPlus.onClick.AddListener(() => {

                if (currentFoodMachine != null)
                {
                    int exists = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                    int num = currentFoodMachine.machineLevelData.fishes;
                    if (exists > increaseFishes && (increaseFishes + 1 + num) <= 100)
                    {
                        increaseFishes++;
                        holding = true;
                        StartCoroutine(Increasing());
                    }

                }


            });
        }*/

        /*  if (fishMinus != null)
          {
              fishMinus.onClick.AddListener(() => {
                  if (currentFoodMachine != null)
                  {
                      if(increaseFishes > 0)
                      {
                          increaseFishes--;
                          holding = true;
                          StartCoroutine(Decreasing());
                      }
                  }



              });
          }

          if(fishMax != null)
          {
              fishMax.onClick.AddListener(() => {

                  if(currentFoodMachine != null)
                  {
                      int exists = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                      int num = 100 - currentFoodMachine.machineLevelData.fishes;
                      num = num > exists ? exists : num;
                      increaseFishes = num;

                  }

              });
          }*/

        if (fishMax != null)
        {
            fishMax.onClick.AddListener(() =>
            {

                if (currentFoodMachine != null)
                {
                    int exists = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                    int num = 100 - currentFoodMachine.machineLevelData.fishes;
                    num = num > exists ? exists : num;
                    increaseFishes = num;

                }

            });
        }
    }
    private void Update()
    {
        if(gageBar != null && currentFoodMachine != null)
        {
            int fishes = currentFoodMachine.machineLevelData.fishes;
            float num = fishes / 100f;
            gageBar.fillAmount = num;

            if(num >= 0.7f) gageBar.color = green;
            else if(num >= 0.5f) gageBar.color = yellowGreen;
            else if(num >= 0.3f) gageBar.color = yellow;
            else gageBar.color = red;

            if(!gageCheck)
            {
                gageCheck = true;
                if (fishes == 0)
                {
                    if (AssetLoader.loadedAtlases.ContainsKey("UI")) fishImage.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("exclamation");
                    currentGageState = false;
                    if(insufficientFish != null) StopCoroutine(insufficientFish);
                    insufficientFish = StartCoroutine(InsufficientFish());
                }
                else
                {
                    if (AssetLoader.loadedAtlases.ContainsKey("UI")) fishImage.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("Fish");
                    if (insufficientFish != null) StopCoroutine(insufficientFish);
                    currentGageState = true;
                }
            }
            else
            {
                if (fishes == 0 && currentGageState)
                {
                    if (AssetLoader.loadedAtlases.ContainsKey("UI")) fishImage.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("exclamation");
                    currentGageState = false;
                    if (insufficientFish != null) StopCoroutine(insufficientFish);
                    insufficientFish = StartCoroutine(InsufficientFish());
                }
                else if (fishes > 0 && !currentGageState)
                {
                    if (AssetLoader.loadedAtlases.ContainsKey("UI")) fishImage.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("Fish");
                    if (insufficientFish != null) StopCoroutine(insufficientFish);
                    currentGageState = true;
                }
            }
          
            float v = fuelSlider.value;
            int available = 100 - fishes;
            int showUseNum = Mathf.CeilToInt(available * v);
            showUseNum = showUseNum > available ? available : showUseNum;
            int existsFishes = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
            showUseNum = showUseNum > existsFishes ? existsFishes : showUseNum;

            fishNum.text = App.gameSettings.language == Language.KOR ? "생선 " + increaseFishes + " 충전"
                            : "Add " + increaseFishes + " Fish";
            furnitureInfo.fishesNum = increaseFishes;
        }
        else if(gettableGold != null && currentFurniture)
        {
            if(rewardTimer < Time.time)
            {
                rewardTimer = Time.time + 5f;
                if(rewardAmount != currentFurniture.GetComponent<VendingMachine>().data.Money)
                {
                    rewardAmount = currentFurniture.GetComponent<VendingMachine>().data.Money;
                    Utility.GetFormattedMoney(rewardAmount, stringBuilder);
                    gettableGold.text = stringBuilder.ToString();
                    furnitureInfo.rewardNum = stringBuilder.ToString();
                }
            }
        }
    }

    public IEnumerator Increasing()
    {
        float timer = 0.2f;
        while(holding)
        {
            if (timer > 0)
            {
                yield return new WaitForSeconds(timer);
                timer -= 0.05f;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }

            if(holding)
            {
                int exists = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                int num = currentFoodMachine.machineLevelData.fishes;
                if (exists > increaseFishes && (increaseFishes + 1 + num) <= 100)
                {
                    increaseFishes++;
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);
                }
                else
                {
                    yield break;
                }
            }
        }
    }
    public IEnumerator Decreasing()
    {
        float timer = 0.2f;
        while (holding)
        {
            if (timer > 0)
            {
                yield return new WaitForSeconds(timer);
                timer -= 0.05f;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }

            if (holding)
            {
                int num = currentFoodMachine.machineLevelData.fishes;
                if ((increaseFishes - 1) >= 0)
                {
                    increaseFishes--;
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);
                }
                else
                {
                    yield break;
                }
            }

        }
    }

    IEnumerator InsufficientFish()
    {

        while(true)
        {
            float f = 0f;
            Color c = fishImage.color;
            while(f <= 1)
            {
                f += Time.deltaTime / 0.2f;
                float newAlpha = Mathf.Lerp(c.a, 0, f);
                c.a = newAlpha;
                fishImage.color = c;
                yield return null;
            }
            f = 0;
            while (f <= 1)
            {
                f += Time.deltaTime / 0.2f;
                float newAlpha = Mathf.Lerp(c.a, 1, f);
                c.a = newAlpha;
                fishImage.color = c;
                yield return null;
            }

        }
    }
    public void ApplyData(Furniture furniture, FoodMachine fm = null)
    {
        currentFurniture = furniture;
        currentFoodMachine = furniture.GetComponent<FoodMachine>(); 
        //  workSpaceType = WorkSpaceType.FoodMachine;
        furnitureImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[furniture.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
        if (fm != null)
        {
            Color c = fishImage.color;
            c.a = 1;
            fishImage.color = c;
            gageCheck = false;
            increaseFishes = 0;
            upgradeCost.text = App.gameSettings.language == Language.KOR ? $"업그레이드 : {fm.machineLevelData.calculatedPrice}" : $"Upgrade : {fm.machineLevelData.calculatedPrice}";


            int level = currentFoodMachine.machineLevelData.level + 1;
            int id = currentFoodMachine.machineLevelData.id;
            int index = id + (level >= 41 ? 41 : level >= 31 ? 31 : 1);

            MachineLevelOffset offset = AssetLoader.machineLevelOffsets[index];

            BigInteger upgradePrice = currentFoodMachine.machineLevelData.Price_Value + Mathf.FloorToInt(Mathf.Pow((level - 1), offset.price_pow) * offset.price_mul);
            StringBuilder moneyString = new StringBuilder();
            moneyString = Utility.GetFormattedMoney(upgradePrice, moneyString);
            ///currentFoodMachine.machineLevelData.calculatedPrice = moneyString.ToString();
            BigInteger salePrice = Utility.StringToBigInteger(currentFoodMachine.machineLevelData.sale_proceed);
            if (offset.sale_div == 0) salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul);
            else salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / Mathf.FloorToInt((level - 1) * 0.07f);
            float nextCookingTimer = currentFoodMachine.machineLevelData.cooking_time - (level - 1) * offset.reduce_timer;
            if (nextCookingTimer < 3f) nextCookingTimer = 3f;
            int maxHeight = currentFoodMachine.machineLevelData.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);

            upgradablesStatus.text = App.gameSettings.language == Language.KOR ? ($"레벨 : {fm.machineLevelData.level} -> <color=#00FF00>{fm.machineLevelData.level + 1}</color> \n음식 판매가격 : {fm.machineLevelData.calculatedSales} -> <color=#00FF00>{salePrice}</color>")
                                                                                : ($"Level : {fm.machineLevelData.level} -> <color=#00FF00>{fm.machineLevelData.level + 1}</color> \nSale Price : {fm.machineLevelData.calculatedSales} -> <color=#00FF00>{salePrice}</color>");

            if (fm.machineLevelData.calculatedCookingTimer != nextCookingTimer)
            {
                upgradablesStatus.text += App.gameSettings.language == Language.KOR ? ($"\n생산 속도 : {fm.machineLevelData.calculatedCookingTimer}/s -> <color=#00FF00>{nextCookingTimer}/s</color>")
                                                                                     : ($"\nCooking Speed : {fm.machineLevelData.calculatedCookingTimer}/s -> <color=#00FF00>{nextCookingTimer}/s</color>");
            }
            else
            {
                upgradablesStatus.text += App.gameSettings.language == Language.KOR ? ($"\n생산 속도 : {fm.machineLevelData.calculatedCookingTimer}/s") : ($"\nCooking Speed : {fm.machineLevelData.calculatedCookingTimer}/s");
            }


            if (fm.machineLevelData.calculatedHeight != maxHeight)
            {
                upgradablesStatus.text += App.gameSettings.language == Language.KOR ? ($"\n최대 생산량: {fm.machineLevelData.calculatedHeight} -> <color=#00FF00>{maxHeight}</color>")
                                                                                    : ($"\nMax Production: {fm.machineLevelData.calculatedHeight} -> <color=#00FF00>{maxHeight}</color>");
            }
            else
            {
                upgradablesStatus.text += App.gameSettings.language == Language.KOR ? ($"\n최대 생산량: {fm.machineLevelData.calculatedHeight}")
                                                                                    : ($"\nMax Production: {fm.machineLevelData.calculatedHeight}");
            }

        }
        // }\n생산 속도 : { fm.machineLevelData.calculatedCookingTimer}/ s-> < color =#00FF00>{nextCookingTimer}/s</color>\n최대 생산량: {fm.machineLevelData.calculatedHeight} -> <color=#00FF00>{maxHeight}</color>")
        if (furniture.spaceType == WorkSpaceType.Table || furniture.spaceType == WorkSpaceType.Trashcan || furniture.spaceType == WorkSpaceType.Counter)
        {
            int max = goods[furniture.id].num;
            int n = GameInstance.GameIns.store.goodsDic[furniture.id].Count;
            string maxNum = n == 0 ? (App.gameSettings.language == Language.KOR ? "(최대)" : "(Max)") : "";
            placedNum.text = App.gameSettings.language == Language.KOR ? "배치됨 " + (max - n) + " " + maxNum : "Placed Count " + (max - n) + " " + maxNum;
            placedText.text = App.gameSettings.language == Language.KOR ? "재배치" : "Replace";
            furnitureName.text = App.gameSettings.language == Language.KOR ? goods[furniture.id].name_kor : goods[furniture.id].name_eng;
        }
        else if (furniture.spaceType == WorkSpaceType.Vending)
        {
            furnitureName.text = App.gameSettings.language == Language.KOR ? "자판기" : "Vending Machine";
        }
        else if (furniture.spaceType == WorkSpaceType.Door)
        {
            placedNum.text = "";
            furnitureName.text = App.gameSettings.language == Language.KOR ? "문" : "Door";
        }

    }
    public void ResetSlider()
    {
        fuelSlider.value = 0;
    }
    public void ResetReward()
    {
        if (currentFurniture.GetComponent<VendingMachine>().data.Money > 0)
        {
            gettableGold.text = "0";
            furnitureInfo.rewardNum = "0";
            currentFurniture.GetComponent<VendingMachine>().data.Money = 0;
        }
    }
}
