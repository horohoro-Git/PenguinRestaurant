using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Button languageBtn;
    public Button graphicBtn;
    LanguageSetting languageSetting;

    GraphicSetting graphicSetting;

    private void Awake()
    {
        languageSetting = GetComponentInChildren<LanguageSetting>();
        graphicSetting = GetComponentInChildren<GraphicSetting>();
    }

    private void Start()
    {
        languageBtn.onClick.AddListener(() =>
        {
            languageSetting.Click();
        });

        graphicBtn.onClick.AddListener(() =>
        {
            graphicSetting.Click();
        });
    }
}
