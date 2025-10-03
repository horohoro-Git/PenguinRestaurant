using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using static AssetLoader;
using Random = UnityEngine.Random;
public class ApplianceUIManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public TMP_Text text;
  
    public GameObject rootUI;
    public GameObject appliancePanel;
    public Image applianceImage;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI upgradeInfoText;

    public Button hireBtn;
    public Button rewardChestBtn;
    public GameObject trashCan;
    public GameObject rewardChest;
    public FullFilledTrashcan rewardChest_Fill;
    public RewardTrashcan clickerReward;
    public GameObject otherUI;
    public Store shopUI;
    public FurnitureInfo furnitureUI;
    RectTransform hireTransform;

    public Button upgradeButton;
    public Sprite employeeIcon;
    public Sprite[] machines;
   
    public TMP_Text feedingDescription;

    public GameObject fishGO;

    public RewardsBox currentBox;
    public List<RewardsBox> rewardsBoxes = new List<RewardsBox>();
    public CanvasGroup othersGroup;
    public CanvasGroup storeGroup;
    public CanvasGroup machineStatusGroup;
  // GraphicRaycaster gr;
    EventSystem es;
    Coroutine buildCoroutine;
    public bool useDescription = false;

    public GameObject AnimalShadowUI;
    public GameObject EmployeeStatusUI;
    [NonSerialized] public CanvasGroup canvasGroup;
    [SerializeField] Shadow shadow;
    [SerializeField] PenguinLevel levelUI;

    bool viewHireBtn;
    bool feeding = false;
    RaycastHit hit;

    float cameraSize;
    float CameraSize {  get { return cameraSize; } set {
            float size = value / 15;
            hireTransform.sizeDelta = new Vector2(200 / size, 200 / size);
            hireTransform.anchoredPosition = new Vector2(-100 / size, 100 / size);
            cameraSize = value; } }

    //  bool activeUpgrade;
    private void Awake()
    {
        hireTransform = hireBtn.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        EventManager.AddTouchEvent(2, (Action<Vector3>)(DropFishes));
        EventManager.AddTouchEvent(3, (Action)(StopFishes));
        EventManager.AddTouchEvent(4, (Action)(StopInfo));
    //    gr = GetComponent<GraphicRaycaster>();
        es = GetComponent<EventSystem>();
      
        GameInstance.GameIns.applianceUIManager = this;
    
        hireBtn.onClick.AddListener(() =>
        {
            GameInstance.GameIns.restaurantManager.HireEmployee();
        });
        rewardChestBtn.onClick.AddListener(() =>
        {
            GameInstance.GameIns.restaurantManager.GetReward();
        });

        appliancePanel.SetActive(false);
    }
  /*  private void OnEnable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.AddGraphicCaster(gr);
    }
    private void OnDisable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.RemoveGraphicCaster(gr);
    }*/

    private void OnDestroy()
    {
        EventManager.RemoveTouchEvent(2);
        EventManager.RemoveTouchEvent(3);
        EventManager.RemoveTouchEvent(4);
    }
  
    private void Update()
    {
        if (GameInstance.GameIns.inputManager.InputDisAble) return;
        if (viewHireBtn)
        {
            if (Physics.CheckSphere(GameInstance.GameIns.inputManager.cameraRange.position, Camera.main.orthographicSize / 4f, 1))
            {
                hireBtn.gameObject.SetActive(true);
            }
            else
            {
                hireBtn.gameObject.SetActive(false);
            }
        }
        if(hireTransform)
        {
            if(InputManger.cachingCamera.orthographic)
            {
                if(CameraSize != InputManger.cachingCamera.orthographicSize)
                {
                    CameraSize = InputManger.cachingCamera.orthographicSize;
                }
           
            }
        }
  
    }

    Coroutine infoCoroutine;
    public void ShowApplianceInfo(Furniture furniture)
    {
        if (scheduleCoroutine == null)
        {
           
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
            furnitureUI.UpdateInfo(furniture);
            appliancePanel.SetActive(true);
            //UnlockHire(false);

            othersGroup.alpha = 0;
            storeGroup.alpha = 0;
            othersGroup.interactable = false;
            othersGroup.blocksRaycasts = false;
            storeGroup.interactable = false;
            storeGroup.blocksRaycasts = false;
            //     otherUI.SetActive(false);
            //     shopUI.gameObject.SetActive(false);

        }
    }

    public void Replace(Furniture furniture)
    {
        if (scheduleCoroutine == null)
        {
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
            furnitureUI.SetFurniture(furniture);
            furnitureUI.Replace();
            //     appliancePanel.SetActive(true);
            //UnlockHire(false);
            othersGroup.alpha = 0;
            storeGroup.alpha = 0;
            othersGroup.interactable = false;
            othersGroup.blocksRaycasts = false;
            storeGroup.interactable = false;
            storeGroup.blocksRaycasts = false;
         //   otherUI.SetActive(false);
        //    shopUI.gameObject.SetActive(false);
        }
    }


    public void Reward(int num)
    {

        int r = Random.Range(5, 11);

        int r2 = Random.Range(num, num * 3 + 1);

        int sum = r + r2;

        StartCoroutine(Rewarding(sum));
    }

    IEnumerator Rewarding(int num)
    {
        int baseNum = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
        GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes += num;
        GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
        ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.TrashcanMinigame))?.Invoke(TutorialEventKey.TrashcanMinigame);
        yield return new WaitForSecondsRealtime(0.5f);

        rewardChestBtn.gameObject.SetActive(true);
        rewardChest_Fill.gameObject.SetActive(true);

        Stack<RectTransform> stack = new Stack<RectTransform>();

        Vector2 pos = new Vector2(540, 960);
        Vector2 target = new Vector2(540, 960);
        for (int i = 0; i < num; i++)
        {
            float x = Random.Range(-200, 200);
            float y = Random.Range(-200, 200);
            RectTransform icon = GameInstance.GameIns.restaurantManager.GetFishIcon().GetComponent<RectTransform>();
            icon.position =  new Vector2(pos.x + x , pos.y + y);
          //  icon.position = target;
            stack.Push(icon);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        while (stack.Count > 0)
        {
            RectTransform icon = stack.Pop();
            StartCoroutine(Rewarding(icon));
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if(RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.TrashcanMinigame))
        {
            RestaurantManager.tutorialKeys.Remove((int)TutorialEventKey.TrashcanMinigame);


            Tutorials tuto = GameInstance.GameIns.restaurantManager.tutorials;
            TutorialStruct tutorialStruct = GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id][0];
            Tutorials.TutorialUnlock(tutorialStruct);
            GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
            

        }
    }
   
    IEnumerator Rewarding(RectTransform icon)
    {
        Vector3 cur = icon.position;
        Vector3 target = GameInstance.GameIns.uiManager.fishImage.position;

        float f = 0;
        while (f <= 1)
        {
            icon.position = Vector3.Lerp(cur, target, f);
            f += Time.unscaledDeltaTime / 0.2f;
            yield return null;
        }
        icon.position = target;

        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);

        int before = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;

        int num = int.Parse(GameInstance.GameIns.uiManager.fishText.text);
        GameInstance.GameIns.uiManager.fishText.text = (num + 1).ToString();
        GameInstance.GameIns.restaurantManager.RemoveFishIcon(icon.gameObject);

    }


    void DisableInput(bool disable)
    {
        //  gameInstance.GameIns.inputManager
        GameInstance.GameIns.inputManager.InputDisAble = disable;
        feedingDescription.gameObject.SetActive(disable);
        // rewardsBox.ClearFishes();
        // rewardsBox.gameObject.SetActive(disable);
    }


    public void HideApplianceInfo()
    {
        if(appliancePanel.activeSelf) appliancePanel.SetActive(false);
    }

    Coroutine scheduleCoroutine;
  
    //이벤트 1
   
    //이벤트 2 
    void DropFishes(Vector3 pos)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 12))
        {
            if (Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("FeedBox")))
            {
                RewardsBox rb = hit.collider.gameObject.GetComponent<RewardsBox>();

                if (rb != currentBox)
                {
                    if (currentBox != null)
                    {
                        if (currentBox.foods.Count == 0)
                        {
                            currentBox.animal.reward = null;
                            StartCoroutine(currentBox.RemoveRewardBox());
                        }
                    }
                }

                feedingDescription.gameObject.SetActive(false);
                GameInstance.GameIns.inputManager.InputDisAble = true;
                feeding = true;
                this.hit = hit;
                currentBox = rb;
                float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                if (test < 0.05f)
                {
                   
                }
            }
        }
    }
    //이벤트 3
    void StopFishes()
    {
        if(currentBox != null && feeding)
        {
            feedingDescription.gameObject.SetActive(false);
            feeding = false;
        }
    }

    //이벤트 4
    public void StopInfo()
    {
        HideApplianceInfo();
        othersGroup.alpha = 1;
        storeGroup.alpha = 1;
        othersGroup.interactable = true;
        othersGroup.blocksRaycasts = true;
        storeGroup.interactable = true;
        storeGroup.blocksRaycasts = true;
      //  if(!otherUI.activeSelf) otherUI.SetActive(true);
      //  if(!shopUI.gameObject.activeSelf) shopUI.gameObject.SetActive(true);
        shopUI.scrolling.Shut();
    }
   
    public void ResetSchadule(RewardsBox rewardsBox)
    {
        
        if (rewardsBox.ClearFishes())
        {
            if(rewardsBox == currentBox)
            {
                UnlockHire(true);
                DisableInput(false);
            }
   
        }
    
        scheduleCoroutine = null;
    }
    public void UnlockHire(bool unlock)
    {
        //hireBtn.gameObject.SetActive(true);
        if (unlock)
        {
            RestaurantManager restaurantManager = GameInstance.GameIns.restaurantManager;
            int num = restaurantManager.employees.num;
            if (num < 8 && restaurantManager.employeeHire[num] <= restaurantManager.GetRestaurantValue() && !RestaurantManager.tutorialEventKeys.Contains(TutorialEventKey.NoEmployee))
            {
                //Debug.Log(num + " " + restaurantManager.employeeHire[num] + " " + restaurantManager.GetRestaurantValue());
                viewHireBtn = true;
                hireBtn.gameObject.SetActive(true);
            }
            else
            {
                viewHireBtn = false;
                hireBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            viewHireBtn = false;
            hireBtn.gameObject.SetActive(false);

        }
    }

    public void UIClearAll(bool visible)
    {
        if (visible)
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            InputManger.cachingCamera.cullingMask |= (1 << LayerMask.NameToLayer("ApplianceUI"));
        }
        else
        {
            GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            InputManger.cachingCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("ApplianceUI"));
        }
    }
}

