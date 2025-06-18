using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using static AssetLoader;
public class ApplianceUIManager : MonoBehaviour
{
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
    public GameObject rewardChest_Fill;
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
            GameInstance.GameIns.restaurantManager.GetFish();
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
        //gameinstance.gameins.applianceuimanager = this;
        //appliancepanel.setactive(false); // 시작할 때는 패널 비활성화
    }
    private void Update()
    {
        if (GameInstance.GameIns.inputManager.inputDisAble) return;
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
         /*   if (infoCoroutine != null) StopCoroutine(infoCoroutine);
            if (Application.platform == RuntimePlatform.Android) infoCoroutine = StartCoroutine(InputInfos_A(false));
            else infoCoroutine = StartCoroutine(InputInfos(false));*/
     
        }
    }

    IEnumerator InputInfos_A(bool onlyAnimal = true)
    {
        GameInstance.GameIns.inputManager.inputDisAble = true;
        yield return null;
        /*while (true)
        {
            if (!bHidden)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        GraphicRaycaster ggr2 = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                        PointerEventData ped2 = new PointerEventData(es);
                        ped2.position = touch.position;
                        List<RaycastResult> raycastResults2 = new List<RaycastResult>();
                        ggr2.Raycast(ped2, raycastResults2);
                        // bool chck2 = false;
                        for (int i = 0; i < raycastResults2.Count; i++)
                        {
                            if (raycastResults2[i].gameObject.GetComponentInParent<UIManager>())
                            {
                                //         chck2 = true;
                                break;
                            }
                        }



                        PointerEventData ped = new PointerEventData(es);
                        ped.position = touch.position;
                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        gr.Raycast(ped, raycastResults);
                        bool chck = false;
                        for (int i = 0; i < raycastResults.Count; i++)
                        {
                            if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                            {
                                chck = true;
                                break;
                            }
                        }

                        Ray rayGround = Camera.main.ScreenPointToRay(touch.position);
                        bool checkGround = Physics.Raycast(rayGround, out RaycastHit hit3, Mathf.Infinity, 1);
                        if (!chck)
                        {
                            GameInstance.GameIns.inputManager.inputDisAble = false;
                            //     GameInstance.GameIns.inputManager.DragScreen_Android(true);
                            infoCoroutine = null;

                        }
                        else
                        {
                            GameInstance.GameIns.inputManager.inputDisAble = true;
                            //        GameInstance.GameIns.inputManager.DragScreen_Android(true);
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        PointerEventData ped = new PointerEventData(es);
                        ped.position = touch.position;
                        List<RaycastResult> raycastResults = new List<RaycastResult>();
                        gr.Raycast(ped, raycastResults);
                        bool chck = false;
                        for (int i = 0; i < raycastResults.Count; i++)
                        {
                            if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                            {
                                chck = true;
                                break;
                            }
                        }

                        if (!chck)
                        {
                            GameInstance.GameIns.inputManager.inputDisAble = false;
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                            {
                                if (hit2.collider.GetComponentInParent<Animal>())
                                {
                                    Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();

                                    infoCoroutine = null;
                                    float test1 = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                    if (test1 < 0.05f)
                                    {
                                        ShowPenguinUpgradeInfo(newAnimal, true);

                                        yield break;
                                    }
                                    // continue;
                                }
                            }

                            Ray ray2 = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(ray2, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                            {
                                if (hit3.collider.GetComponentInParent<FoodMachine>())
                                {
                                    FoodMachine foodMachine = hit3.collider.GetComponentInParent<FoodMachine>();

                                    infoCoroutine = null;
                                    float test2 = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                    if (test2 < 0.05f)
                                    {
                                        ShowApplianceInfo(foodMachine);

                                        //   yield break;
                                        yield break;
                                    }
                                    // continue;
                                }

                            }
                            float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                            if (test < 0.05f)
                                break;
                        }
                    }
                }
            }
            yield return null;
        }*/

        //UnlockHire(true);
        otherUI.SetActive(true);
        appliancePanel.SetActive(false);
        infoCoroutine = null;

    }
    IEnumerator InputInfos(bool onlyAnimal = true)
    {
        GameInstance.GameIns.inputManager.inputDisAble = true;
        yield return null;
        while (true)
        {
            if (!bHidden)
            {
          //      if (Input.GetMouseButtonDown(0))
                {
                    GraphicRaycaster ggr2 = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                    PointerEventData ped2 = new PointerEventData(es);
                //    ped2.position = Input.mousePosition;
                    List<RaycastResult> raycastResults2 = new List<RaycastResult>();
                    ggr2.Raycast(ped2, raycastResults2);
                    //  bool chck2 = false;
                    for (int i = 0; i < raycastResults2.Count; i++)
                    {
                        if (raycastResults2[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("child");
                            //          chck2 = true;
                            break;
                        }
                    }



                    PointerEventData ped = new PointerEventData(es);
              //      ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    gr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            Debug.Log("Chid");
                            chck = true;
                            break;
                        }
                    }

              //      Ray rayGround = Camera.main.ScreenPointToRay(Input.mousePosition);
                //    bool checkGround = Physics.Raycast(rayGround, out RaycastHit hit3, Mathf.Infinity, 1);
                    if (!chck)
                    {
                        GameInstance.GameIns.inputManager.inputDisAble = false;
                        GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                        infoCoroutine = null;

                    }
                    else
                    {
                        GameInstance.GameIns.inputManager.inputDisAble = true;
                        GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                    }
                }
             //   if (Input.GetMouseButtonUp(0))
                {
                    PointerEventData ped = new PointerEventData(es);
                //    ped.position = Input.mousePosition;
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    gr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<ApplianceUIManager>() || raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            chck = true;
                            break;
                        }
                    }

                    if (!chck)
                    {
                        GameInstance.GameIns.inputManager.inputDisAble = false;
                     //   Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                     //   if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                        {
                      //      if (hit2.collider.GetComponentInParent<Animal>())
                            {
                        //        Employee newAnimal = hit2.collider.GetComponentInParent<Animal>().GetComponentInChildren<Employee>();

                                infoCoroutine = null;
                                float test1 = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                if (test1 < 0.05f)
                                {
                              //      ShowPenguinUpgradeInfo(newAnimal, true);

                                    //   yield break;
                                    yield break;
                                }
                                // continue;
                            }
                        }

                   //     Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                       // if (Physics.Raycast(ray2, out RaycastHit hit3, Mathf.Infinity, 1 << 13))
                        {
                      //      if (hit3.collider.GetComponentInParent<FoodMachine>())
                            {
                       //         FoodMachine foodMachine = hit3.collider.GetComponentInParent<FoodMachine>();

                                infoCoroutine = null;
                       //         float test2 = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                      //          if (test2 < 0.05f)
                                {
                               //     ShowApplianceInfo(foodMachine);

                                    //   yield break;
                                    yield break;
                                }
                                // continue;
                            }

                        }
                     //   float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                   //     if (test < 0.05f)
                  //          break;
                    }
                }
            }
            yield return null;
        }
        otherUI.SetActive(true);
        //UnlockHire(true);
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
        GameInstance.GameIns.inputManager.inputDisAble = disable;
        feedingDescription.gameObject.SetActive(disable);
        // rewardsBox.ClearFishes();
        // rewardsBox.gameObject.SetActive(disable);
    }


    public void HideApplianceInfo()
    {
        if(appliancePanel.activeSelf) appliancePanel.SetActive(false);
       /* // UnlockHire(true);
        if (infoCoroutine != null)
        {
            StopCoroutine(infoCoroutine);
            infoCoroutine = null;
        }*/
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
        //  text.text = $"Level {level}";
        Invoke("Level", 1f);
    }

    void Level()
    {
        text.text = $"레벨 {level}";
    }

    public void ActtiveSubUI(bool isactive)
    {
        hireBtn.gameObject.SetActive(isactive);
        rewardChestBtn.gameObject.SetActive(isactive);
    }

    void Upgrade()
    {
        if (foodMachine) GameInstance.GameIns.restaurantManager.UpgradeFoodMachine(foodMachine);
        else
        {
            if (GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes > 0 && animalController.reward == null)
                Feed();
        }
    }

    Coroutine scheduleCoroutine;
    void Feed()
    {
        //    if (scheduleCoroutine == null && buildBoxCoroutine == false)
        {
            if (animalController.rewardingType == RewardingType.None && rewardsBoxes.Count < 3)
            {
                // if (infoCoroutine != null) StopCoroutine(infoCoroutine);
                //  gameInstance.GameIns.inputManager.inputDisAble = true;
                if (currentBox != null)
                {
                    if (currentBox.foods.Count == 0 && currentBox.spawnTimer <= Time.time && !currentBox.destroyed)
                    {
                        currentBox.animal.reward = null;
                        currentBox.RemoveRewardBox();
                        currentBox = null;
                    }
                }


                HideApplianceInfo();
                otherUI.SetActive(true);
                feedingDescription.gameObject.SetActive(true);
                if (useDescription) feedingDescription.text = "생선 상자를 생성하기 위해\n바닥을 클릭해주세요";
                EventManager.AddTouchEvent(1, (Action<Vector3>)(EmployeeSchedule));
              //  if (Application.platform == RuntimePlatform.Android) scheduleCoroutine = StartCoroutine(EmployeeScheduleWork_A());
               // else scheduleCoroutine = StartCoroutine(EmployeeScheduleWork());
            }
            else
            {
                Debug.Log("boxCount " + rewardsBoxes.Count + " " + animalController.rewardingType);
            }
        }
    }

    //이벤트 1
    void EmployeeSchedule(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            if (!Physics.CheckBox(hit.point, new Vector3(1f, 1f, 1f), Quaternion.identity, 1 << 6 | 1 << 7))
            {
                GameInstance.GameIns.inputManager.inputDisAble = false;
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
                GameInstance.GameIns.inputManager.inputDisAble = true;
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
         //   currentBox = null;
       //     GameInstance.GameIns.inputManager.inputDisAble = false;
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
    IEnumerator EmployeeScheduleWork_A()
    {
        if (useDescription)
        {

            feedingDescription.gameObject.SetActive(true);
           // feedingDescription.text = "생선 상자를 생성하기 위해\n바닥을 클릭해주세요";
        }
        Employee employee = animalController;
        yield return null;
        //while (gameInstance.GameIns.inputManager.inputDisAble)
      /*  while (true)
        {
            float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
            GameInstance.GameIns.inputManager.inputDisAble = false;

            if (Input.touchCount > 0)
            {
             *//*   Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Ended)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                    {
                        if (!Physics.CheckBox(hit.point, new Vector3(1f, 1f, 1f), Quaternion.identity, 1 << 6 | 1 << 7))
                        {
                            //   float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                            if (test < 0.05f)
                            {
                                if(Application.platform == RuntimePlatform.Android) buildCoroutine = StartCoroutine(BuildPackageBox_A(hit.point, employee));
                                else buildCoroutine = StartCoroutine(BuildPackageBox(hit.point, employee));

                                yield break;
                            }
                        }
                    }
                }*//*
           
            }
            yield return null;
        }*/
        // scheduleCoroutine = null;
    }


    IEnumerator EmployeeScheduleWork()
    {
        if (useDescription)
        {

            feedingDescription.gameObject.SetActive(true);
            feedingDescription.text = "생선 상자를 생성하기 위해\n바닥을 클릭해주세요";
        }
        Employee employee = animalController;
        yield return null;
        //while (gameInstance.GameIns.inputManager.inputDisAble)
        while (true) 
        {
            float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
            GameInstance.GameIns.inputManager.inputDisAble = false;
        //    if (Input.GetMouseButtonUp(0))
            {
             //   Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             //   if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {
           //         if (!Physics.CheckBox(hit.point, new Vector3(1f, 1f, 1f), Quaternion.identity, 1 << 6 | 1 << 7))
                    {
                        //   float test = (gameInstance.GameIns.inputManager.preLoc - gameInstance.GameIns.inputManager.curLoc).magnitude;
                        if (test < 0.05f)
                        {
                            buildCoroutine = StartCoroutine(BuildPackageBox(hit.point, employee));

                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }
       // scheduleCoroutine = null;
    }

    IEnumerator BuildPackageBox_A(Vector3 endPosition, Employee employee)
    {
        GameInstance.GameIns.inputManager.inputDisAble = false;
        RewardsBox rewardsBox = FoodManager.GetRewardsBox();
        rewardsBoxes.Add(rewardsBox);
        employee.reward = rewardsBox;
        currentBox = rewardsBox;

        //buildBoxCoroutine = true;
        Vector3 startPosition = new Vector3(endPosition.x, 5, endPosition.z);
        currentBox.gameObject.SetActive(true);
        currentBox.transform.localScale = new Vector3(2f, 2f, 2f);

        currentBox.transform.position = startPosition;

        if (useDescription) feedingDescription.text = "생선 상자를 클릭해서\n생선을 주세요";
        while (true)
        {
            currentBox.transform.Translate(Vector3.down * 30f * Time.deltaTime, Space.World);
            if (currentBox.transform.position.y < 0)
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

        // while (gameInstance.GameIns.inputManager.inputDisAble)
   /*     while (true)
        {

            if (currentBox != null && !bHidden)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && falling == false || touch.phase == TouchPhase.Began)
                    {
                        GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                        PointerEventData ped = new PointerEventData(es);
                        ped.position = touch.position;
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
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 12))
                            {
                                if (Physics.CheckBox(hit.point, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, LayerMask.GetMask("FeedBox")))
                                {
                                    float test = (GameInstance.GameIns.inputManager.preLoc - GameInstance.GameIns.inputManager.curLoc).magnitude;
                                    if (test < 0.05f)
                                    {
                                        if (GameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
                                        {
                                            if (currentBox.GetFish(true)) GameInstance.GameIns.restaurantManager.UseFish();
                                            falling = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && falling == true)
                    {
                        GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                        PointerEventData ped = new PointerEventData(es);
                        ped.position = touch.position;
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
                            Ray ray = Camera.main.ScreenPointToRay(touch.position);
                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("FeedBox")))
                            {
                                if (GameInstance.GameIns.restaurantManager.fishNum > 0 && hit.collider.GetComponent<RewardsBox>() == currentBox)
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
                        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                        {
                            GraphicRaycaster ggr = GameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();

                            PointerEventData ped = new PointerEventData(es);
                            ped.position = touch.position;
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
                                Ray ray = Camera.main.ScreenPointToRay(touch.position);
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

                                        Ray ray2 = Camera.main.ScreenPointToRay(touch.position);
                                        if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Animal")))
                                        {
                                            if (hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>())
                                            {
                                                ShowPenguinUpgradeInfo(hit2.collider.GetComponentInParent<Animal>().GetComponent<Employee>(), false);
                                            }
                                        }
                                        Ray ray3 = Camera.main.ScreenPointToRay(touch.position);
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

            }
            yield return null;
        }*/

       // buildBoxCoroutine = false;
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
            if (num < 8 && restaurantManager.employeeHire[num] <= restaurantManager.GetRestaurantValue())
            {
                //Debug.Log(num + " " + restaurantManager.employeeHire[num] + " " + restaurantManager.GetRestaurantValue());
                viewHireBtn = true;
                hireBtn.gameObject.SetActive(true);
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

