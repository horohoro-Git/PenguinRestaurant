using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class VendingMachine : Furniture
{

    public BigInteger getMoney;
    [NonSerialized] public VendingMachineData data;
    private void Awake()
    {
        id = 1010;
    }


    public override void Start()
    {
        base.Start();
        StartCoroutine(GetVendigData());
    }

    IEnumerator GetVendigData()
    {
        while (true)
        {
            if(GameInstance.GameIns.restaurantManager.vendingData != null)
            {
                data = GameInstance.GameIns.restaurantManager.vendingData;
            ///    getMoney = data.Money;
                StartCoroutine(IncreaseReward());

                yield break;
            }
            yield return null;
        }
    }
    IEnumerator IncreaseReward()
    {
        float f = 0;
        while (true)
        {

            while (f < 5)
            {
                
                f += Time.unscaledDeltaTime;
                yield return null;
            }

            data.changed = true;
            data.Money += 10000;
            f = 0;
            yield return null;
        }
    }
}
