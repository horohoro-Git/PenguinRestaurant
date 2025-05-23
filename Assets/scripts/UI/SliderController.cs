using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
//using UnityEditor.Animations; // ***** TextMeshPro를 사용하기 위해 추가 *****

public class SliderController : MonoBehaviour
{
    public Slider slider; // ***** Slider UI 연결 변수 *****
    public float incrementAmount = 10f; // ***** 증가량 *****
    public float duration = 0.3f; // ***** 부드럽게 증가하는 시간 (초) *****
    public TMP_Text levelText; // ***** TextMeshPro 연결 변수 ***** 

    private float targetValue; // ***** 최종 목표값 *****
    private Coroutine currentCoroutine; // ***** 현재 실행 중인 Coroutine *****
    private int level = 1; // ***** 현재 레벨 *****

    public Transform model;
    public Employee targetEmployee;
    public RectTransform rectTransform;

    public RectTransform levelTextTrans;
    public RectTransform BG;
    public RectTransform Border;
    public RectTransform Bar;
    public Image sliderBG;
    public Image sliderBorder;
    public Image sliderBar;
    int targetEXP = 100;
    int currentEXP = 0;
    int testexp = 0;
    int exp;
    int progress;
    Vector2 size = new Vector2(0,150);
    bool isEXPCoroutineRunning;


    public int GetEXP { get { if (targetEmployee == null) return 0; else return targetEmployee.EXP; } set { if (targetEmployee == null) return; else targetEmployee.EXP = value; } }

    // Start is called before the first frame update
    void Start()
    {
      /*  if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        BG = Instantiate(BG, GetComponent<RectTransform>().parent.GetComponent<RectTransform>());
        Border = Instantiate(Border, GetComponent<RectTransform>().parent.GetComponent<RectTransform>());
        Bar = Instantiate(Bar, GetComponent<RectTransform>().parent.GetComponent<RectTransform>());
        levelText = Instantiate(levelText, GetComponent<RectTransform>().parent.GetComponent<RectTransform>());
        sliderBG = BG.GetComponent<Image>();
        sliderBorder = Border.GetComponent<Image>();
        sliderBar = Bar.GetComponent<Image>();
        levelTextTrans = levelText.GetComponent<RectTransform>();*/

        // if(levelText.fontMaterial != null) levelText.fontMaterial = null;
       // levelText.font = AssetLoader.font;
     //   levelText.fontSharedMaterial = AssetLoader.font_mat;


        // targetValue = slider.value;
        //  level = 1; // ***** 초기 레벨 설정 *****
        UpdateLevelText(); // ***** 텍스트 초기화 *****
                           // currentCoroutine = StartCoroutine(SmoothIncrease());

        //   StartCoroutine(t());
        // Bar.position = 
        // Bar.localScale = new Vector3(0.002f, 0.004f, 0.004f);

        Vector2 size = new Vector2(0,150);
        size.x = 0;
        Bar.sizeDelta = size;
        double x = -0.01026 * (100 - 0);
        double z = 0.01026 * (100 - 0);
        Vector3 pos = new Vector3((float)x, 0, (float)z);
        Bar.position = pos;
        isEXPCoroutineRunning = true;
        StartCoroutine(IncreaseEXP());
    }

    IEnumerator t()
    {
        int i = 0;
        double s;
      
        while(true)
        {
            i++;
            Vector2 size = rectTransform.sizeDelta;
            s = 7.25 * i;
            size.x = (float)s;
            Bar.sizeDelta = size;
            double x = -0.01026 * (100 - i);
            double z = 0.01026 * (100 - i);
            Vector3 pos = new Vector3((float)x, 0, (float)z);
            Bar.position = pos;
            if (i == 100) break;
            yield return null;
        }
    }

    float expX;
    float expZ;
    // Update is called once per frame
    void Update()
    {
        if (model != null)
        {
            Vector3 rectPosition = GameInstance.GetVector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
            Vector3 rectPosition2 = GameInstance.GetVector3(model.position.x - 50f + expX, model.position.y + 120, model.position.z - 50f + expZ);
            BG.position = rectPosition;
            Border.position = rectPosition;
            Bar.position = rectPosition2;
            levelTextTrans.position = rectPosition;
            Bar.sizeDelta = size;
          
          //  rectTransform.position = rectPosition; // new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
        }
       /* if (Input.GetMouseButtonDown(0))
        {
            exp += 50;
            if(!isEXPCoroutineRunning)
            {
                isEXPCoroutineRunning = true;
                StartCoroutine(IncreaseEXP());
            }
         //   if (expCoroutine != null) StopCoroutine(expCoroutine);
         //   expCoroutine = StartCoroutine(IncreaseEXP());
        }*/
    }
    public void EXPChanged()
    {
        if (!isEXPCoroutineRunning)
        {
            isEXPCoroutineRunning = true;
            StartCoroutine(IncreaseEXP());
        }
    }
    IEnumerator IncreaseEXP()
    {
        while (true)
        {
            if(GetEXP != targetEXP && currentEXP == GetEXP)
            {
                isEXPCoroutineRunning = false;
                yield break;
            }
            if (targetEXP > currentEXP)
            {
                int least1 = (int)(((float)currentEXP / targetEXP) * 100);
                int least2 = (int)(((float)GetEXP / targetEXP) * 100);
                least2 = least2 > 100 ? 100 : least2;

                float f = 0;
                progress = least1;
                double s = 0;
                currentEXP = least2;

                while (f < 0.4f)
                {
                    progress = (int)Mathf.Lerp(least1, least2, f * 2.5f);
                    size = new Vector2(0, 150);
                    s = 7.25 * progress;
                    size.x = (float)s;
                  //  Bar.sizeDelta = size;
                    double x = -0.01026 * (100 - progress);
                    double z = 0.01026 * (100 - progress);
                    expX = (float)x;
                    expZ = (float)z;
                    //   Bar.position = pos;
                    f += Time.unscaledDeltaTime;
                    // if (i == least2) break;
                    yield return null;
                }
                progress = least2;
                size.x = (float)s;
                expX = (float)(-0.01026 * (100 - progress));
                expZ = (float)(0.01026 * (100 - progress));

                yield return null;
                continue;
            }
            if(GetEXP >= targetEXP)
            {
                
                float f = 0;
                GetEXP -= targetEXP;
                int least1 = progress;
                int least2 = 0;
               // progress = 0;
                double s = 0;
                while (f < 0.2)
                {
                    progress = (int)Mathf.Lerp(least1, least2, f * 5);
                    size = new Vector2(0, 150);
                    s = 7.25 * progress;
                    size.x = (float)s;
                   // Bar.sizeDelta = size;
                    double x = -0.01026 * (100 - progress);
                    double z = 0.01026 * (100 - progress);
                    expX = (float)x;
                    expZ = (float)z;
                    //   Bar.position = pos;
                    f += Time.unscaledDeltaTime;
                    // if (i == least2) break;
                    yield return null;

                }
                currentEXP = 0;
                progress = least2;
                size.x = (float)(7.25 * progress);
                expX = (float)(-0.01026 * (100 - progress));
                expZ = (float)(0.01026 * (100 - progress));
                yield return null;
            }
           
        }
    }

