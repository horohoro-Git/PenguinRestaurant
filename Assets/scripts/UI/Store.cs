using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AssetLoader;

public class Store : MonoBehaviour
{
    public RectTransform content;
    public RectTransform subCanvas;
    public ScrollRect contentRect;
    public StoreGoods goods;
    public RectTransform instanceImage;
    public Scrolling scrolling;
    public Scrollbar scrollbar;
    public ScrollRect scrollRect;
    Dictionary<int, GoodsStruct> goodsStructs = new Dictionary<int, GoodsStruct>();
    // Dictionary<WorkSpaceType, StoreGoods> goodsDic = new Dictionary<WorkSpaceType, StoreGoods>();
    Dictionary<int, StoreGoods> goodsList = new Dictionary<int, StoreGoods>();

    public Dictionary<int, PlaceController> goodsPreviewDic = new Dictionary<int, PlaceController>();
    public Dictionary<int, Queue<GameObject>> goodsDic = new Dictionary<int, Queue<GameObject>>();
    
    public PlaceController currentPreview;

    public static RectTransform instancingImage;
    public static GameObject storeObjects;

    WorkSpaceType spaceType = WorkSpaceType.Counter;

    public HashSet<int> require = new HashSet<int>();
    private void Awake()
    {
        storeObjects = new GameObject();
        storeObjects.name = "StoreObjects";
        storeObjects.transform.position = Vector3.zero;
        GameInstance.GameIns.store = this;

        instancingImage = Instantiate(instanceImage, subCanvas.transform);
        instancingImage.gameObject.SetActive(false);
        foreach (var v in AssetLoader.goods)
        {
            if (v.Value.type != WorkSpaceType.None)
            {
                goodsDic[v.Key] = new Queue<GameObject>();
                for (int i = 0; i < v.Value.num; i++)
                {
                    GameObject g = Instantiate(loadedAssets[itemAssetKeys[v.Key].ID], storeObjects.transform);
                    g.transform.position = Vector3.zero;
                    g.SetActive(false);
                    goodsDic[v.Key].Enqueue(g);
                }
                PlaceController preview = Instantiate(loadedAssets[itemAssetKeys[v.Key + 1000].ID], storeObjects.transform).GetComponent<PlaceController>();
                preview.gameObject.SetActive(false);
                preview.transform.position = Vector3.zero;
                goodsPreviewDic[v.Key + 1000] = preview;

            }
        }
    }

    public void NewGoods(Dictionary<int, GoodsStruct> goodsStruct)
    {
        goodsStructs = goodsStruct;
        
      
        foreach (var d in goodsStruct)
        {
            StoreGoods g = Instantiate(goods, content);
            goodsList[d.Key] = g;
            g.Setup(d.Value);
            if (d.Value.type == WorkSpaceType.Counter) g.gameObject.SetActive(true);
            else g.gameObject.SetActive(false);
            if (d.Value.type != WorkSpaceType.None)
            {
                goodsPreviewDic[d.Key + 1000].storeGoods = g;
            }
        }
    }

    public PlaceController GetGoods(int id)
    {
        if (currentPreview != null)
        {
            if(currentPreview.storeGoods.currnet)
            {
                if(currentPreview.storeGoods.currnet.purchasedObject)
                {
                    currentPreview.storeGoods.currnet.currentFurniture.gameObject.SetActive(true);
                    GameInstance.GameIns.gridManager.Revert(currentPreview.storeGoods.currnet);
                }
                currentPreview.storeGoods.currnet.purchasedObject = false;
                currentPreview.storeGoods.currnet.currentFurniture = null;
                currentPreview.storeGoods.currnet.gameObject.SetActive(false);
                currentPreview.storeGoods.currnet = null;
            }
            currentPreview.Cancel();
        }
        goodsList[id].currnet = goodsPreviewDic[id + 1000];
        currentPreview = goodsPreviewDic[id + 1000]; 
        return currentPreview;
    }
    public void CloseStore()
    {
        scrolling.Shut();
    }
    public void ChangeCounterList()
    {
        spaceType = WorkSpaceType.Counter;
        ChangeList(WorkSpaceType.Counter);
    }
    public void ChangeMachineList()
    {
        spaceType = WorkSpaceType.FoodMachine;
        ChangeList(WorkSpaceType.FoodMachine);
    }
    public void ChangeTableList()
    {
        spaceType = WorkSpaceType.Table;
        ChangeList(WorkSpaceType.Table);
    }

    public void ChangeTrashcanList()
    {
        spaceType = WorkSpaceType.Trashcan;
        ChangeList(WorkSpaceType.Trashcan);
    }
    public void ChangeExpands()
    {
        spaceType = WorkSpaceType.None;
        ChangeList(WorkSpaceType.None);
    }
    public void ChangeList(WorkSpaceType type)
    {
      
        foreach (var g in goodsList)
        {
            if (g.Value.goods.type == type)
            {
                if(g.Value.goods.require == 0 || require.Contains(g.Value.goods.require))
                {
                    g.Value.gameObject.SetActive(true);
                }
                else
                {
                    g.Value.gameObject.SetActive(false);
                }
            }
            else
            {
                g.Value.gameObject.SetActive(false);

            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        // 위치 재설정
        scrollRect.horizontalNormalizedPosition = 0f;
    }

    public void Refresh()
    {
        ChangeList(spaceType);
    }


    public void StoreUpdate()
    {
        foreach(var g in goodsList)
        {
            if(g.Value.goods.type != 0)
            {
                int num = goodsDic[g.Key].Count;
                if (num == 0)
                {
                    g.Value.goods.soldout = true;
                    g.Value.soldout_text.gameObject.SetActive(true);
                }
                g.Value.UpdatePrice(num);
            }
        }
        Refresh();
    }


    public void Extended(int id)
    {
        goodsList[id].goods.soldout = true;
        goodsList[id].soldout_text.gameObject.SetActive(true);

    }
}
