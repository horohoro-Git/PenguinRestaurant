using CryingSnow.FastFoodRush;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEditor.Rendering;
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
    public Slider fuelSlider;
    public TMP_Text fishNum;
    public TMP_Text gettableGold;
    public Furniture currentFurniture;
    public FoodMachine currentFoodMachine;
    Color red = Color.red;
    Color green = Color.green;
    Color yellow = Color.yellow;
    Color yellowGreen = new Color(0.8f, 1f, 0.6f);
    StringBuilder stringBuilder = new StringBuilder();
    float rewardTimer = 0;
    BigInteger rewardAmount;
    private void Awake()
    {
        furnitureInfo = GetComponentInParent<FurnitureInfo>();
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

            float v = fuelSlider.value;
            int available = 100 - fishes;
            int showUseNum = Mathf.CeilToInt(available * v);
            showUseNum = showUseNum > available ? available : showUseNum;
        
            fishNum.text = "����� �� " + showUseNum.ToString() + " / " + available.ToString();
            furnitureInfo.fishesNum = showUseNum;
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

    public void ApplyData(Furniture furniture, FoodMachine fm = null)
    {
        currentFurniture = furniture;
        currentFoodMachine = furniture.GetComponent<FoodMachine>(); 
        //  workSpaceType = WorkSpaceType.FoodMachine;
        furnitureImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[furniture.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
        if(fm != null)
        {
            upgradeCost.text = $"���׷��̵� ��� : {fm.machineLevelData.calculatedPrice}��";
            upgradablesStatus.text = $"���� : {fm.machineLevelData.level}\n���� �ǸŰ��� : {fm.machineLevelData.calculatedSales}��\n���� �ӵ� : {fm.machineLevelData.calculatedCookingTimer}/s\n�ִ� ���귮: {fm.machineLevelData.calculatedHeight}��"; 

        }
        if (furniture.spaceType == WorkSpaceType.Table || furniture.spaceType == WorkSpaceType.Trashcan || furniture.spaceType == WorkSpaceType.Counter)
        {
            int max = goods[furniture.id].num;
            int n = GameInstance.GameIns.store.goodsDic[furniture.id].Count;
            placedNum.text = "��ġ�� �� " + (max - n) + " / " + max;
            furnitureName.text = App.gameSettings.language == Language.KOR ? goods[furniture.id].name_kor : goods[furniture.id].name_eng;
        }
        else if (furniture.spaceType == WorkSpaceType.Vending) 
        {
            furnitureName.text = "���Ǳ�";
        }
        else if (furniture.spaceType == WorkSpaceType.Door)
        {
            placedNum.text = "";
            furnitureName.text = "��";
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
