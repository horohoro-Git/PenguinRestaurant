
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public TMP_Text drawPriceText;
    public Button drawSpeedUpBtn;
    public Button fishingBtn;
    public Button fishingStartButton;
    public TMP_Text fishesNumText;
    public GameObject animalGuide;
    public Image fadeImage;


    public GraphicRaycaster graphicRaycaster;

    public GameObject checkMark;

    bool bGuideOn = false;

    private float cameraSize;

  //  public AudioSource audioSource;

    Dictionary<int, Sprite> atlasSprites = new Dictionary<int, Sprite>();
    EventSystem eventSystem;

    public RectTransform fishImage;
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
        eventSystem = EventSystem.current;
        atlasSprites[10001] = loadedAtlases["Town"].GetSprite(spriteAssetKeys[10001].Name);
        atlasSprites[10002] = loadedAtlases["Town"].GetSprite(spriteAssetKeys[10002].Name);
        atlasSprites[10003] = loadedAtlases["Town"].GetSprite(spriteAssetKeys[10003].Name);
        atlasSprites[10004] = loadedAtlases["Town"].GetSprite(spriteAssetKeys[10004].Name);
       

        animalGuideButton.onClick.AddListener(() =>
        {
            UIClick();
          
            if (bGuideOn)
            {

                animalGuideImage.sprite = atlasSprites[10001]; //loadedAtlases["Town"].GetSprite(spriteAssetKeys[10001].Name);
                bGuideOn = false;
            }
            else
            {
                switch (GameIns.app.currentScene)
                {
                    case SceneState.Restaurant:
                        animalGuideImage.sprite = atlasSprites[10002]; //loadedAtlases["Town"].GetSprite(spriteAssetKeys[10002].Name);
                        break;
                    case SceneState.Draw:
                        animalGuideImage.sprite = atlasSprites[10003];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10003].Name);
                        break;
                    case SceneState.Fishing:
                        animalGuideImage.sprite = atlasSprites[10004];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10004].Name);
                        break;
                }

                bGuideOn = true;
            }
            
            StartCoroutine(FadeInFadeOut(bGuideOn, 0));
        });

        changeScene.onClick.AddListener(() =>
        {
            UIClick();
            switch (GameIns.app.currentScene)
            {
                case SceneState.Restaurant:
                    
                    // GameIns.app.currentScene = SceneState.Draw;
                    if (GameIns.applianceUIManager.currentBox != null) GameIns.applianceUIManager.currentBox.ClearFishes();
                    animalGuideImage.sprite = atlasSprites[10001];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10001].Name); //loadedSprites[spriteAssetKeys[10001].ID];
                    changeSceneImage.sprite = atlasSprites[10002];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10002].Name);

                    cameraSize = InputManger.cachingCamera.orthographicSize;
                    drawBtn.gameObject.SetActive(false);
                    drawSpeedUpBtn.gameObject.SetActive(false);
                    StartCoroutine(FadeInFadeOut(true, 2));
                    break;
                case SceneState.Draw:
                  //  GameIns.app.currentScene = SceneState.Restaurant;
                   // animalGuideImage.sprite = loadedSprites[spriteAssetKeys[10001].ID];
                   // changeSceneImage.sprite = loadedSprites[spriteAssetKeys[10003].ID];
                    animalGuideImage.sprite = atlasSprites[10001]; //loadedAtlases["Town"].GetSprite(spriteAssetKeys[10001].Name); //loadedSprites[spriteAssetKeys[10001].ID];
                    changeSceneImage.sprite = atlasSprites[10003];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10003].Name);
                    //     GameIns.app.pos = GameIns.inputManager.cameraTrans.position;
                    // if (GameIns.applianceUIManager.currentBox != null) GameIns.applianceUIManager.currentBox.ClearFishes();

                    StartCoroutine(FadeInFadeOut(true, 1));
                    break;
                case SceneState.Fishing:
                    animalGuideImage.sprite = atlasSprites[10001]; //loadedAtlases["Town"].GetSprite(spriteAssetKeys[10001].Name); //loadedSprites[spriteAssetKeys[10001].ID];
                    changeSceneImage.sprite = atlasSprites[10003];//loadedAtlases["Town"].GetSprite(spriteAssetKeys[10004].Name);
                    StartCoroutine(FadeInFadeOut(true, 1));
                    break;
            }

        });

        fishingBtn.onClick.AddListener(() =>
        {
            UIClick();
            animalGuideImage.sprite = atlasSprites[10001];
            changeSceneImage.sprite = atlasSprites[10002];
            StartCoroutine(FadeInFadeOut(true, 3));
            fishingBtn.gameObject.SetActive(false);

        });

        menuOption.onClick.AddListener(() => {
            UIClick();
            option.SetActive(true);
            
        });

        drawBtn.onClick.AddListener(() =>
        {
            UIClick();
            GameIns.gatcharManager.StartGatcha();
        });

        drawSpeedUpBtn.onClick.AddListener(() =>
        {
            UIClick();
            GameIns.gatcharManager.GatcharSpeedUp();
        });

        fishingStartButton.onClick.AddListener(() =>
        {
            UIClick();
            GameIns.fishingManager.StartFishing();
            fishingStartButton.gameObject.SetActive(false);
        });
    }


    IEnumerator FadeInFadeOut(bool fades,int t)
    {
        eventSystem.enabled = false;
        float f = 0;
        fadeImage.raycastTarget = true;
        while (f <= 0.1f)
        {
            f += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0, 1, f / 0.1f);
            Color c = fadeImage.color;
            c.a = a;
          
            fadeImage.color = c;
            yield return null;
        }
        f = 0;
        ShowUI(t);
        /* if (t == 1)
         {
             Camera.main.orthographicSize = cameraSize;
         }
         else if (t == 2)
         {
             Camera.main.orthographicSize = 15;
         }
 */
        float timer = 0;
        while (timer < 0.1f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        while (f <= 0.1f)
        {
            f += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1, 0, f / 0.1f);
            Color c = fadeImage.color;
            c.a = a;

            fadeImage.color = c;
            yield return null;
        }
        Color transparent = fadeImage.color;
        transparent.a = 0;
        fadeImage.color = transparent;
        fadeImage.raycastTarget = false;
        yield return null;
        eventSystem.enabled = true;
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
            if(GameIns.restaurantManager.miniGame.activate && GameIns.restaurantManager.miniGame.type == MiniGameType.Fishing) fishingBtn.gameObject.SetActive(true);
            GameIns.app.ChangeScene_Restaurant();
        }
        else if (t == 2)
        {
            GameIns.applianceUIManager.UIClearAll(false);
            animalGuide.SetActive(false);
            bGuideOn = false;
            GameIns.app.ChangeScene_DrawScene();
        }
        else if (t == 3)
        {
            animalGuide.SetActive(false);
            bGuideOn = false;
            GameIns.applianceUIManager.UIClearAll(false);
            GameIns.app.ChangeScene_Fishing();

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
      
        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.UIClick(), 0.2f);

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

    public void UIClick()
    {
      /*  audioSource.clip = GameIns.uISoundManager.UIClick();
        audioSource.volume = 0.1f;
        audioSource.Play();*/
        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.UIClick(), 0.1f);
    }
}
