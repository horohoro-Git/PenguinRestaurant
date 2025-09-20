using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEditor.PlayerSettings;
//using static UnityEngine.Rendering.DebugUI;

public enum TutorialEventKey
{
    None,
    NoFishNoCook = 1,
    NoEnemy = 2,
    NoCleaning = 3,
    NoFishFilling = 4,
    NoEating = 5,
    NoEmloyeeCleaning = 6,
    NoCustomer = 7,
    NoEmployee = 8,
    NoFishing = 9,
    BuyCounter = 1001,
    BuyMachine = 1003,
    BuyTable = 1007,
    BuyTrashcan = 1008,
    EnterGatcha = 2001,
    DrawGatcha = 2002,
    EnterRestaurant = 2003,
    HireEmployee = 2004,
    FillFishes = 2005,
    EnterFishing = 2006,
    StartFishing = 2007,
    ComploeteFishing = 2008,
    CatchEnemy = 2009,
    Cleaning = 2010,
    TrashcanMinigame = 2011,
    TutorialComplete = 2012,
    NextMap = 2013
}

public enum TutorialType
{
    None,
    UnlockGooods,
    ChangeMapRestaurant,
    ChangeMapFishing,
    ChangeMapGatcha,
    FillFish,
    Advertise,
    Fishing,
    SpawnCustomer,
    SpawnEnemy,
    TutorialComplete
}


public class Tutorials
{
    public int id;
    public bool worked;
    public int count;
    static string start_unlock = "start_unlock_key";
    static string late_unlock = "late_unlock_key";
    static string start_lock = "start_lock_key";
    static string late_lock = "late_lock_key";
    public Tutorials(int id, bool worked)
    {
        this.id = id;
        this.worked = worked;
    }

    public static void TutorialEvent(TutorialEventKey id, TutorialStruct data)
    {
        if (id != data.event_id) return;

        Tutorials tutorials = GameInstance.GameIns.restaurantManager.tutorials;


        switch (data.event_type)
        {
            case TutorialType.SpawnEnemy:
                EnemySpawner enemySpawner = GameInstance.GameIns.restaurantManager.door.GetComponentInChildren<EnemySpawner>();
                GameObject target = enemySpawner.SpawnEnemyTutorial();
                EnemyFollowing(target, App.GlobalToken).Forget();
             
                break;
            case TutorialType.SpawnCustomer:
                AnimalSpawner[] spawners = AnimalSpawner.FindObjectsOfType<AnimalSpawner>();

                foreach (var v in spawners)
                {
                    if (v.type == AnimalSpawner.SpawnerType.FastFood)
                    {
                        v.TutorialSpawnAnimal(App.GlobalToken).Forget();
                    }
                }
                break;
          
        }


        tutorials.id = data.id + 1;
        tutorials.count = 0;
        tutorials.worked = true;
        SaveLoadSystem.SaveTutorialData(tutorials);

        if(tutorials.id < GameInstance.GameIns.restaurantManager.tutorialStructs.Count) Setup(tutorials);

        EventManager.RemoveTutorialEvent(data.event_id);
    }
    public static void TutorialUnlock(TutorialStruct key)
    {
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;

        for (int i = 1; i <= 2; i++)
        {
            string keyName = start_unlock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(key);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value, true); //상점 제한 및 기능 제한 키 해제
        }

