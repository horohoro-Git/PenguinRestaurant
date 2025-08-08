using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class GraphicSetting : Settings
{
   
 
    public Button lower;
    public Button upper;
    public TMP_Text graphicState;
  
    int graphicLevel = 0;

    private void Awake()
    {
        graphicLevel = 1 + (int)App.gameSettings.graphics;
        QualitySettings.SetQualityLevel(graphicLevel);
        int currentLevel = graphicLevel + 10010;
        graphicState.GetComponent<LanguageText>().id = currentLevel;
        graphicState.text = App.languages[currentLevel].text;
        
        if(graphicLevel == 1) lower.enabled = false;
        else if(graphicLevel == 3) upper.enabled = false;

        if (GameInstance.GameIns.playerCamera.cam.TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            if (graphicLevel == 3) cameraData.renderPostProcessing = true;
            else cameraData.renderPostProcessing = false;
        }

        lower.onClick.AddListener(() =>
        {
            graphicLevel--;
            if(!upper.enabled) upper.enabled = true;
            if(graphicLevel == 1) lower.enabled = false;

            ChangeQuality(graphicLevel);
        });

        upper.onClick.AddListener(() => { 
            graphicLevel++;
            if(!lower.enabled) lower.enabled = true;
            if(graphicLevel == 3) upper.enabled = false;
            ChangeQuality(graphicLevel);
        });
    }

    void ChangeQuality(int level)
    {
        QualitySettings.SetQualityLevel(graphicLevel);

        string currentLevelName = QualitySettings.names[graphicLevel];
        if (GameInstance.GameIns.playerCamera.cam.TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            if(level == 3) cameraData.renderPostProcessing = true;
            else cameraData.renderPostProcessing = false;
        }
        graphicState.GetComponent<LanguageText>().id = 10010 + graphicLevel;
        graphicState.text = App.languages[10010 + graphicLevel].text;
        App.gameSettings.graphics = (GraphicsLevel)(graphicLevel - 1);
        SaveLoadSystem.SaveGameSettings(App.gameSettings);
    }

   
}