    public void Setup()
    {
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();

        //1회 초기화
        BG = Instantiate(BG);
        Border = Instantiate(Border);
        Bar = Instantiate(Bar);
        levelText = Instantiate(levelText);
        sliderBG = BG.GetComponent<Image>();
        sliderBorder = Border.GetComponent<Image>();
        sliderBar = Bar.GetComponent<Image>();
        levelTextTrans = levelText.GetComponent<RectTransform>();

       // if(levelText.fontMaterial != null) levelText.fontMaterial = null;
        levelText.font = AssetLoader.font;
        levelText.fontSharedMaterial = AssetLoader.font_mat;

        BG.SetParent(AnimalManager.SubUIParent.transform);
        Border.SetParent(AnimalManager.SubUIParent.transform);
        Bar.SetParent(AnimalManager.SubUIParent.transform);
        levelTextTrans.SetParent(AnimalManager.SubUIParent.transform);

        sliderBG.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("slider_bar_color_0"); //AssetLoader.atlasSprites["slider_bar_color_0"];
        sliderBorder.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("slider_bg_0"); // AssetLoader.atlasSprites["slider_bg_0"];
        sliderBar.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("slider_bar_color_0");// AssetLoader.atlasSprites["slider_bar_color_0"];
       // Canvas.ForceUpdateCanvases();
    }

    public void Activate(bool isActivate)
    {
        BG.gameObject.SetActive(isActivate);
        Border.gameObject.SetActive(isActivate);
        Bar.gameObject.SetActive(isActivate);
        levelTextTrans.gameObject.SetActive(isActivate);
    }
    public void GainExperience()
    {
        // 이미 실행 중인 Coroutine이 있다면 멈춤
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 새로운 Coroutine 실행
     //   currentCoroutine = StartCoroutine(SmoothIncrease());
    }

   
    private IEnumerator SmoothIncrease()
    {
        float startValue = slider.value;
        float elapsedTime = 0f;

        // ***** 애니메이션 실행 *****
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, Mathf.Min(targetValue, slider.maxValue), elapsedTime / duration);
            yield return null;
        }

        // ***** 최종값 보정 *****
        slider.value = Mathf.Min(targetValue, slider.maxValue);
        // Coroutine이 끝나면 null로 설정
        currentCoroutine = null;

        // ***** 슬라이더가 다 찼는지 확인 *****
        if (slider.value >= slider.maxValue)
        {
            float overflowValue = targetValue - slider.maxValue; // ***** targetValue에서 초과 값 계산 *****
          //  LevelUp(overflowValue);
        }
    }

    private void LevelUp(float overflowValue)
    {
        level++; // ***** 레벨 증가 *****
        slider.value = 0; // ***** 슬라이더 초기화 *****
        targetValue = overflowValue; // ***** 초과된 값을 새로운 목표값으로 설정 *****
        UpdateLevelText(); // ***** 텍스트 업데이트 *****

        // ***** 초과 값이 있을 경우 애니메이션을 이어서 진행 *****
        if (overflowValue > 0)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(SmoothIncrease());
        }
    }


    public void ClearEXP()
    {
        StopAllCoroutines();
        StartCoroutine(Clear());
    }

    IEnumerator Clear()
    {
        targetValue = 0;
      //  yield return new WaitForSeconds(0.f);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(Mathf.Min(targetValue, slider.maxValue), 0, elapsedTime / duration);
            yield return null;
        }
        slider.value = 0f;
    }

    public void UpdateEXP(int value)
    {
        targetValue += (float)value;
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
        if(levelText != null) levelText.text = $"Level {level}";
    
    }

    void UpdateLevelText()
    {
        levelText.text = $"Level {level}"; // ***** 텍스트를 "Level X"로 설정 *****
    }
}