        for (int i = 1; i <= 2; i++)
        {
            string keyName = start_lock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(key);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value); //상점 제한 및 기능 제한 키 해제
        }

        switch (key.event_id)
        {
            case TutorialEventKey.None: break;
            case TutorialEventKey.EnterGatcha:
                GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
                break;
            case TutorialEventKey.FillFishes:
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishPlus.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMinus.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMax.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.getFishes.gameObject.SetActive(true);
                break;
            case TutorialEventKey.EnterFishing:
                GameInstance.GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
                break;
        }

        if (key.event_text > 0) GameInstance.GameIns.uiManager.tutoText2.text = App.languages[key.event_text].text; //튜토리얼 목표 업데이트

        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);


    }
    public static void TutorialUnlockLateTime(TutorialStruct data)
    {
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        for (int i = 1; i <= 2; i++)
        {
            string keyName = late_unlock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(data);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value, true); //상점 제한 및 기능 제한 키 해제
        }

        for (int i = 1; i <= 2; i++)
        {
            string keyName = late_lock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(data);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value); //상점 제한 및 기능 제한 
        }

        switch(data.event_id)
        {
            case TutorialEventKey.TutorialComplete:
                GameInstance.GameIns.uiManager.targetGO.SetActive(true);
                break;
        }
      

        if (data.event_text > 0) GameInstance.GameIns.uiManager.tutoText2.text = App.languages[data.event_text].text; //튜토리얼 목표 업데이트

        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
    }

    public static void Setup(Tutorials tutos)
    {
      //  if(tutos.id >=)
        TutorialStruct tutorialStruct = GameInstance.GameIns.restaurantManager.tutorialStructs[tutos.id][0];
        RestaurantManager.tutorialKeys.Add((int)tutorialStruct.event_id);

        if (tutorialStruct.event_start) TutorialUnlock(tutorialStruct);
        //if (GameInstance.GameIns.restaurantManager.tutorialStructs[tutos.id].Count == 1 &&  tutorialStruct.event_start) TutorialUnlock(tutorialStruct);
        Action<TutorialEventKey> del = (id) =>
        {
            TutorialEvent(id, tutorialStruct);
        };
        EventManager.AddTutorialEvent(tutorialStruct.event_id, del);

    }

    public async static UniTask EnemyFollowing(GameObject target, CancellationToken cancellationToken)
    {
        await UniTask.Delay(1000, true, cancellationToken: cancellationToken);
      
        if(!target.GetComponent<BlackConsumer>().bDead)
        {
            GameInstance.GameIns.playerCamera.followTarget = target;
            GameInstance.GameIns.inputManager.InputDisAble = true;
        }
    }

 
    public static void TutorialUpdate(int saveID)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.UnlockHire(false);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(false);
       
        //기능 제한
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        if (goods.ContainsKey(1001)) AddStoreGoodsKey(goods[1001], 1001); else AddStoreGoodsKey(null, 1001);
        if (goods.ContainsKey(1002)) AddStoreGoodsKey(goods[1002], 1002); else AddStoreGoodsKey(null, 1002);
        if (goods.ContainsKey(1003)) AddStoreGoodsKey(goods[1003], 1003); else AddStoreGoodsKey(null, 1003);
        if (goods.ContainsKey(1004)) AddStoreGoodsKey(goods[1004], 1004); else AddStoreGoodsKey(null, 1004);
        if (goods.ContainsKey(1005)) AddStoreGoodsKey(goods[1005], 1005); else AddStoreGoodsKey(null, 1005);
        if (goods.ContainsKey(1006)) AddStoreGoodsKey(goods[1006], 1006); else AddStoreGoodsKey(null, 1006);
        if (goods.ContainsKey(1007)) AddStoreGoodsKey(goods[1007], 1007); else AddStoreGoodsKey(null, 1007);
        if (goods.ContainsKey(1008)) AddStoreGoodsKey(goods[1008], 1008); else AddStoreGoodsKey(null, 1008);
        if (goods.ContainsKey(1101)) AddStoreGoodsKey(goods[1101], 1101); else AddStoreGoodsKey(null, 1101);
        if (goods.ContainsKey(1102)) AddStoreGoodsKey(goods[1102], 1102); else AddStoreGoodsKey(null, 1102);
        RestaurantManager.tutorialKeys.Add(1000); //직원 고용 제한
        RestaurantManager.tutorialKeys.Add(3000);
        RestaurantManager.tutorialKeys.Add(5000);//손님 제한
        RestaurantManager.tutorialKeys.Add(500); //음식 먹기 제한
        RestaurantManager.tutorialKeys.Add(200); //불량배 생성 제한
        RestaurantManager.tutorialKeys.Add(300); //직접 쓰레기 치우기 제한
        RestaurantManager.tutorialKeys.Add(400); //생선 채우기 제한
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoEmployee);
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoFishNoCook);
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoEating);
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoCleaning);
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoCustomer);
        AddStoreGoodsKey(null, (int)TutorialEventKey.NoFishing);
        GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishPlus.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMinus.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMax.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.getFishes.gameObject.SetActive(false);


        for(int i = 0; i < saveID; i++)
        {
            Unlock(i);
        }
      
    }


    public static void Unlock(int id)
    {
        TutorialStruct tutorialStruct = GameInstance.GameIns.restaurantManager.tutorialStructs[id][0];

        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;

        for (int i = 1; i <= 2; i++)
        {
            string keyName = start_unlock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(tutorialStruct);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value, true); //상점 제한 및 기능 제한 키 해제
        }

        for (int i = 1; i <= 2; i++)
        {
            string keyName = start_lock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(tutorialStruct);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value); //상점 제한 및 기능 제한 키 해제
        }

        for (int i = 1; i <= 2; i++)
        {
            string keyName = late_unlock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(tutorialStruct);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value, true); //상점 제한 및 기능 제한 키 해제
        }

        for (int i = 1; i <= 2; i++)
        {
            string keyName = late_lock + i.ToString();
            FieldInfo field = typeof(TutorialStruct).GetField(keyName, BindingFlags.Public | BindingFlags.Instance);
            TutorialEventKey value = (TutorialEventKey)field.GetValue(tutorialStruct);

            StoreGoods storeGoods = goods.ContainsKey((int)value) ? goods[(int)value] : null;
            AddStoreGoodsKey(storeGoods, (int)value); //상점 제한 및 기능 제한 
        }
        switch(tutorialStruct.event_id)
        {
            case TutorialEventKey.None: break;
            case TutorialEventKey.EnterGatcha:
                GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
                break;
            case TutorialEventKey.FillFishes:
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishPlus.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMinus.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.fishMax.gameObject.SetActive(true);
                GameInstance.GameIns.applianceUIManager.furnitureUI.UI_TypeA.getFishes.gameObject.SetActive(true);
                break;
     
            case TutorialEventKey.TutorialComplete:
                GameInstance.GameIns.uiManager.targetGO.SetActive(true);
                break;
        }
    }

    public static void AddStoreGoodsKey(StoreGoods storeGoods, int id, bool bShow = false)
    {
        if (id == 0) return;
        if(storeGoods != null) storeGoods.gameObject.SetActive(bShow);
        if (!bShow)
        {
          //  RestaurantManager.tutorialKeys.Add(id);
            RestaurantManager.tutorialEventKeys.Add((TutorialEventKey)id);
        }
        else if (RestaurantManager.tutorialEventKeys.Contains((TutorialEventKey)id))
        {
            RestaurantManager.tutorialEventKeys.Remove((TutorialEventKey)id);

        }
        switch((TutorialEventKey)id)
        {
            case TutorialEventKey.NoEmployee:
                {
                    if (!bShow) GameInstance.GameIns.applianceUIManager.UnlockHire(false);
                    else GameInstance.GameIns.applianceUIManager.UnlockHire(true);
                                   
                    break;
                }
            case TutorialEventKey.NextMap:
                GameInstance.GameIns.uiManager.worldBtn.gameObject.SetActive(true);
                break;
        }
        if(storeGoods != null) GameInstance.GameIns.store.Refresh();
    }
}
