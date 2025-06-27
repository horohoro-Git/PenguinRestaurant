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
                if (data.unlocked)
                {
                    DateTime lastLoginTime = new DateTime(data.lastTime);
                    TimeSpan diff = DateTime.UtcNow - lastLoginTime;
                    double seconds = diff.TotalSeconds;

                  //  if (seconds > 7200) seconds = 7200;

                    int timer = Mathf.FloorToInt((int)seconds / 5);
                    if(timer > 1440) timer = 1440;
                    BigInteger testInteger = 100 * 1440 * AnimalManager.gatchaValues / 100;
                    BigInteger newInteger = 100 * timer * AnimalManager.gatchaValues / 100;
                    data.Money += newInteger;
                    if(data.Money > testInteger) data.Money = testInteger;
                    data.changed = true;   

                    Debug.Log(data.Money);
                }
                else
                {
                    data.unlocked = true;
                    data.changed = true;

                }
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

            data.Money += (100 * AnimalManager.gatchaValues / 100);
            data.changed = true;
            BigInteger testInteger = 100 * 1440 * AnimalManager.gatchaValues / 100;
            if(data.Money > testInteger) data.Money = testInteger;
            f = 0;
            yield return null;
        }
    }
}
