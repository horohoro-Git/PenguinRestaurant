using CryingSnow.FastFoodRush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureInfo : MonoBehaviour
{
    Furniture currentFurniture;

    public FurnitureInfo_Type UI_TypeA;
    public FurnitureInfo_Type UI_TypeB;

  
    public void UpdateInfo(Furniture furniture)
    {
        currentFurniture = furniture;
        if (furniture.TryGetComponent<FoodMachine>(out FoodMachine fm))
        {
            UI_TypeA.gameObject.SetActive(true);
            UI_TypeB.gameObject.SetActive(false);
            UI_TypeA.ApplyData(furniture, fm);
          /*  workSpaceType = WorkSpaceType.FoodMachine;
            applianceImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[foodMachine.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
            upgradeCostText.text = $"업그레이드 비용 : {foodMachine.machineLevelStruct.price}원";
            upgradeInfoText.text = $"레벨 : {foodMachine.machineLevelStruct.level}\n음식 판매가격 : {foodMachine.machineLevelStruct.sale_proceed}원\n생산 속도 : {foodMachine.machineLevelStruct.cooking_time}/s\n최대 생산량: {foodMachine.machineLevelStruct.max_height}개";*/
        }
        else
        {
            UI_TypeA.gameObject.SetActive(false);
            UI_TypeB.gameObject.SetActive(true);
            UI_TypeB.ApplyData(furniture);
        }
    }

    public void Replace()
    {
        if (currentFurniture)
        { 
            currentFurniture.gameObject.SetActive(false);
            PlaceController placeController = GameInstance.GameIns.store.GetGoods(currentFurniture.id);
           

            placeController.transform.position = currentFurniture.transform.position;
            if (currentFurniture.TryGetComponent(out IObjectOffset fm))
            {
                placeController.offset.transform.rotation = fm.offset.rotation;
                placeController.transform.position = currentFurniture.transform.position;
            }
            else
            {
                placeController.offset.transform.rotation = currentFurniture.transform.rotation;
                placeController.transform.position = currentFurniture.transform.position;
            }
            placeController.SetLevel(currentFurniture.rotateLevel);
            GameInstance.GameIns.gridManager.ReCalculate(placeController);
            placeController.purchasedObject = true;
            placeController.currentFurniture = currentFurniture;
            placeController.gameObject.SetActive(true);

            GameInstance.GameIns.applianceUIManager.appliancePanel.gameObject.SetActive(false);
            GameInstance.GameIns.applianceUIManager.StopInfo();
            GameInstance.GameIns.gridManager.VisibleGrid(true);
        }
    }

    public void GetEnergy()
    {
        if (currentFurniture == null) GameInstance.GameIns.restaurantManager.AddFuel(currentFurniture.GetComponent<FoodMachine>());
    }

    public void Upgrade()
    {

        if (currentFurniture) GameInstance.GameIns.restaurantManager.UpgradeFoodMachine(currentFurniture.GetComponent<FoodMachine>());
      
    }
}
