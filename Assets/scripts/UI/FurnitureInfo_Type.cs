using CryingSnow.FastFoodRush;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AssetLoader;

public class FurnitureInfo_Type : MonoBehaviour
{
    public Image gageBar;
    public Image furnitureImage;
    public TMP_Text furnitureName;

    public TMP_Text upgradablesStatus;
    public TMP_Text upgradeCost;
    public TMP_Text placedNum;


    Color red = Color.red;
    Color green = Color.green;
    Color yellow = Color.yellow;
    Color yellowGreen = new Color(128, 255, 0);

    public void ApplyData(Furniture furniture, FoodMachine fm = null)
    {
        //  workSpaceType = WorkSpaceType.FoodMachine;
        furnitureImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[furniture.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
        if(fm != null)
        {
            upgradeCost.text = $"업그레이드 비용 : {fm.machineLevelStruct.price}원";
            upgradablesStatus.text = $"레벨 : {fm.machineLevelStruct.level}\n음식 판매가격 : {fm.machineLevelStruct.sale_proceed}원\n생산 속도 : {fm.machineLevelStruct.cooking_time}/s\n최대 생산량: {fm.machineLevelStruct.max_height}개"; 

        }
    }

    
}
