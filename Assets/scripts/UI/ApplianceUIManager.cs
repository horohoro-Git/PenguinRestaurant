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
    int level = 0;

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

    public Button upgradeButton;
    FoodMachine foodMachine;
    Employee animalController;
    public Sprite employeeIcon;
    public Sprite[] machines;
    //   private GameInstance gameInstance = new GameInstance();

    //  public Button feedExitButton;
    public TMP_Text feedingDescription;
    RewardsBox rewardsBox;

    public GameObject fishGO;
    Queue<GameObject> gameObjects = new Queue<GameObject>();
    List<GameObject> rewards = new List<GameObject>();

    public RewardsBox currentBox;
    public List<RewardsBox> rewardsBoxes = new List<RewardsBox>();
    GraphicRaycaster gr;
    EventSystem es;
    Coroutine buildCoroutine;
    public bool useDescription = false;
    bool bHidden;

    public GameObject AnimalShadowUI;
    public GameObject EmployeeStatusUI;
    [SerializeField] Shadow shadow;
    [SerializeField] PenguinLevel levelUI;

    bool viewHireBtn;
    bool feeding = false;
    RaycastHit hit;

    //  bool activeUpgrade;
    private void Awake()
    {
        EventManager.AddTouchEvent(2, (Action<Vector3>)(DropFishes));
        EventManager.AddTouchEvent(3, (Action)(StopFishes));
        EventManager.AddTouchEvent(4, (Action)(StopInfo));
        gr = GetComponent<GraphicRaycaster>();
        es = GetComponent<EventSystem>();
        //  activeUpgrade = true;
        // rewardsBox = GetComponentInChildren<RewardsBox>();

        GameInstance.GameIns.applianceUIManager = this;
      /*  upgradeButton.onClick.AddListener(() =>
        {
            Upgrade();
        });*/
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
    private void OnEnable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.AddGraphicCaster(gr);
    }
    private void OnDisable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.RemoveGraphicCaster(gr);
    }
    private void Start()
    {
      //  canvasScaler = GetComponent<CanvasScaler>();
      //  AdjustReferenceResolution();
      //  PlayerCamera.ApplySafeArea(rootUI.GetComponent<RectTransform>());
        //gameinstance.gameins.applianceuimanager = this;
        //appliancepanel.setactive(false); // 시작할 때는 패널 비활성화
    }
    void AdjustReferenceResolution()
    {
        // 현재 디바이스의 해상도 가져오기
        float deviceWidth = Screen.width;
        float deviceHeight = Screen.height;

        // 기준 해상도 설정 (예: 1080x1920을 기준으로 비율 계산)
        float referenceWidth = 1080f;
        float referenceHeight = 1920f;

        // 디바이스의 가로세로 비율 계산
        float deviceAspect = deviceWidth / deviceHeight;
        float referenceAspect = referenceWidth / referenceHeight;

        // 비율에 따라 Reference Resolution 조정
        if (deviceAspect > referenceAspect) // 더 넓은 화면 (예: 갤럭시 Z 폴드)
        {
            canvasScaler.referenceResolution = new Vector2(
                referenceWidth,
                referenceWidth / deviceAspect
            );
        }
        else // 더 높은 화면 (예: 일반 스마트폰)
        {
            canvasScaler.referenceResolution = new Vector2(
                referenceHeight * deviceAspect,
                referenceHeight
            );
        }

        Debug.Log($"Adjusted Reference Resolution: {canvasScaler.referenceResolution}");
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
        if(feeding)
        {
            if(currentBox && !currentBox.destroyed)
            {
              
                if (currentBox.foods.Count == 0 && currentBox.spawnTimer <= Time.time)
                {
                    currentBox.animal.reward = null;
                    StartCoroutine(currentBox.RemoveRewardBox());
                    currentBox = null;
                    feedingDescription.gameObject.SetActive(false);
                }
             
                if (currentBox.GetFish()) GameInstance.GameIns.restaurantManager.UseFish();
                //  feeding = true;
            }
        }
        else
        {
            if(currentBox)
            {
                if(currentBox.foods.Count == 0 && currentBox.spawnTimer <= Time.time && !currentBox.destroyed)
                {
                    currentBox.animal.reward = null;
                    StartCoroutine(currentBox.RemoveRewardBox());
                    currentBox = null;
                    feedingDescription.gameObject.SetActive(false);
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
            otherUI.SetActive(false);
            shopUI.gameObject.SetActive(false);
       
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
    //    Vector3 target = InputManger.cachingCamera.WorldToScreenPoint(pos);

       
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

        /*yield return new WaitForSecondsRealtime(0.2f);

        foreach (var v in stack)
        {
            StartCoroutine(SpreadFishes(v));
        }*/

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
            GameInstance.GameIns.uiManager.TutorialStart(tuto.id, tuto.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tuto.id].Count);
        }
       // if(RestaurantManager.tutorialKeys.Contains(15000)) Ac
        //  GameInstance.GameIns.uiManager.fishText.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes.ToString();
    }
    IEnumerator SpreadFishes(RectTransform rect)
    {
        float x = Random.Range(-200, 200);
        float y = Random.Range(-200, 200);
        Vector3 pos = new Vector3(540, 960);
        Vector3 target = new Vector3(540, 960);
        target.x += x;
        target.y += y;
        float f = 0;
        while (f <= 1)
        {
            rect.position = Vector3.Lerp(pos, target, f);
            f += Time.unscaledDeltaTime / 0.2f;
            yield return null;
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





    IEnumerator InputInfos_A(bool onlyAnimal = true)
    {
        GameInstance.GameIns.inputManager.InputDisAble = true;
        yield return null;
        otherUI.SetActive(true);
        appliancePanel.SetActive(false);
        infoCoroutine = null;

    }
    public void ShowPenguinUpgradeInfo(Employee animal, bool justUpdate = false)
    {
        if (scheduleCoroutine == null)
        {
            bool bShow = false;
            if (!justUpdate) bShow = true;
            else if (appliancePanel.activeSelf) bShow = true;

            if (bShow)
            {
                animalController = animal;
           //     EmployeeLevelStruct employeeData = animalController.employeeLevel;// gameInstance.GameIns.restaurantManager.currentEmployeeData;
                EmployeeLevelData employeeLevelData = animalController.employeeLevelData;
                foodMachine = null;
                otherUI.SetActive(false);
                //UnlockHire(false);
                appliancePanel.SetActive(true);
                if (infoCoroutine != null) StopCoroutine(infoCoroutine);
                applianceImage.sprite = employeeIcon;
                // upgradeCostText.text = $"업그레이드 비용 : {employeeData.}원";
                upgradeInfoText.text = $"레벨 : {employeeLevelData.level}\n이동 속도 : {employeeLevelData.speed}\n행동 속도 : {employeeLevelData.speed}/s\n최대 운반량 : {employeeLevelData.max_weight}개";
            }
        }
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
    void EmployeeSchedule(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            if (!Physics.CheckBox(hit.point, new Vector3(1f, 1f, 1f), Quaternion.identity, 1 << 6 | 1 << 7))
            {
                GameInstance.GameIns.inputManager.InputDisAble = false;
                RewardsBox rewardsBox = FoodManager.GetRewardsBox();
                rewardsBoxes.Add(rewardsBox);
                animalController.reward = rewardsBox;
                currentBox = rewardsBox;

                //  buildBoxCoroutine = true;
                Vector3 startPosition = new Vector3(hit.point.x, 5, hit.point.z);
                currentBox.gameObject.SetActive(true);
                currentBox.transform.localScale = new Vector3(2f, 2f, 2f);

                currentBox.transform.position = startPosition;
                if (useDescription) feedingDescription.text = "생선 상자를 클릭해서\n생선을 주세요";
                //    if (Application.platform == RuntimePlatform.Android) buildCoroutine = StartCoroutine(BuildPackageBox_A(hit.point, employee));
                buildCoroutine = StartCoroutine(BuildPackageBox(hit.point, animalController));

                currentBox.animal = animalController;
                currentBox.spawnTimer = Time.time + 5f;
            }
        }
    }
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
        if(!otherUI.activeSelf) otherUI.SetActive(true);
        if(!shopUI.gameObject.activeSelf) shopUI.gameObject.SetActive(true);
        shopUI.scrolling.Shut();
    }
   
    IEnumerator BuildPackageBox(Vector3 endPosition, Employee employee)
    {
      
        while (true)
        {
            currentBox.transform.Translate(Vector3.down * 30f * Time.deltaTime, Space.World);
            if(currentBox.transform.position.y < 0)
            {
                currentBox.transform.position = new Vector3(currentBox.transform.position.x, 0, currentBox.transform.position.z);
                break;
            }
            yield return null;
        }

        while (true)
        {
            currentBox.transform.localScale += new Vector3(1, 1, 1) * 10 * Time.deltaTime;

            if (currentBox.transform.localScale.x > 2.5f)
            {
                break;
            }

            yield return null;
        }

        while (true)
        {
            currentBox.transform.localScale -= new Vector3(1, 1, 1) * 5 * Time.deltaTime;

            if (currentBox.transform.localScale.x < 2f)
            {
                currentBox.transform.localScale = new Vector3(2, 2, 2);
                break;
            }
            yield return null;
        }
      
     /*  
        bool falling = false;
       // while (gameInstance.GameIns.inputManager.inputDisAble)
        while(true)
        {
            
            if (currentBox != null && !bHidden)
            {
                if ((Input.GetMouseButton(0)) && falling == false || Input.GetMouseButtonDown(0))
                {
                    GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    ggr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            chck = true;
                            break;
                        }
                    }
                    if (!chck)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 12))
                        {
                            if (Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("FeedBox")))
                            {
                                float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (GameInstance.GameIns.restaurantManager.playerData.fishesNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                                    {
                                        if (currentBox.GetFish(true)) GameInstance.GameIns.restaurantManager.UseFish();
                                        falling = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Input.GetMouseButton(0) && falling == true)
                {
                    GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped = new PointerEventData(es);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    ggr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("child");
                            chck = true;
                            break;
                        }
                    }

                    if (!chck)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                        {
                            if (GameInstance.GameIns.restaurantManager.playerData.fishesNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                            {
                                float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (currentBox.GetFish(false)) GameInstance.GameIns.restaurantManager.UseFish();
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                        PointerEventData ped = new PointerEventData(es);
                        ped.position = Input.mousePosition;
                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        ggr.Raycast(ped, raycastResults);
                        bool chck = false;
                        for (int i = 0; i < raycastResults.Count; i++)
                        {
                            if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                            {
                                Debug.Log("child");
                                chck = true;
                                break;
                            }
                        }

                        if (!chck)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                            {
                                currentBox.StopFish();
                            }
                            else
                            {
                                float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test < 0.05f)
                                {
                                    if (currentBox.ClearFishes())
                                    {
                                        employee.reward = null;
                                        UnlockHire(true);
                                        DisableInput(false);
                                        // animalController.reward = null;
                                        //     gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                    }
                                    else
                                    {
                                        UnlockHire(true);
                                        DisableInput(false);
                                        //   gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                                    }

                                    scheduleCoroutine = null;

                                    Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                                    {
                                        if (hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>())
                                        {
                                            ShowPenguinUpgradeInfo(hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>(), false);
                                        }
                                    }
                                    Ray ray3 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                    if (Physics.Raycast(ray3, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                                    {
                                        if (hit3.collider.GetComponentInParent<FoodMachine>())
                                        {
                                            FoodMachine foodMachine = hit3.collider.GetComponentInParent<FoodMachine>();

                                            infoCoroutine = null;

                                            ShowApplianceInfo(foodMachine);
                                            GameInstance.GameIns.inputManager.inputDisAble = false;
                                            //   yield break;
                                            yield break;

                                            // continue;
                                        }

                                    }
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentBox == null)
                {
                    feedingDescription.gameObject.SetActive(false);
                    yield break;
                }
                //  Debug.Log("Bug");
                //   DisableInput(false);
            }
            yield return null;
        }*/

      //  buildBoxCoroutine = false;
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
        bHidden = !visible;
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

