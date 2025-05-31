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
    public Furniture currentFurniture;

    Color red = Color.red;
    Color green = Color.green;
    Color yellow = Color.yellow;
    Color yellowGreen = new Color(128, 255, 0);


    private void Update()
    {
        if(gageBar != null && currentFurniture != null)
        {
            
        }
    }

    public void ApplyData(Furniture furniture, FoodMachine fm = null)
    {
        currentFurniture = furniture;
        //  workSpaceType = WorkSpaceType.FoodMachine;
        furnitureImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[furniture.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
        if(fm != null)
        {
            upgradeCost.text = $"���׷��̵� ��� : {fm.machineLevelData.price}��";
            upgradablesStatus.text = $"���� : {fm.machineLevelData.level}\n���� �ǸŰ��� : {fm.machineLevelData.sale_proceed}��\n���� �ӵ� : {fm.machineLevelData.cooking_time}/s\n�ִ� ���귮: {fm.machineLevelData.max_height}��"; 

        }
    }

    
}
