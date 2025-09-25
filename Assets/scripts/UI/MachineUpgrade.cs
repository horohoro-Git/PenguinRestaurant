using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineUpgrade : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    FurnitureInfo_Type furnitureInfo;

    void Awake()
    {
        furnitureInfo = GetComponentInParent<FurnitureInfo_Type>();
    }

    void OnDisable()
    {
        if(furnitureInfo != null)  furnitureInfo.upgradeHolding = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        furnitureInfo.upgradeHolding = true;
        furnitureInfo.furnitureInfo.Upgrade();
        if (furnitureInfo.machineUpgrading != null) StopCoroutine(furnitureInfo.machineUpgrading);
        furnitureInfo.machineUpgrading = StartCoroutine(furnitureInfo.Upgrading());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (furnitureInfo != null) furnitureInfo.upgradeHolding = false;
    }
}
