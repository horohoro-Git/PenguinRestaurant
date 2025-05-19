using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameInstance;
using static AssetLoader;
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
        instaceImage = Instantiate(GameIns.store.instanceImage, GameIns.store.subCanvas.transform);
        instaceImage.gameObject.SetActive(false);
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
        }
        // origin = instaceImage.position;
        //     instaceImage = Instantiate(GameIns.store.instanceImage, GameIns.store.subCanvas.transform);
        //   instaceImage.GetComponent<Image>().sprite = itemImage.GetComponent<Image>().sprite;
        //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameIns.inputManager.CheckClickedUI(1 << 14))
        {
            if(!instaceImage.gameObject.activeSelf)
            {
                instaceImage.gameObject.SetActive(true);
                instaceImage.GetComponent<Image>().sprite = itemImage.GetComponent<Image>().sprite;
                instaceImage.gameObject.layer = 14;
                instaceImage.GetComponent<Image>().raycastTarget = true;
                instaceImage.position = Input.mousePosition;
            }
            else instaceImage.position = Input.mousePosition;
          //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);

        }
        else
        {
            if (instaceImage.gameObject.activeSelf)
            {
                instaceImage.gameObject.SetActive(false);
                GameIns.store.CloseStore();
            }
            if(currnet == null)
            {
                currnet = GameIns.store.goodsPreviewDic[goods.ID + 1000];
                currnet.gameObject.SetActive(true);
                currnet.storeGoods = this;
                GameIns.store.currentPreview = currnet;
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

        if (instaceImage != null)
        {
            instaceImage.gameObject.SetActive(false);
          //  Destroy(instaceImage.gameObject);
            //instaceImage = null;
        }
        GameIns.inputManager.inputDisAble = false;
    }


    public void RemoveGoodsPreview()
    {
        if (currnet != null)
        {
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
            go.SetActive(true);
            Vector3 target = currnet.transform.position;
            go.transform.position = target;
            if (go.TryGetComponent<IObjectOffset>(out IObjectOffset offsets))
            {
                offsets.offset.localPosition = offset;
                offsets.offset.rotation = currnet.offset.transform.rotation;
            }
            else
            {
                go.transform.rotation = currnet.offset.transform.rotation;
            }
         
        }
    }

    public void Purchase()
    {
      //  GameInstance.GameIns.inputManager.draggingFurniture = 
    }
}
