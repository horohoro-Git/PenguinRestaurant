using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Button languageBtn;
    LanguageSetting languageSetting;


    private void Awake()
    {
        languageSetting = GetComponentInChildren<LanguageSetting>();
    }

    private void Start()
    {
        languageBtn.onClick.AddListener(() =>
        {
            languageSetting.Click();
        });

    }
}
