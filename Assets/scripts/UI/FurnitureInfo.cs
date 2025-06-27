using CryingSnow.FastFoodRush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FurnitureInfo : MonoBehaviour
{
    Furniture currentFurniture;

    public FurnitureInfo_Type UI_TypeA;
    public FurnitureInfo_Type UI_TypeB;
    public FurnitureInfo_Type UI_TypeC;

    [NonSerialized] public int fishesNum;
    [NonSerialized] public string rewardNum;
  
    public void UpdateInfo(Furniture furniture)
    {
        currentFurniture = furniture;
        if (furniture.TryGetComponent<FoodMachine>(out FoodMachine fm))
        {
            UI_TypeA.gameObject.SetActive(true);
            UI_TypeB.gameObject.SetActive(false);
            UI_TypeC.gameObject.SetActive(false);
            UI_TypeA.ApplyData(furniture, fm);
          /*  workSpaceType = WorkSpaceType.FoodMachine;
            applianceImage.sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[foodMachine.id].ID); //machines[(int)foodMachine.machineLevelStruct.type - 1];
            upgradeCostText.text = $"업그레이드 비용 : {foodMachine.machineLevelStruct.price}원";
            upgradeInfoText.text = $"레벨 : {foodMachine.machineLevelStruct.level}\n음식 판매가격 : {foodMachine.machineLevelStruct.sale_proceed}원\n생산 속도 : {foodMachine.machineLevelStruct.cooking_time}/s\n최대 생산량: {foodMachine.machineLevelStruct.max_height}개";*/
        }
        else if(furniture.TryGetComponent(out VendingMachine vm))
        {
            UI_TypeA.gameObject.SetActive(false);
            UI_TypeB.gameObject.SetActive(false);
            UI_TypeC.gameObject.SetActive(true);
            UI_TypeC.ApplyData(furniture);
        }
        else 
        {
            UI_TypeA.gameObject.SetActive(false);
            UI_TypeB.gameObject.SetActive(true);
            UI_TypeC.gameObject.SetActive(false);
            UI_TypeB.ApplyData(furniture);
        }
    }

    public void GetReward()
    {
        if (currentFurniture)
        {
            if (currentFurniture.GetComponent<VendingMachine>().data.Money > 0)
            {
                GameInstance.GameIns.restaurantManager.GetMoney(currentFurniture.GetComponent<VendingMachine>().data.Money.ToString());
                SoundManager.Instance.PlayAudioWithKey(GameInstance.GameIns.uISoundManager.Money(), 0.2f, GameInstance.GameIns.restaurantManager.moneyChangedSoundKey);
            }
        }
    }

    public void Replace()
    {
        if (currentFurniture)
        {  
            Queue<int> placedArea = MoveCalculator.GetCheckAreaWithBounds(GameInstance.GameIns.calculatorScale, currentFurniture.GetComponentInChildren<Collider>());
            currentFurniture.gameObject.SetActive(false);
            PlaceController placeController = GameInstance.GameIns.store.GetGoods(currentFurniture.id);
           
            placeController.placedArea = placedArea;
            placeController.transform.position = currentFurniture.originPos;
            if (currentFurniture.TryGetComponent(out IObjectOffset fm))
            {
                placeController.offset.transform.rotation = fm.offset.rotation;
            }
            else
            {
                placeController.offset.transform.rotation = currentFurniture.transform.rotation;
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
        if (currentFurniture != null)
        {
            if(fishesNum <= GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes)
            GameInstance.GameIns.restaurantManager.AddFuel(currentFurniture.GetComponent<FoodMachine>(), fishesNum);
        }
    }

    public void Upgrade()
    {

        if (currentFurniture) GameInstance.GameIns.restaurantManager.UpgradeFoodMachine(currentFurniture.GetComponent<FoodMachine>(), this);
      
    }
}
