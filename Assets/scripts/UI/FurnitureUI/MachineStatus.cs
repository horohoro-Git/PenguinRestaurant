using CryingSnow.FastFoodRush;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MachineStatus : MonoBehaviour
{
    public FoodMachine foodMachine;
    public FuelGage exclamationFish;
    public Image fishImage;
    public TMP_Text fishNum;
    public Image fishGageBar;
    int latestFishes = -1;
    [NonSerialized] public RectTransform rectTransform;
    Button fillingFishes;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        fillingFishes = GetComponent<Button>();
    }
    void Start()
    {
        fillingFishes.onClick.AddListener(() =>
        {
            if (!RestaurantManager.tutorialEventKeys.Contains(TutorialEventKey.NoFishTutorial))
            {
                int fishes = 100 - foodMachine.machineLevelData.fishes;
                if (fishes > 0)
                {
                    fishes = fishes <= GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes ? fishes : GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                    foodMachine.machineLevelData.checkingFishes += fishes;
                    GameInstance.GameIns.restaurantManager.AddFuel(foodMachine, fishes);

                    if (RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.FillFishes))
                    {
                        RestaurantManager.tutorialKeys.Remove((int)TutorialEventKey.FillFishes);
                        Tutorials tutorials = GameInstance.GameIns.restaurantManager.tutorials;
                        Tutorials.TutorialUnlockLateTime(GameInstance.GameIns.restaurantManager.tutorialStructs[tutorials.id][tutorials.count - 1]);
                        ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.FillFishes))?.Invoke(TutorialEventKey.FillFishes);
                        GameInstance.GameIns.uiManager.TutorialEnd(true);
                    }

                    GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.increaseFishes = 0;
                }
            }
        });
    }
    private void OnEnable()
    {
        UpdateAnimation();
    }
    public void Setup(FoodMachine machine)
    {
        foodMachine = machine;
        foodMachine.machineLevelData.OnFishChanged += UpdateFishes;
        exclamationFish.Setup();
        UpdateFishes();
        GameInstance.GameIns.restaurantManager.machineStatuses.Add(this);
    }

    public void UpdateFishes()
    {
        if (foodMachine.machineLevelData != null && GameInstance.GameIns.restaurantManager != null)
        {
            int hasFishes = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
            int leastFishes = foodMachine.machineLevelData.fishes;
            int fishes = 100 - leastFishes;
            int res = fishes > hasFishes ? hasFishes : fishes;
            fishNum.text = res.ToString();
            if (leastFishes == 0)
            {
                if (latestFishes != leastFishes)
                {
                    latestFishes = leastFishes;
                    exclamationFish.gameObject.SetActive(true);
                    fishImage.gameObject.SetActive(false);
                    if (exclamationFish.gameObject.activeSelf)
                    {
                        exclamationFish.animator.enabled = true;
                        exclamationFish.animator.SetInteger(AnimationKeys.state, 1);
                    }
                }
            }
            else
            {
                if (latestFishes != leastFishes)
                {
                    latestFishes = leastFishes;

                    if (exclamationFish.gameObject.activeSelf)
                    {
                        exclamationFish.animator.SetInteger(AnimationKeys.state, 0);
                        exclamationFish.animator.enabled = false;
                        exclamationFish.gameObject.SetActive(false);
                    }
                    fishImage.gameObject.SetActive(true);
                }
            }

            fishGageBar.fillAmount = leastFishes == 0 ? 0 : leastFishes / 100f;
        }
    }

    public void UpdateAnimation()
    {
        if (foodMachine != null)
        {
            if (foodMachine.machineLevelData != null && GameInstance.GameIns.restaurantManager != null)
            {
                int leastFishes = foodMachine.machineLevelData.fishes;
                if (leastFishes == 0 && exclamationFish.gameObject.activeSelf)
                {
                    exclamationFish.animator.enabled = true;
                    exclamationFish.animator.SetInteger(AnimationKeys.state, 1);
                }
            }
        }
    }
}
