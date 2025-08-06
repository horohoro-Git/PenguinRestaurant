using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GraphicSetting : Settings
{
   
    public Animator lowAnimator;
    public Animator midAnimator;
    public Animator highAnimator;
    public Button lowBtn;
    public Button midBtn;
    public Button highBtn;
    public RectTransform graphicBorder;
    TMP_Text low_Text;
    TMP_Text mid_Text;
    TMP_Text high_Text;
    Coroutine coroutine;
    bool isOpen;

    private void Awake()
    {
        low_Text = lowBtn.GetComponentInChildren<TMP_Text>();
        mid_Text = midBtn.GetComponentInChildren<TMP_Text>();
        high_Text = highBtn.GetComponentInChildren<TMP_Text>();
    }

    public void Click()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(OpenAnimation());

            lowBtn.onClick.AddListener(() =>
            {
                QualitySettings.SetQualityLevel(1);
                int currentLevel = QualitySettings.GetQualityLevel();
                string currentLevelName = QualitySettings.names[currentLevel];
                if (GameInstance.GameIns.playerCamera.cam.TryGetComponent(out UniversalAdditionalCameraData cameraData))
                {
                    cameraData.renderPostProcessing = false;
                }
                Debug.Log("현재 퀄리티 레벨 인덱스: " + currentLevel);
                Debug.Log("현재 퀄리티 이름: " + currentLevelName);
            });

            midBtn.onClick.AddListener(() =>
            {
                QualitySettings.SetQualityLevel(2);
                int currentLevel = QualitySettings.GetQualityLevel();
                string currentLevelName = QualitySettings.names[currentLevel];
                if (GameInstance.GameIns.playerCamera.cam.TryGetComponent(out UniversalAdditionalCameraData cameraData))
                {
                    cameraData.renderPostProcessing = false;
                }
                Debug.Log("현재 퀄리티 레벨 인덱스: " + currentLevel);
                Debug.Log("현재 퀄리티 이름: " + currentLevelName);
            });
            highBtn.onClick.AddListener(() =>
            {
                QualitySettings.SetQualityLevel(3);
                int currentLevel = QualitySettings.GetQualityLevel();
                string currentLevelName = QualitySettings.names[currentLevel];
                if (GameInstance.GameIns.playerCamera.cam.TryGetComponent(out UniversalAdditionalCameraData cameraData))
                {
                    cameraData.renderPostProcessing = true;
                }
                Debug.Log("현재 퀄리티 레벨 인덱스: " + currentLevel);
                Debug.Log("현재 퀄리티 이름: " + currentLevelName);
            });
        }
        else
        {
            isOpen = false;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(CloseAnimation());
            lowBtn.onClick.RemoveAllListeners();
            midBtn.onClick.RemoveAllListeners();
            highBtn.onClick.RemoveAllListeners();
        }
        
    }
    IEnumerator OpenAnimation()
    {
        animator.SetInteger("state", 1);
        yield return new WaitForSecondsRealtime(0.2f);

        lowAnimator.SetInteger("state", 1);
        yield return new WaitForSecondsRealtime(0.1f);
        midAnimator.SetInteger("state", 1);
        yield return new WaitForSecondsRealtime(0.1f);
        highAnimator.SetInteger("state", 1);
    }

    IEnumerator CloseAnimation()
    {
        highAnimator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.1f);
        midAnimator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.1f);
        lowAnimator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.2f);
        animator.SetInteger("state", 2);

    }
    private void OnDisable()
    {
        if (isOpen)
        {
            isOpen = false;

            low_Text.fontSize = 0;
            mid_Text.fontSize = 0;
            high_Text.fontSize = 0;
            lowBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            midBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            highBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            graphicBorder.sizeDelta = Vector2.zero;
        }
    }
}
