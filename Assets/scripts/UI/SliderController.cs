using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.U2D;
//using UnityEditor.Animations; // ***** TextMeshPro를 사용하기 위해 추가 *****

public class SliderController : MonoBehaviour
{
    public Slider slider; 
    public float incrementAmount = 10f; 
    public float duration = 0.3f;
    public TMP_Text levelText;

    private float targetValue;
    private Coroutine currentCoroutine; 
    private int level = 1; 

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
    int currentEXP = 0;
    float progress;
    Vector2 size = new Vector2(0,150);
    bool isEXPCoroutineRunning;


    public int GetEXP { get { if (targetEmployee == null) return 0; else return targetEmployee.EXP; } }
    public int TargetEXP { get { if (targetEmployee == null) return 0; else return targetEmployee.employeeLevelData.targetEXP; } }
    
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
       /* if (model != null && InputManager.cachingCamera !=null)
        {
            if (model.position.y > 5)
            {
                if (BG.gameObject.activeSelf) BG.gameObject.SetActive(false);
                if (Border.gameObject.activeSelf) Border.gameObject.SetActive(false);
                if (Bar.gameObject.activeSelf) Bar.gameObject.SetActive(false);
                if (levelTextTrans.gameObject.activeSelf) levelTextTrans.gameObject.SetActive(false);
            }
            else
            {
                if(!BG.gameObject.activeSelf) BG.gameObject.SetActive(true);
                if (!Border.gameObject.activeSelf) Border.gameObject.SetActive(true);
                if (!Bar.gameObject.activeSelf) Bar.gameObject.SetActive(true);
                if (!levelTextTrans.gameObject.activeSelf) levelTextTrans.gameObject.SetActive(true);
                *//*     Vector3 rectPosition = new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
                     Vector3 rectPosition2 = new Vector3(model.position.x - 50f + expX, model.position.y + 120, model.position.z - 50f + expZ);
                     BG.position = rectPosition;
                     Border.position = rectPosition;
                     Bar.position = rectPosition2;
                     Vector3 rectPosition3 = new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
                     levelTextTrans.position = rectPosition3;*//*
             //   BG.anchoredPosition = new Vector3(0,0,0);
                Vector2 screenPos = InputManager.cachingCamera.WorldToScreenPoint(model.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(AnimalManager.employeeStatusParent.GetComponent<RectTransform>(), screenPos, AnimalManager.employeeStatusParent.GetComponent<Canvas>().worldCamera, out Vector2 uiPos);
                uiPos.y -= 80;
                Vector2 fixedPos = uiPos + new Vector2(0.25f, 0);
                Border.anchoredPosition = fixedPos;
                Bar.anchoredPosition = fixedPos;
                BG.anchoredPosition = uiPos;
                levelTextTrans.anchoredPosition = fixedPos;
            }
            
          
          //  rectTransform.position = rectPosition; // new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
        }*/
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

