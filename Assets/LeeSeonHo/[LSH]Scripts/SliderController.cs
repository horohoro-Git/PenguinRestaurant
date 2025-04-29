using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.Animations; // ***** TextMeshPro를 사용하기 위해 추가 *****

public class SliderController : MonoBehaviour
{
    public Slider slider; // ***** Slider UI 연결 변수 *****
    public float incrementAmount = 10f; // ***** 증가량 *****
    public float duration = 0.3f; // ***** 부드럽게 증가하는 시간 (초) *****
    public TextMeshProUGUI levelText; // ***** TextMeshPro 연결 변수 ***** 

    private float targetValue; // ***** 최종 목표값 *****
    private Coroutine currentCoroutine; // ***** 현재 실행 중인 Coroutine *****
    private int level = 1; // ***** 현재 레벨 *****

    public Transform model;
    public RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        // targetValue = slider.value;
        //  level = 1; // ***** 초기 레벨 설정 *****
        UpdateLevelText(); // ***** 텍스트 초기화 *****
        currentCoroutine = StartCoroutine(SmoothIncrease());

        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (model != null)
        {
            Vector3 rectPosition = GameInstance.GetVector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
            rectTransform.position = rectPosition; // new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
        }

    }

    

    public void GainExperience()
    {
        // 이미 실행 중인 Coroutine이 있다면 멈춤
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 새로운 Coroutine 실행
        currentCoroutine = StartCoroutine(SmoothIncrease());
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
