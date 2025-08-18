using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Tutorials
{

    public int id;
    public bool worked;
    public int count;

    public Tutorials(int id, bool worked)
    {
        this.id = id;
        this.worked = worked;
    }


    public static void Setup(Tutorials tutos)
    {
       // if (tutos.worked)
        {

            List<TutorialStruct> list = GameInstance.GameIns.restaurantManager.tutorialStructs[tutos.id];
            string methodName = $"Event_{list[list.Count - 1].event_id}";
          

            MethodInfo method = typeof(Tutorials).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                var del = Delegate.CreateDelegate(typeof(Action<int>), method);
                EventManager.AddTutorialEvent(list[list.Count - 1].event_id, del);

                string methodName2 = methodName + "0";
                MethodInfo method2 = typeof(Tutorials).GetMethod(methodName2, BindingFlags.Public | BindingFlags.Static);
                if(method2 != null)
                {
                    var delv = Delegate.CreateDelegate(typeof(Action<int>), method2);
                    ((Action<int>)delv)?.Invoke(list[list.Count - 1].event_id);
                }

            }
        }
    }

    //카운터 구매 
    public static void Event_90101(int id)
    {
      

        if (id != 1001) return;

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
    public static void Event_90104(int id)
    {
        if (id != 1007) return;

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
        if (id != 1003) return;

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
        if (id != 2000) return;

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
        if (id != 3000) return;

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
        if (id != 4000) return;

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
        if (id != 6000) return;
        if (RestaurantManager.tutorialKeys.Contains(6000)) RestaurantManager.tutorialKeys.Remove(6000);
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
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[list[0].event_id].text;
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
        Debug.Log("AAA");
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
            Setup(tuto);
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
        GameInstance.GameIns.uiManager.tutoText2.text = App.languages[list[0].event_id].text;
        //  GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
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
            Setup(tuto);
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
        Debug.Log("L");
    }

    //낚시 완료
    public static void Event_90114(int id)
    {
        if (id != 10000) return;
        if (RestaurantManager.tutorialKeys.Contains(10000)) RestaurantManager.tutorialKeys.Remove(10000);

        GameInstance.GameIns.uiManager.tutoText2.text = "";
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
     
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
    public static void TutorialUpdate()
    {
        GameInstance.GameIns.uiManager.tutoText2.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.UnlockHire(false);
        GameInstance.GameIns.uiManager.changeScene.gameObject.SetActive(false);
        GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(false);
        int id = GameInstance.GameIns.restaurantManager.tutorials.id;

        //기능 제한
        Dictionary<int, StoreGoods> goods = GameInstance.GameIns.store.goodsList;
        switch (id)
        {
            case 0:
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
                
                break;
        }
    }

    public static void AddStoreGoodsKey(StoreGoods storeGoods, int id, bool bShow = false)
    {
        if(storeGoods != null) storeGoods.gameObject.SetActive(bShow);
        if (!bShow) RestaurantManager.tutorialKeys.Add(id);
        else if (RestaurantManager.tutorialKeys.Contains(id)) RestaurantManager.tutorialKeys.Remove(id);
    }
}
