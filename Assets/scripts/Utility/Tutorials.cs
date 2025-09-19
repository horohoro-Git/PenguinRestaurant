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

    //카운터 구매 
    public static void Event_90101(TutorialEventKey id)
    {
      

        if (id != TutorialEventKey.BuyCounter) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 1;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }
    public static void Event_901010(int id)
    {
        //
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);

        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        GameInstance.GameIns.store.Refresh();
    }

    //테이블 구매
    public static void Event_90104(TutorialEventKey id)
    {
        if (id != TutorialEventKey.BuyTable) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 2;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;

    }
    public static void Event_901040(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        GameInstance.GameIns.store.Refresh();
    }

    //조리기구 구매
    public static void Event_90105(int id)
    {
        if (id != (int)TutorialEventKey.BuyMachine) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 3;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;

    }
    public static void Event_901050(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        GameInstance.GameIns.store.Refresh();
    }

    //뽑기 씬 입장 하기
    public static void Event_90106(int id)
    {
        if (id != 3000) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 4;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;

    }
    public static void Event_901060(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        GameInstance.GameIns.store.Refresh();
    }


    //뽑기
    public static void Event_90107(int id)
    {
        if (id != (int)TutorialEventKey.DrawGatcha) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 5;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;

    }
    public static void Event_901070(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
     //   RestaurantManager.tutorialKeys.Remove(3000);
        GameInstance.GameIns.store.Refresh();
    }

    //집으로 귀환
    public static void Event_90108(int id)
    {
        if (id != (int)TutorialEventKey.EnterRestaurant) return;

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 6;
        tuto.count = 0;
     
  /*      AnimalSpawner[] animalSpawners = GameInstance.GameIns.restaurantManager.door.GetComponentsInChildren<AnimalSpawner>();
        foreach(var v in animalSpawners)
        {
            if(v.type == AnimalSpawner.SpawnerType.FastFood)
            {
                v.TutorialSpawnAnimal().Forget();
              
                break;
            }
        }*/
       
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
        AnimalSpawner[] spawners = AnimalSpawner.FindObjectsOfType<AnimalSpawner>();

        foreach (var v in spawners)
        {
            if (v.type == AnimalSpawner.SpawnerType.FastFood)
            {
                v.TutorialSpawnAnimal(App.GlobalToken).Forget();
            }
        }
    }
    public static void Event_901080(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;

        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        RestaurantManager.tutorialKeys.Remove(3000);
        GameInstance.GameIns.store.Refresh();
    }

    //직원 고용하기
    public static void Event_90109(int id)
    {
        if (id != (int)TutorialEventKey.HireEmployee) return;
        if (RestaurantManager.tutorialKeys.Contains(id)) RestaurantManager.tutorialKeys.Remove(id);
        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 7;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
        }
        else
        {
            tuto.worked = true;
        }
        Setup(tuto);
        GameInstance.GameIns.uiManager.TutorialEnd(true);
        //   GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        //   tuto.count++;

    }
    public static void Event_901090(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
      //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if(RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if(RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(6000);
        GameInstance.GameIns.applianceUIManager.UnlockHire(true);
        
        GameInstance.GameIns.store.Refresh();

        
    }

    //생선 충전하기
    public static void Event_90111(int id)
    {
        if (id != 7000) return;
        if (RestaurantManager.tutorialKeys.Contains(7000)) RestaurantManager.tutorialKeys.Remove(7000);
   
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 8;
        tuto.count = 0;

        List<TutorialStruct> list = GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id];
   //     GameInstance.GameIns.uiManager.tutoText2.text = App.languages[list[0].event_id].text;
        //  GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            
        }
        else
        {
            tuto.worked = true;
        }
        Setup(tuto);
        GameInstance.GameIns.uiManager.TutorialEnd(true);
        //  GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        //  tuto.count++;
    }
    public static void Event_901110(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(7000);//물고기 충전
        GameInstance.GameIns.applianceUIManager.UnlockHire(true);

        GameInstance.GameIns.store.Refresh();
    }
    //낚시 이동
    public static void Event_90112(int id)
    {
        if (id != 8000) return;
        if (RestaurantManager.tutorialKeys.Contains(8000)) RestaurantManager.tutorialKeys.Remove(8000);
        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        tuto.id = 9;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }
    public static void Event_901120(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
       // RestaurantManager.tutorialKeys.Add(6000);//물고기 충전
        RestaurantManager.tutorialKeys.Add(8000);
       
        GameInstance.GameIns.store.Refresh();
    }

    //낚시 시작
    public static void Event_90113(int id)
    {
        if (id != 9000) return;
        if (RestaurantManager.tutorialKeys.Contains(9000)) RestaurantManager.tutorialKeys.Remove(9000);
        List<TutorialStruct> list = GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id];
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
      //  GameInstance.GameIns.uiManager.tutoText2.text = App.languages[list[0].event_id].text;
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        tuto.id = 10;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }

    public static void Event_901130(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(9000);
      
        GameInstance.GameIns.store.Refresh();
    }

    //낚시 완료
    public static void Event_90114(int id)
    {
        if (id != 10000) return;
      //  if (RestaurantManager.tutorialKeys.Contains(9000)) RestaurantManager.tutorialKeys.Remove(9000);
        if (RestaurantManager.tutorialKeys.Contains(10000)) RestaurantManager.tutorialKeys.Remove(10000);

        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);

        tuto.id = 11;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }
    public static void Event_901140(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(10000);
        GameInstance.GameIns.store.Refresh();
    }
    //가게로 귀환
    public static void Event_90115(int id)
    {
        if (id != 11000) return;
        if (RestaurantManager.tutorialKeys.Contains(10000)) RestaurantManager.tutorialKeys.Remove(10000);
        if (RestaurantManager.tutorialKeys.Contains(11000)) RestaurantManager.tutorialKeys.Remove(11000);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 12;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }


    public static void Event_901150(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(11000);
        GameInstance.GameIns.store.Refresh();
    }

    //쥐 잡기
    public static void Event_90116(int id)
    {
        if (id != 12000) return;
        if (RestaurantManager.tutorialKeys.Contains(12000)) RestaurantManager.tutorialKeys.Remove(12000);
        if (RestaurantManager.tutorialKeys.Contains(500)) RestaurantManager.tutorialKeys.Remove(500);

        GameInstance.GameIns.inputManager.InputDisAble = false;
        GameInstance.GameIns.playerCamera.followTarget = null;
        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 13;
        tuto.count = 0;
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
     //       Setup(tuto);
        }
        RestaurantManager.tutorialKeys.Add(13000);
        GameInstance.GameIns.uiManager.TutorialEnd(true);
        // GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        // tuto.count++;
    }

    public static void Event_901160(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(12000);
        EnemySpawner enemySpawner = GameInstance.GameIns.restaurantManager.door.GetComponentInChildren<EnemySpawner>();
        GameObject target = enemySpawner.SpawnEnemyTutorial();
        GameInstance.GameIns.playerCamera.followTarget = target;
        GameInstance.GameIns.inputManager.InputDisAble = true;
        GameInstance.GameIns.store.Refresh();
    }

    //쓰레기통 배치
    public static void Event_90117(int id)
    {
        if (!(id == 13000 || id == 1008)) return;
        if (RestaurantManager.tutorialKeys.Contains(13000)) RestaurantManager.tutorialKeys.Remove(13000);
      
        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 14;
        tuto.count = 0;
       
        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }

        GameInstance.GameIns.uiManager.TutorialEnd(true);
        //GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        // tuto.count++;

        AnimalSpawner[] spawners = AnimalSpawner.FindObjectsOfType<AnimalSpawner>();

        foreach (var v in spawners)
        {
            if (v.type == AnimalSpawner.SpawnerType.FastFood)
            {
                v.TutorialSpawnAnimal(App.GlobalToken).Forget();
            }
        }
    }

    public static void Event_901170(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        AddStoreGoodsKey(goods[1008], 1008, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(13000);
        GameInstance.GameIns.store.Refresh();
    }

    //직접 쓰레기 줍기
    public static void Event_90119(int id)
    {
        if (id != 14000) return;
        if (RestaurantManager.tutorialKeys.Contains(14000)) RestaurantManager.tutorialKeys.Remove(14000);
        if (RestaurantManager.tutorialKeys.Contains(600)) RestaurantManager.tutorialKeys.Remove(600);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 15;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }

    public static void Event_901190(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        AddStoreGoodsKey(goods[1008], 1008, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
  //      if (RestaurantManager.tutorialKeys.Contains(300)) RestaurantManager.tutorialKeys.Remove(300);
        RestaurantManager.tutorialKeys.Add(14000);
      //  RestaurantManager.tutorialKeys.Add(600);
        GameInstance.GameIns.store.Refresh();
    }

    //쓰레기통 미니게임
    public static void Event_90120(int id)
    {
        if (id != 15000) return;
        if (RestaurantManager.tutorialKeys.Contains(15000)) RestaurantManager.tutorialKeys.Remove(15000);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 16;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }

    public static void Event_901200(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(15000);
        GameInstance.GameIns.store.Refresh();
    }

    //1차 튜토리얼 종료
    public static void Event_90121(int id)
    {
        if (id != 16000) return;
        if (RestaurantManager.tutorialKeys.Contains(16000)) RestaurantManager.tutorialKeys.Remove(16000);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

        tuto.id = 17;
        tuto.count = 0;

        if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }
        GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        tuto.count++;
    }

    public static void Event_901210(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(16000);
        GameInstance.GameIns.store.Refresh();
    }

    //새로운 맵
    public static void Event_90123(int id)
    {
        if (id != 16000) return;
        if (RestaurantManager.tutorialKeys.Contains(16000)) RestaurantManager.tutorialKeys.Remove(16000);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
       // Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;

       // tuto.id = 17;
    //    tuto.count = 0;

      /*  if (1 == GameInstance.GameIns.restaurantManager.tutorialStructs[GameInstance.GameIns.restaurantManager.tutorials.id].Count)
        {
            tuto.worked = false;
            Setup(tuto);
        }
        else
        {
            tuto.worked = true;
        }*/
        // GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        // tuto.count++;
    }

    public static void Event_901230(int id)
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[id].text;
        //  GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(true);
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        AddStoreGoodsKey(goods[1001], 1001, true);
        AddStoreGoodsKey(goods[1002], 1002, true);
        AddStoreGoodsKey(goods[1007], 1007, true);
        AddStoreGoodsKey(goods[1003], 1003, true);
        if (RestaurantManager.tutorialKeys.Contains(3000)) RestaurantManager.tutorialKeys.Remove(3000);
        if (RestaurantManager.tutorialKeys.Contains(1000)) RestaurantManager.tutorialKeys.Remove(1000);
        RestaurantManager.tutorialKeys.Add(16000);
        GameInstance.GameIns.store.Refresh();
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
