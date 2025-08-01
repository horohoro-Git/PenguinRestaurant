using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Cinemachine.CinemachineOrbitalTransposer;

public class RefillFishes : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public FurnitureInfo_Type info;
    public bool increase;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (info.currentFoodMachine != null)
        {
            info.holding = true;
            if (increase)
            {
                int exists = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
                int num = info.currentFoodMachine.machineLevelData.fishes;
                if (exists > info.increaseFishes && (info.increaseFishes + 1 + num) <= 100)
                {
                    info.increaseFishes++;
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);
                    if (info.refill != null) StopCoroutine(info.refill);
                    info.refill = StartCoroutine(info.Increasing());
                }
            }
            else
            {
                if (info.increaseFishes > 0)
                {
                    info.increaseFishes--;
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);
                    if (info.refill != null) StopCoroutine(info.refill);
                    info.refill = StartCoroutine(info.Decreasing());
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        info.holding = false;
    }
}
