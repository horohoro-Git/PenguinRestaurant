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
using UnityEngine.InputSystem;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using System.Text;

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
    public TMP_Text soldout_text;
    [NonSerialized]
    public GoodsStruct goods;


    StringBuilder stringBuilder = new StringBuilder();
    void Awake()
    {
        rect = GetComponent<RectTransform>();
      
    }
    public void Setup(GoodsStruct goods)
    {
  
        image_name.font = font;
        image_name.fontMaterial = font_mat;
        this.goods = goods;
        image_name.text = App.gameSettings.language == Language.KOR ? goods.name_kor : goods.name_eng;
        price_text.text = goods.Price;
        this.goods.defaultPrice = goods.Price;
        itemImage.GetComponent<Image>().sprite = loadedAtlases["Furnitures"].GetSprite(spriteAssetKeys[goods.id].ID);

        if (goods.type != WorkSpaceType.None && GameIns.store.goodsDic[goods.id].Count == 0)
        {
            this.goods.soldout = true;
            soldout_text.gameObject.SetActive(true);
        }

    }

    public void UpdatePrice(int remains)
    {
        if (remains == 0)
        {
            price_text.gameObject.SetActive(false);
        }
        else
        {
            int count = goods.num - remains;
            if (count == 0 && goods.sale != "0")
            {
                price_text.text = App.gameSettings.language == Language.KOR ? "����" : "Free";
                goods.Price = "0";
            }
            else
            {
              
                BigInteger bigInteger = Utility.StringToBigInteger(goods.defaultPrice);
                int pow = (int)(goods.pow * 100);
                bigInteger = bigInteger + (bigInteger * pow * (count)) / 100;
                stringBuilder = Utility.GetFormattedMoney(bigInteger, stringBuilder);
                price_text.text = stringBuilder.ToString();
                goods.Price = stringBuilder.ToString();
            }
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(goods.Price_Value > GameIns.restaurantManager.restaurantCurrency.Money) return;
      
        currentCheckArea = false;
        GameIns.inputManager.inputDisAble = true;
     
        if(GameIns.store.currentPreview != null)
        {
            GameIns.store.currentPreview.Cancel();
            GameIns.gridManager.VisibleGrid(false);
        }
        if (GameIns.restaurantManager.doorPreview.gameObject.activeSelf)
        {
            GameIns.restaurantManager.doorPreview.Cancel();
        }
      //if(!goods.soldout) InputManger.cachingCamera.GetComponent<OrthographicCamera>().ZoomOut();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (goods.Price_Value > GameIns.restaurantManager.restaurantCurrency.Money) return;
        if (!goods.soldout)
        {
            if (GameIns.inputManager.CheckClickedUI(1 << 18))
            {
                Vector3 screenPos;
#if UNITY_ANDROID || UNITY_IOS
                screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
                screenPos = Mouse.current.position.ReadValue();
#endif
                if (!instancingImage.gameObject.activeSelf)
                {
                    instancingImage.gameObject.SetActive(true);
                    instancingImage.GetComponent<Image>().sprite = itemImage.GetComponent<Image>().sprite;
                    instancingImage.gameObject.layer = 14;
                    instancingImage.GetComponent<Image>().raycastTarget = true;
                    instancingImage.position = screenPos;
                }
                else instancingImage.position = screenPos;
                //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);

            }
            else
            {

                if (goods.type != WorkSpaceType.None)
                {
                    if (instancingImage.gameObject.activeSelf)
                    {
                        instancingImage.gameObject.SetActive(false);
                        GameIns.store.CloseStore();
                    }
                    if (currnet == null)
                    {
                        currnet = GameIns.store.goodsPreviewDic[goods.ID + 1000];
                        currnet.spawnAnimation = true;
                        currnet.gameObject.SetActive(true);
                        currnet.storeGoods = this;
                        GameIns.store.currentPreview = currnet;

                        GameIns.gridManager.VisibleGrid(true);
                    }

                    Vector3 screenPos;
#if UNITY_ANDROID || UNITY_IOS
                    screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
                screenPos = Mouse.current.position.ReadValue();
#endif

                    Ray r = InputManger.cachingCamera.ScreenPointToRay(screenPos);
                    if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
                    {
                        currentCheckArea = GameIns.gridManager.SelectLine(hit.point, currnet, currentCheckArea, goods.type);
                    }
                }
                else
                {
                    if (instancingImage.gameObject.activeSelf)
                    {
                        instancingImage.gameObject.SetActive(false);
                        BigInteger price = Utility.StringToBigInteger(goods.Price);
                        if (GameIns.restaurantManager.restaurantCurrency.Money >= price)
                        {
                            SoundManager.Instance.PlayAudio(GameIns.uISoundManager.FurniturePurchase(), 0.2f);
                            GameIns.restaurantManager.GetMoney((-price).ToString());
                            GameIns.restaurantManager.restaurantCurrency.changed = true;
                            GameIns.restaurantManager.Extension();

                            if ((GameIns.restaurantManager.employees.num < 8 && GameIns.restaurantManager.employeeHire[GameIns.restaurantManager.employees.num] <= GameIns.restaurantManager.GetRestaurantValue()))
                            {
                                GameIns.applianceUIManager.UnlockHire(true);
                            }
                            else
                            {
                                GameIns.applianceUIManager.UnlockHire(false);
                            }

                            goods.soldout = true;
                            soldout_text.gameObject.SetActive(true);
                            itemImage.GetComponent<Image>().raycastTarget = false;
                            GameIns.store.require.Add(goods.ID);
                            GameIns.store.Refresh();
                        }
                    }
                }

            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
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
                currnet.currentFurniture.placed = true;
            }
            currnet.purchasedObject = false;    
            currnet.currentFurniture = null;    
            currnet.gameObject.SetActive(false);
            //Destroy(currnet.gameObject);
        }
        currnet = null;
    }

    public void PlaceGoods(Vector3 offset, int level, Furniture currentFurniture)
    {
        if(currnet)
        {
            //GameObject go =  //GameIns.store.goodsDic[goods.ID].Dequeue(); //Instantiate(furniture);
            Furniture f = currentFurniture; //go.GetComponent<Furniture>();
            if(f == null) f = GameIns.store.goodsDic[goods.ID].Dequeue().GetComponent<Furniture>();
            bool firstPlaced = !f.spawned;

            f.rotateLevel = level;
            Vector3 target = currnet.transform.position;
            f.transform.position = target;
            f.originPos = target;
            f.id = goods.ID;
            if (f.TryGetComponent<IObjectOffset>(out IObjectOffset offsets))
            {
             //   f.offsetPoint = 
                offsets.offset.localPosition = offset;
                offsets.offset.rotation = currnet.offset.transform.rotation;
            }
            else
            {
                f.transform.rotation = currnet.offset.transform.rotation;
            }

            f.placed = true;
            f.gameObject.SetActive(true);

            GameIns.restaurantManager.ApplyPlaced(f);

            if (firstPlaced)
            {
                if(f.TryGetComponent<FoodMachine>(out FoodMachine fm))
                {
                    fm.Set(false);
                }
            }
            SaveLoadSystem.SaveRestaurantBuildingData();
            if (GameIns.store.goodsDic[goods.ID].Count == 0)
            {
                goods.soldout = true;
                soldout_text.gameObject.SetActive(true);
            }
            GameIns.store.require.Add(goods.ID);
            GameIns.store.Refresh();
        }
    }

    public void Purchase()
    {
        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.FurniturePurchase(), 0.2f);
      //  GameIns.restaurantManager.restaurantCurrency.Money -= goods.Price_Value;
        GameIns.restaurantManager.GetMoney((-goods.Price_Value).ToString());
        SaveLoadSystem.SaveRestaurantCurrency(GameIns.restaurantManager.restaurantCurrency);
        int num = GameIns.store.goodsDic[goods.id].Count;

        if (num == 0) price_text.gameObject.SetActive(false); else UpdatePrice(num);

        if ((GameIns.restaurantManager.employees.num < 8 && GameIns.restaurantManager.employeeHire[GameIns.restaurantManager.employees.num] <= GameIns.restaurantManager.GetRestaurantValue()))
        {
            GameIns.applianceUIManager.UnlockHire(true);
        }
        else
        {
            GameIns.applianceUIManager.UnlockHire(false);
        }
        //  GameInstance.GameIns.inputManager.draggingFurniture = 
    }
}
