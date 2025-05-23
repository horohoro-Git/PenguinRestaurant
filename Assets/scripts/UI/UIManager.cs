
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AssetLoader;
using static GameInstance;

public class UIManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text fishText;
    public Order[] order;

    public Button animalGuideButton;
    public Image animalGuideImage;
    public Panel panel;
    public Button changeScene;
    public Button menuOption;
    public GameObject option;
    public RectTransform optionPopup;
    public Image changeSceneImage;
    public Button drawBtn;
    public Button drawSpeedUpBtn;
    public GameObject animalGuide;
    public Image fadeImage;


    public GraphicRaycaster graphicRaycaster;

    public GameObject checkMark;

    bool bGuideOn = false;

    private float cameraSize;

    // Start is called before the first frame update
    private void Awake()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        panel = GetComponentInChildren<Panel>();
        GameIns.uiManager = this;
    }
    private void OnEnable()
    {
        if (graphicRaycaster == null) graphicRaycaster = GetComponent<GraphicRaycaster>();
        AddGraphicCaster(graphicRaycaster);
    }
    private void OnDisable()
    {
        if (graphicRaycaster == null) graphicRaycaster = GetComponent<GraphicRaycaster>();
        RemoveGraphicCaster(graphicRaycaster);
        
    }
    void Start()
    {
        animalGuideButton.onClick.AddListener(() =>
        {
            if (bGuideOn)
            {
                animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10001].ID];
                bGuideOn = false;
            }
            else
            {
                switch (GameIns.app.currentScene)
                {
                    case SceneState.Restaurant:
                        animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10002].ID];
                        break;
                    case SceneState.Draw:
                        animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10003].ID];
                        break;
                }

                bGuideOn = true;
            }
            
            StartCoroutine(FadeInFadeOut(bGuideOn, 0));

         /*   //   GameInstance.GameIns.inputManager.inputDisAble = false;
            //   GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);

            //      animalGuide.SetActive(bGuideOn);
            StartCoroutine(FadeInFadeOut(bGuideOn, 0));
            }
            else
            {
                // if(state == SceneState.Draw)
                {
                //    GameInstance.GameIns.inputManager.inputDisAble = true;
         //           GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
                }
                bGuideOn = true;
                // animalGuide.SetActive(bGuideOn);
                StartCoroutine(FadeInFadeOut(bGuideOn, 0));
            }*/

        });

        changeScene.onClick.AddListener(() =>
        {
            switch (GameIns.app.currentScene)
            {
                case SceneState.Restaurant:
                    // GameIns.app.currentScene = SceneState.Draw;
                    if (GameIns.applianceUIManager.currentBox != null) GameIns.applianceUIManager.currentBox.ClearFishes();
                    animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10001].ID];
                    changeSceneImage.sprite = loadedSprites[spriteAssetKeys[10002].ID];

                    cameraSize = InputManger.cachingCamera.orthographicSize;
                    drawBtn.gameObject.SetActive(false);
                    drawSpeedUpBtn.gameObject.SetActive(false);
                    StartCoroutine(FadeInFadeOut(true, 2));
                    break;
                case SceneState.Draw:
                  //  GameIns.app.currentScene = SceneState.Restaurant;
                    animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10001].ID];
                    changeSceneImage.sprite = loadedSprites[spriteAssetKeys[10003].ID];
               //     GameIns.app.pos = GameIns.inputManager.cameraTrans.position;
                   // if (GameIns.applianceUIManager.currentBox != null) GameIns.applianceUIManager.currentBox.ClearFishes();

                    StartCoroutine(FadeInFadeOut(true, 1));
                    break;
            }

        });

        menuOption.onClick.AddListener(() => {

            option.SetActive(true);
            
        });

        drawBtn.onClick.AddListener(() =>
        {

            GameIns.gatcharManager.StartGatcha();
        });

        drawSpeedUpBtn.onClick.AddListener(() =>
        {
            GameIns.gatcharManager.GatcharSpeedUp();
        });
    }


    IEnumerator FadeInFadeOut(bool fades,int t)
    {
      //  if (fades)
        {
            float f = 0;
            fadeImage.raycastTarget = true;
            while (true)
            {   
                f += Time.unscaledDeltaTime * 8;
                Color c = fadeImage.color;
                c.a = f;
                if (fadeImage.color.a > 0.9)
                {
                    if (t == 1)
                    {
                        Camera.main.orthographicSize = cameraSize;
                    }
                    else if (t == 2)
                    {
                        Camera.main.orthographicSize = 15;
                    }
                    fadeImage.raycastTarget = false;
                    ShowUI(t);
                    c.a = 0;
                    fadeImage.color = c;
                    break;
                }
                fadeImage.color = c;
                yield return null;
            }
            
        }
    }

    void ShowUI(int t)
    {
       
        if (t == 0)
        {
        
            if (bGuideOn)
            {
                GameIns.applianceUIManager.UIClearAll(false);
                drawBtn.gameObject.SetActive(false);
                drawSpeedUpBtn.gameObject.SetActive(false);
            }
            else
            {
                if (GameIns.app.currentScene == SceneState.Draw)
                {
                    GameIns.applianceUIManager.UIClearAll(false);
                    drawBtn.gameObject.SetActive(true);
                    drawSpeedUpBtn.gameObject.SetActive(true);
                }        
                else
                {
                    GameIns.applianceUIManager.UIClearAll(true);
                }
            }
            animalGuide.SetActive(bGuideOn);
        }
        else if (t == 1)
        {
            
            // GameIns.applianceUIManager.UIClearAll(true);
            animalGuide.SetActive(false);
            bGuideOn = false;
            drawBtn.gameObject.SetActive(false);
            drawSpeedUpBtn.gameObject.SetActive(false);
            GameIns.app.ChangeScene_Restaurant();
        }
        else if (t == 2)
        {
            GameIns.applianceUIManager.UIClearAll(false);
            animalGuide.SetActive(false);
            bGuideOn = false;
            GameIns.app.ChangeScene_DrawScene();
        }
    }

    public void UpdateOrder(AnimalController customer, CounterType counterType)
    {
        Customer cs = customer.GetComponent<Customer>();
        List<Counter> c = GameInstance.GameIns.workSpaceManager.counters;
        List<PackingTable> p = GameInstance.GameIns.workSpaceManager.packingTables;
        if (counterType == CounterType.Delivery)
        {
            int count = 0;
            for (int i=0; i<p.Count; i++)
            {
                if (p[i].counterType == counterType)
                {
                    if (cs.foodStacks[0].needFoodNum - cs.foodStacks[0].foodStack.Count != 0)
                    {
                        PrintOrder(customer, cs.foodStacks[0].type, cs.foodStacks[0].needFoodNum - cs.foodStacks[0].foodStack.Count, count, counterType);
                        count++;
                    }
                    else
                    {
                        ClearOrder(counterType);
                    }
                }
            }
        }
        else
        {
            int count = 0;
            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].Customer == customer)
                {
                    for (int j = 0; j < cs.foodStacks.Count; j++)
                    {
                        if (cs.foodStacks[j].needFoodNum - cs.foodStacks[j].foodStack.Count != 0)
                        {
                            PrintOrder(customer, cs.foodStacks[j].type, cs.foodStacks[j].needFoodNum - cs.foodStacks[j].foodStack.Count, count, counterType);
                            count++;
                        }
                        else if(count == 0)
                        {
                            ClearOrder(counterType);
                        }

                    }
                }
            }
        }
    }

    void ClearOrder(CounterType counterType)
    {
        order[(int)counterType - 1].gameObject.SetActive(false);
        order[(int)counterType - 1].transform.SetParent(null);
    }

    void PrintOrder(AnimalController customer, MachineType type, int num, int count, CounterType counterType)
    {

        order[(int)counterType - 1].animalController = customer;
        // transform.position = order[(int)counterType - 1].animalController.transform.position + new Vector3(0, 10, 0);
        //  transform.rotation = Quaternion.Euler(new Vector3(60, 45, 0));
        order[(int)counterType - 1].transform.position = order[(int)counterType - 1].animalController.transform.position + GetVector3(0, 10, 0);
        order[(int)counterType - 1].transform.rotation = Quaternion.Euler(60, 45, 0);

        order[(int)counterType - 1].transform.SetParent(customer.transform);
        order[(int)counterType - 1].ShowOrder(type, num,count);
        order[(int)counterType - 1].gameObject.SetActive(true);
        if (num == 0) ClearOrder(counterType);
    }

    public float currentMoney = 900f; // 초기 돈 설정
   // public TextMeshProUGUI moneyText; // UI 텍스트를 연결할 변수


    //게임 종료 (사용중)
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    /* // 돈을 추가하는 메서드
     public void AddMoney(int amount)
     {
         currentMoney += amount;
         UpdateMoneyText();
     }
 */
    /* // 돈을 소모하는 메서드
     public void SpendMoney(int amount)
     {
         if (currentMoney >= amount)
         {
             currentMoney -= amount;
             UpdateMoneyText();
         }
         else
         {
         }
     }*/

    // UI 텍스트 업데이트 메서드
    public void UpdateMoneyText(float money)
    {
        currentMoney = money;
        if (currentMoney >= 1_000_000_000)
        {
            // 1_000_000_000 이상일 경우 B 단위로 표시
            float valueInK = currentMoney / 1_000_000_000f;
            moneyText.text = valueInK.ToString("F2") + "B";
        }
        else if (currentMoney >= 1_000_000)
        {
            // 1000000 이상일 경우 M 단위로 표시
            float valueInK = currentMoney / 1_000_000f;
            moneyText.text = valueInK.ToString("F2") + "M";
        }
        else if (currentMoney >= 1_000)
        {
            // 1000 이상일 경우 K 단위로 표시
            float valueInK = currentMoney / 1_000f;
            moneyText.text = valueInK.ToString("F2") + "K";
        }
        else
        {
            // 1000 미만일 경우 일반 표시
            moneyText.text = currentMoney.ToString();
        }
    }
}
