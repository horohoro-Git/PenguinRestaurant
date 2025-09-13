
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AssetLoader;
using static GameInstance;

public class UIManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public TMP_Text moneyText;
    public TMP_Text fishText;
    public Order[] order;
    public RectTransform safeArea;
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
    public TMP_Text autoDrawText;
    public Button drawSpeedUpBtn;
    public Button fishingBtn;
    public Button fishingStartButton;
    public Button worldBtn;
    public TMP_Text fishesNumText;
    public TMP_Text reputation;
    public GameObject animalGuide;
    public Image fadeImage;
    public RectTransform tutorialBorder;
    public Image tutorialImage;
    public RectTransform chatBorder;
    public Image tutorialChatImage;
    public TMP_Text tutorialText;
    public Button tutoBtn;
    public TMP_Text tutoBtn_Text;
    public TMP_Text tutoText2;
    public GameObject targetGO;
    public TMP_Text targetText;
    public WorldUI worldMap;
    public RectTransform cloudsUI;
    public GraphicRaycaster graphicRaycaster;
    public GraphicRaycaster graphicRaycaster2;

    public GameObject checkMark;

    bool bGuideOn = false;
    public bool gameGuide = false;
    private float cameraSize;
    [NonSerialized] public bool tutorialPlaying;

  //  public AudioSource audioSource;

    Dictionary<int, Sprite> atlasSprites = new Dictionary<int, Sprite>();
    public EventSystem eventSystem;

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
        //AddGraphicCaster(graphicRaycaster2);
