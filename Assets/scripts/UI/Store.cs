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
    Dictionary<int, GoodsStruct> goodsStructs = new Dictionary<int, GoodsStruct>();
    // Dictionary<WorkSpaceType, StoreGoods> goodsDic = new Dictionary<WorkSpaceType, StoreGoods>();
    Dictionary<int, StoreGoods> goodsList = new Dictionary<int, StoreGoods>();

    public Dictionary<int, PlaceController> goodsPreviewDic = new Dictionary<int, PlaceController>();
    public Dictionary<int, GameObject> goodsDic = new Dictionary<int, GameObject>();
    
    public PlaceController currentPreview;

    public static RectTransform instancingImage;
    public static GameObject storeObjects;
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
            GameObject g = Instantiate(loadedAssets[itemAssetKeys[v.Key].ID], storeObjects.transform);
            g.transform.position = Vector3.zero;
            PlaceController preview = Instantiate(loadedAssets[itemAssetKeys[v.Key + 1000].ID], storeObjects.transform).GetComponent<PlaceController>();
            preview.transform.position = Vector3.zero;
            g.SetActive(false);
            preview.gameObject.SetActive(false);   
            goodsDic[v.Key] = g;
            goodsPreviewDic[v.Key + 1000] = preview;

            
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
            if(d.Value.type == WorkSpaceType.Counter) g.gameObject.SetActive(true);
            else g.gameObject.SetActive(false); 
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

        foreach(var g in goodsList)
        {
            if (g.Value.goods.type == type)
            {
                g.Value.gameObject.SetActive(true);
            }
            else
            {
                g.Value.gameObject.SetActive(false);

            }
        }
     
        scrollbar.value = 0;
    }
}
