using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxUpGradeBtn : MonoBehaviour
{
    [SerializeField] public Button upButton;
    FoodList FoodList;
    MachineType machineType;
    GameInstance gameInstance;
    FoodMachine foodMachinesType;



    public float moneyUpGrade = 100f;

    private void Start()
    {
        FoodList = GetComponent<FoodList>();
        foodMachinesType = GameInstance.GameIns.workSpaceManager.foodMachines[0];

        upButton.onClick.AddListener(()=> OnBtnFoodUpGrade(moneyUpGrade));
    }
    public void OnBtnFoodUpGrade(float moneyUpGrade)
    {
        switch(foodMachinesType.machineType)
        {
            case MachineType.BurgerMachine:
                FoodList.humbugerPrice += moneyUpGrade;
                Debug.Log("ÇÜ¹ö°Å °¡°ÝÀÌ ÀÛµ¿µÊ");
                break;
            case MachineType.DonutMachine:
                FoodList.dountPrice += moneyUpGrade;
                break;
            case MachineType.CokeMachine:
                FoodList.cokePrice += moneyUpGrade;
                break;
            case MachineType.CoffeeMachine:
                FoodList.CuffePrice += moneyUpGrade;
                break;
        }
        
    }
}