//        if(RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.EnterGatcha)) changeScene.gameObject.SetActive(false);
//  
    }
    private void OnDisable()
    {
        if (graphicRaycaster == null) graphicRaycaster = GetComponent<GraphicRaycaster>();
        RemoveGraphicCaster(graphicRaycaster);
       // RemoveGraphicCaster(graphicRaycaster2);
        
    }
    void Start()
    {
   //     canvasScaler = GetComponent<CanvasScaler>();
    //    AdjustReferenceResolution();
        //   PlayerCamera.ApplySafeArea(safeArea);
       // eventSystem = EventSystem.current;
        if (loadedAtlases.ContainsKey("UI"))
        {
            atlasSprites[10001] = loadedAtlases["UI"].GetSprite(spriteAssetKeys[10001].Name);
            atlasSprites[10002] = loadedAtlases["UI"].GetSprite(spriteAssetKeys[10002].Name);
            atlasSprites[10003] = loadedAtlases["UI"].GetSprite(spriteAssetKeys[10003].Name);
            atlasSprites[10004] = loadedAtlases["UI"].GetSprite(spriteAssetKeys[10004].Name);
        }
       

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
                switch (App.currentScene)
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
            if (GameIns.gatcharManager.isSpawning) return;
            switch (App.currentScene)
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
            if(RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.StartFishing))
            {
                tutoText2.gameObject.SetActive(false);
                TutorialEnd(true);
            }
            GameIns.fishingManager.StartFishing();
            fishingStartButton.gameObject.SetActive(false);
        });

        worldBtn.onClick.AddListener(() =>
        {
            UIClick();
            GameIns.inputManager.InputDisAble = true;
            StartCoroutine(ChangeWorldMap());
            //worldMap.SetActive(true);
        });


        tutoBtn.onClick.AddListener(() =>
        {
            if (GameIns.restaurantManager != null)
            {
                if (GameIns.restaurantManager.tutorials.worked)
                {
                    TutorialStart(GameIns.restaurantManager.tutorials.id, GameIns.restaurantManager.tutorials.count, GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count);

                  /*  if (GameIns.restaurantManager.tutorials.count + 1 == GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count)
                    {
                        GameIns.restaurantManager.tutorials.worked = false;
                        Tutorials.Setup(GameIns.restaurantManager.tutorials);
                    }
                    else
                    {

                    }
                    Debug.Log(GameIns.restaurantManager.tutorials.count + " " + GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count);
                  
                    GameIns.restaurantManager.tutorials.count++;*/
                }
                else
                {
                    TutorialEnd(false);
                }
            }
        });
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
                if (App.currentScene == SceneState.Draw)
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
        order[(int)counterType - 1].transform.position = order[(int)counterType - 1].animalController.transform.position;
        order[(int)counterType - 1].transform.rotation = Quaternion.Euler(60, 45, 0);

        order[(int)counterType - 1].transform.SetParent(customer.transform);
        order[(int)counterType - 1].ShowOrder(type, num,count);
        order[(int)counterType - 1].gameObject.SetActive(true);
        if (num == 0) ClearOrder(counterType);
    }

    
    public void QuitGame()
    {
        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.UIClick(), 0.2f);
        App.GameExit().Forget();
    }



    public void UIClick()
    {
   
        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.UIClick(), 0.1f);
    }

    public void ChangeRestaurant()
    {
        StartCoroutine(ChangeRestaurantCoroutine());
    }
    IEnumerator ChangeRestaurantCoroutine()
    {
        Animator animator = cloudsUI.GetComponent<Animator>();
        animator.SetInteger("state", 1);
        RestaurantManager.restaurantTimer = 0;
        Time.timeScale = 0;
        SoundManager.Instance.PlayAudio(loadedSounds["CloudClose"], 1);
        yield return new WaitForSecondsRealtime(1);
        WorldUI worldUI = GameIns.worldUI.GetComponent<WorldUI>();
        worldUI.worldScene.SetActive(false);
        worldUI.bg.SetActive(false);
        SoundManager.Instance.PlayAudio(loadedSounds["CloudOpen"], 1);
        animator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.5f);
        GameIns.inputManager.InputDisAble = false;
        RestaurantManager.restaurantTimer = 1;
        Time.timeScale = 1;
        App.restaurantTimeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        GameIns.bgMSoundManager.BGMChange(901000, 1);
    }

    IEnumerator ChangeWorldMap()
    {
    
        Animator animator = cloudsUI.GetComponent<Animator>();
        animator.SetInteger("state", 1);
        RestaurantManager.restaurantTimer = 0;
        App.restaurantTimeScale = 0;
        Time.timeScale = 0;
        SoundManager.Instance.PlayAudio(loadedSounds["CloudClose"], 1);
        yield return new WaitForSecondsRealtime(1);
     //   worldMap.worldScene.gameObject.SetActive(true);
        WorldUI worldUI = worldMap.GetComponent<WorldUI>();
        worldUI.worldScene.SetActive(true);
        worldUI.bg.SetActive(true);
        worldUI.scrollSnap.Setup((int)App.currentStage);
        SoundManager.Instance.PlayAudio(loadedSounds["CloudOpen"], 1);
        animator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.5f);
        GameIns.bgMSoundManager.BGMChange(900001,1);
    }

    Coroutine tuto;
    public void TutorialStart(int id, int c, int max)
    { 
        if (GameIns.restaurantManager.tutorials.count + 1 == GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count)
        {
            GameIns.restaurantManager.tutorials.worked = false;
            if (!GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count].event_start) Tutorials.TutorialUnlock(GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count]);
            if(GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count].event_start) Tutorials.TutorialUnlockLateTime(GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count]);
         //   if (GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count].event_start) Tutorials.TutorialUnlock(GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id][GameIns.restaurantManager.tutorials.count]);
        }
        /*  else
          {

          }
          Debug.Log(GameIns.restaurantManager.tutorials.count + " " + GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count);
  */
        if (GameIns.restaurantManager.tutorials.count < GameIns.restaurantManager.tutorialStructs[GameIns.restaurantManager.tutorials.id].Count)
        {
            GameIns.restaurantManager.tutorials.count++;

            tutorialText.gameObject.SetActive(false);
            tutorialText.text = "";
            tutorialText.ForceMeshUpdate();
            if (tuto != null) StopCoroutine(tuto);
            if (c + 1 == max)
            {
                tutoBtn_Text.text = App.languages[91002].text;
                tutoBtn_Text.GetComponent<LanguageText>().id = 91002;
            }
            else
            {
                tutoBtn_Text.text = App.languages[91001].text;
                tutoBtn_Text.GetComponent<LanguageText>().id = 91001;
            }
            tuto = StartCoroutine(ShowTutorial(id, c, max));
        }
    }
    public void TutorialEnd(bool hideText)
    {
        if (tuto != null) StopCoroutine(tuto);
        HideTutorial(hideText);
       // tuto = StartCoroutine(HideTutorial(hideText));
    }
    IEnumerator ShowTutorial(int id, int c, int max)
    {
        GameIns.store.RemovePreview();
        GameIns.restaurantManager.doorPreview.CancelDoor();
        //   GameIns.restaurantManager.d
        GameIns.applianceUIManager.canvasGroup.alpha = 0;
        tutorialPlaying = true;
        tutorialImage.gameObject.SetActive(true);
        if (!tutorialText.gameObject.activeSelf) tutorialText.gameObject.SetActive(true);

        string res = App.languages[GameIns.restaurantManager.tutorialStructs[id][c].text_id].text;
        tutorialText.GetComponent<LanguageText>().id = GameIns.restaurantManager.tutorialStructs[id][c].text_id;
        tutorialText.text = res;
       // SoundManager
     //   yield return null;
        tutorialText.ForceMeshUpdate();
        TMP_TextInfo info = tutorialText.textInfo;
        int b = info.lineCount;
    
        tutorialText.maxVisibleCharacters = 0;
        int total = tutorialText.textInfo.characterCount;


        tutorialText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
       
        tutorialImage.sprite = loadedSprites[spriteAssetKeys[GameIns.restaurantManager.tutorialStructs[id][c].image_id].str];
        RectTransform tutorialImageRect = tutorialChatImage.rectTransform;
        Vector2 rectPos = new(0, -120);
        Vector2 rectPos2 = new(0, -300);
        Vector2 rectScale = new(600, 400);

        float rect1Range = ((b - 3) > 4 ? (90 + 30 * (b - 7)) : ((2.5f * MathF.Pow((b - 3), 2) + (b - 3) * 12.5f)));
 
        float rect2Range = ((b - 3) > 4 ? (260 + 50 * (b - 7)) : (-10 * (b-3) + 90));
        float rect1 = -120 - rect1Range; 
        float rect2 = -290 - (b - 3) * 55f;
        float rect3 = 400 + rect2Range;
        if(b > 3)
        {
            rectPos.y = rect1;
            rectPos2.y = rect2;

            rectScale.y = rect3;
        }
     //   tutorialImageRect.anchoredPosition = rectPos;
       // tutorialImageRect.sizeDelta = rectScale;
      //  tutoBtn.GetComponent<RectTransform>().anchoredPosition = rectPos2;

        Vector2 targetPos = new(-280, 1300);
        Vector2 startPos = new(280, 1300);
    /*    if (tutorialBorder.anchoredPosition != targetPos)
        {
            float f = 0;
            while (f <= 1)
            {
                Vector2 pos = Vector2.Lerp(startPos, targetPos, f);
                tutorialBorder.anchoredPosition = pos;
                f += Time.unscaledDeltaTime / 0.3f;
                yield return null;
            }
        }
        tutorialBorder.anchoredPosition = targetPos;*/


        tutorialChatImage.gameObject.SetActive(true);
        tutoBtn.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);

        if (c == 0)
        {
            TutorialStruct tutorialStruct = GameIns.restaurantManager.tutorialStructs[id][0];

            SoundManager.Instance.PlayAudio(AssetLoader.loadedSounds[sounds[tutorialStruct.event_sound].Name], 1);
        }

        for (int i = 0; i < total; i++)
        {
            tutorialText.maxVisibleCharacters = i + 1;
            yield return new WaitForSecondsRealtime(0.01f);
        }
     
    }
    void HideTutorial(bool hideText)
    {
        // tutoText2.gameObject.SetActive(true);
        //    tutoText2.text = App.languages[id].text;
        GameIns.applianceUIManager.canvasGroup.alpha = 1;
        tutorialPlaying = false;

        tutorialText.text = "";
        tutorialImage.gameObject.SetActive(false);
        tutoBtn.gameObject.SetActive(false);
        tutorialChatImage.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(false);
        Vector2 startPos = new(-280, 1300);
        Vector2 targetPos = new(280, 1300);

     /*   if (tutorialBorder.anchoredPosition != targetPos)
        {
            float f = 0;
            while (f <= 1)
            {
                Vector2 pos = Vector2.Lerp(startPos, targetPos, f);
                tutorialBorder.anchoredPosition = pos;
                f += Time.unscaledDeltaTime / 0.5f;
                yield return null;
            }
        }
        tutorialBorder.anchoredPosition = targetPos;*/


        tutoText2.gameObject.SetActive(!hideText);
    }
}