    private void LateUpdate()
    {
        if (model != null && InputManager.cachingCamera != null)
        {
            if (model.position.y > 5)
            {
                if (BG.gameObject.activeSelf) BG.gameObject.SetActive(false);
                if (Border.gameObject.activeSelf) Border.gameObject.SetActive(false);
                if (Bar.gameObject.activeSelf) Bar.gameObject.SetActive(false);
                if (levelTextTrans.gameObject.activeSelf) levelTextTrans.gameObject.SetActive(false);
            }
            else
            {
                if (!BG.gameObject.activeSelf) BG.gameObject.SetActive(true);
                if (!Border.gameObject.activeSelf) Border.gameObject.SetActive(true);
                if (!Bar.gameObject.activeSelf) Bar.gameObject.SetActive(true);
                if (!levelTextTrans.gameObject.activeSelf) levelTextTrans.gameObject.SetActive(true);
                /*     Vector3 rectPosition = new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
                     Vector3 rectPosition2 = new Vector3(model.position.x - 50f + expX, model.position.y + 120, model.position.z - 50f + expZ);
                     BG.position = rectPosition;
                     Border.position = rectPosition;
                     Bar.position = rectPosition2;
                     Vector3 rectPosition3 = new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
                     levelTextTrans.position = rectPosition3;*/
                //   BG.anchoredPosition = new Vector3(0,0,0);
                Vector2 screenPos = InputManager.cachingCamera.WorldToScreenPoint(model.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(AnimalManager.employeeStatusParent.GetComponent<RectTransform>(), screenPos, AnimalManager.employeeStatusParent.GetComponent<Canvas>().worldCamera, out Vector2 uiPos);
                uiPos.y -= 80;
                Vector2 fixedPos = uiPos + new Vector2(0.25f, 0);
                Border.anchoredPosition = fixedPos;
                Bar.anchoredPosition = fixedPos;
                BG.anchoredPosition = uiPos;
                levelTextTrans.anchoredPosition = fixedPos;
            }


            //  rectTransform.position = rectPosition; // new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
        }
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
        bool validCheck = false;
        while (true)
        {
            if(GetEXP < TargetEXP && currentEXP == GetEXP)
            {
                isEXPCoroutineRunning = false;
                yield break;
            }
            if (TargetEXP > currentEXP)
            {
                float least1 = (float)currentEXP / TargetEXP;
                float least2 = (float)GetEXP / TargetEXP;
                least2 = least2 > 1 ? 1 : least2;

                float f = 0;
                progress = least1;
                double s = 0;
                currentEXP = GetEXP;

                while (f < 0.2f)
                {
                    progress = Mathf.Lerp(least1, least2, f * 5f);
                    Bar.GetComponent<Image>().fillAmount = progress;
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
            if(GetEXP >= TargetEXP)
            {
                if(validCheck) yield break;
                validCheck = true;
                float f = 0;
              
                //레벨 업
                targetEmployee.LevelUp();

                
/*
                float least1 = 1;
                float least2 = 0;
               // progress = 0;
                double s = 0;
                while (f < 0.1f)
                {
                    progress = Mathf.Lerp(least1, least2, f * 10);
                    
                    Debug.Log(progress);
                    Bar.GetComponent<Image>().fillAmount = progress;
                    //   Bar.position = pos;
                    f += Time.unscaledDeltaTime;
                    // if (i == least2) break;
                    yield return null;

                }
                currentEXP = 0;
                size.x = (float)(7.25 * progress);
                expX = (float)(-0.01026 * (100 - progress));
                expZ = (float)(0.01026 * (100 - progress));*/

               // yield return new WaitForSecondsRealtime(0.1f);
                progress = 0;
                Bar.GetComponent<Image>().fillAmount = progress;

              //  yield return null;

            }
            yield return null;

            //isEXPCoroutineRunning = false;
            //Debug.LogWarning("Error " + GetEXP + " " + TargetEXP + " " + currentEXP);
          //  yield return null;
        }
    }

    public void Setup()
    {
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();

        //1회 초기화
        Border = Instantiate(Border);
  //      Border.SetSiblingIndex(0);
        Bar = Instantiate(Bar);
    //    Bar.SetSiblingIndex(1);
        BG = Instantiate(BG);
   //     BG.SetSiblingIndex(2);
        levelText = Instantiate(levelText);
  //      levelText.GetComponent<RectTransform>().SetSiblingIndex(3);
        sliderBG = BG.GetComponent<Image>();
        sliderBorder = Border.GetComponent<Image>();
        sliderBar = Bar.GetComponent<Image>();
        levelTextTrans = levelText.GetComponent<RectTransform>();
        //  BG.transform.position = new Vector3(100, 100, 100);
        //   Border.transform.position = new Vector3(100, 100, 100);
        //    Bar.transform.position = new Vector3(100, 100, 100);
        //   levelText.transform.position = new Vector3(100, 100, 100);
        // if(levelText.fontMaterial != null) levelText.fontMaterial = null;
        levelText.font = AssetLoader.font;
        levelText.fontSize = 30;
        levelText.fontSharedMaterial = AssetLoader.font_mat;
        Border.SetParent(AnimalManager.employeeStatusParent.transform);
        Bar.SetParent(AnimalManager.employeeStatusParent.transform);
        BG.SetParent(AnimalManager.employeeStatusParent.transform);
        levelTextTrans.SetParent(AnimalManager.employeeStatusParent.transform);
        sliderBG.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("gage_bg"); //AssetLoader.atlasSprites["slider_bar_color_0"];
        sliderBorder.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("whitebox"); // AssetLoader.atlasSprites["slider_bg_0"];
        sliderBar.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("whitebox");// AssetLoader.atlasSprites["slider_bar_color_0"];
        Bar.GetComponent<Image>().type = Image.Type.Filled;
        Bar.GetComponent<Image>().fillAmount = 0;
        levelText.GetComponent<RectTransform>().sizeDelta = new Vector2(137.5f, 45);
        BG.sizeDelta = new Vector2(150, 50);
        Bar.sizeDelta = new Vector2(137.5f, 45);
        Border.sizeDelta = new Vector2(137.5f, 45);
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
//level++; // ***** 레벨 증가 *****
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
