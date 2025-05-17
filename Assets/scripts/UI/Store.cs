using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public RectTransform content;
    public RectTransform subCanvas;
    public ScrollRect contentRect;
    public StoreGoods goods;
    public RectTransform instanceImage;
    public Scrolling scrolling;
    public Scrollbar scrollbar;
    Dictionary<int, GoodsStruct> goodsStructs = new Dictionary<int, GoodsStruct>();
   // Dictionary<WorkSpaceType, StoreGoods> goodsDic = new Dictionary<WorkSpaceType, StoreGoods>();
    List<StoreGoods> goodsList = new List<StoreGoods>();
    private void Awake()
    {
        GameInstance.GameIns.store = this;
    }

    public void NewGoods(Dictionary<int, GoodsStruct> goodsStruct)
    {
        goodsStructs = goodsStruct;
       
      
        foreach (var d in goodsStruct)
        {
            StoreGoods g = Instantiate(goods, content);
            goodsList.Add(g);
            g.Setup(d.Value);
            if(d.Value.type == WorkSpaceType.Counter) g.gameObject.SetActive(true);
            else g.gameObject.SetActive(false); 
        }
    }
    public void CloseStore()
    {
        scrolling.Shut();
    }
    public void ChangeCounterList()
    {
        ChangeList(WorkSpaceType.Counter);
    }
    public void ChangeMachineList()
    {
        ChangeList(WorkSpaceType.FoodMachine);
    }
    public void ChangeTableList()
    {

        ChangeList(WorkSpaceType.Table);
    }
    public void ChangeExpands()
    {
        ChangeList(WorkSpaceType.None);
    }
    public void ChangeList(WorkSpaceType type)
    {
        for (int i = 0; i < goodsList.Count; i++)
        {
            if (goodsList[i].goods.type == type)
            {
                goodsList[i].gameObject.SetActive(true);
            }
            else
            {
                goodsList[i].gameObject.SetActive(false);

            }
        }
        scrollbar.value = 0;
    }
}
