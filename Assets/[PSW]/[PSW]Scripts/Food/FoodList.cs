using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FoodMachine;

public class FoodList : MonoBehaviour
{
    // 음식 가격을 정하는 class

    MoneyManager moneyManager;
    MachineType machineType;

    // 음식 가격들 ====================
    public float humbugerPrice = 300f;
    public float dountPrice = 250f;
    public float cokePrice = 150f;
    public float CuffePrice = 100f;
    // ===============================

    public void foodPriceList(float humbugerPrice, float dountPrice, float cokePrice, float CuffePrice)
    {
        this.humbugerPrice = humbugerPrice;
        this.dountPrice = dountPrice;
        this.cokePrice = cokePrice;
        this.CuffePrice = CuffePrice;
    }

    public enum foodPriceType
    {
        humbugerPrice,
        dountPrice,
        cokePrice,
        CuffePrice
    }


    //public void CheckFood(Food food)
    //{
    //    switch (machineType)
    //    {
    //        case MachineType.BurgerMachine:
    //            food.foodPrice = this.humbugerPrice;
    //            break;

    //        case MachineType.DonutMachine:
    //            food.foodPrice = this.dountPrice;
    //            break;

    //        case MachineType.CokeMachine:
    //            food.foodPrice = this.cokePrice;
    //            break;

    //        case MachineType.CoffeeMachine:
    //            food.foodPrice = this.CuffePrice;
    //            break;
    //    }
    //}
}
