using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameInstance;
using static AssetLoader;
using static Store;
using System;
using TMPro;

public class StoreGoods : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject furniture;
    public GameObject furniture_Preview;
    public PlaceController currnet;
    public RectTransform rect;
    public RectTransform itemImage;

    public RectTransform instaceImage;
    Vector3 origin;
    bool currentCheckArea = false;
    public TMP_Text image_name;
    public TMP_Text price_text;
    [NonSerialized]
    public GoodsStruct goods;


   
    void Awake()
    {
        rect = GetComponent<RectTransform>();
      
    }
    public void Setup(GoodsStruct goods)
    {
  
        image_name.font = font;
        image_name.fontMaterial = font_mat;
        this.goods = goods;
        image_name.text = goods.name;
        itemImage.GetComponent<Image>().sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[goods.id].ID);
       // instaceImage = Instantiate(GameIns.store.instanceImage, GameIns.store.subCanvas.transform);
      //  instaceImage.gameObject.SetActive(false);
        //instaceImage.GetComponent<Image>().sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[goods.id].ID);
        // furniture = GameIns.store.goodsDic[goods.id];
        // /   
        //   if(itemAssetKeys.ContainsKey(goods.id + 1000)) furniture_Preview = loadedAssets[itemAssetKeys[goods.id + 1000].ID];
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        currentCheckArea = false;
        GameIns.inputManager.inputDisAble = true;
     
        if(GameIns.store.currentPreview != null)
        {
            GameIns.store.currentPreview.Cancel();
            GameIns.gridManager.VisibleGrid(false);
        }
        InputManger.cachingCamera.GetComponent<OrthographicCamera>().ZoomOut();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameIns.inputManager.CheckClickedUI(1 << 18))
        {
            if(!instancingImage.gameObject.activeSelf)
            {
                instancingImage.gameObject.SetActive(true);
                instancingImage.GetComponent<Image>().sprite = itemImage.GetComponent<Image>().sprite;
                instancingImage.gameObject.layer = 14;
                instancingImage.GetComponent<Image>().raycastTarget = true;
                instancingImage.position = Input.mousePosition;
            }
            else instancingImage.position = Input.mousePosition;
          //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);

        }
        else
        {
            if (instancingImage.gameObject.activeSelf)
            {
                instancingImage.gameObject.SetActive(false);
                GameIns.store.CloseStore();
            }
            if(currnet == null)
            {
                currnet = GameIns.store.goodsPreviewDic[goods.ID + 1000];
                currnet.gameObject.SetActive(true);
                currnet.storeGoods = this;
                GameIns.store.currentPreview = currnet;

                GameIns.gridManager.VisibleGrid(true);
            }
         //   instaceImage.SetParent(itemImage);
         //   instaceImage.position = itemImage.position;
            Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
            {
                //    currnet.transform.position = hit.point;
                currentCheckArea = GameIns.gridManager.SelectLine(hit.point, currnet, currentCheckArea);
            }
        //    currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);
        
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //  if (currnet != null) Destroy(currnet);
        //currnet = null;
        if (currentCheckArea)
        {
            Debug.Log("Success");

        }
        else
        {
            Debug.Log("Fail");
        }

        if (instancingImage != null)
        {
            instancingImage.gameObject.SetActive(false);
          //  Destroy(instaceImage.gameObject);
            //instaceImage = null;
        }
        GameIns.inputManager.inputDisAble = false;
    }


    public void RemoveGoodsPreview()
    {
        if (currnet != null)
        {
            if(currnet.purchasedObject)
            {
                currnet.currentFurniture.gameObject.SetActive(true);
            }
            currnet.purchasedObject = false;    
            currnet.currentFurniture = null;    
            currnet.gameObject.SetActive(false);
            //Destroy(currnet.gameObject);
        }
        currnet = null;
    }

    public void PlaceGoods(Vector3 offset)
    {
        if(currnet)
        {
            GameObject go = GameIns.store.goodsDic[goods.ID]; //Instantiate(furniture);
            Furniture f = go.GetComponent<Furniture>();
            f.spawned = true;
          //  GameInstance.GameIns.workSpaceManager.AddWorkSpace(f);
           
            Vector3 target = currnet.transform.position;
            go.transform.position = target;
            f.id = goods.ID;
            if (go.TryGetComponent<IObjectOffset>(out IObjectOffset offsets))
            {
                offsets.offset.localPosition = offset;
                offsets.offset.rotation = currnet.offset.transform.rotation;
            }
            else
            {
                go.transform.rotation = currnet.offset.transform.rotation;
            }

            go.SetActive(true);

            GameIns.restaurantManager.ApplyPlaced(f);
        }
    }

    public void Purchase()
    {
      //  GameInstance.GameIns.inputManager.draggingFurniture = 
    }
}
