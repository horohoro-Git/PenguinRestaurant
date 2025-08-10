using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    GraphicRaycaster raycaster;
    public Button languageBtn;
    LanguageSetting languageSetting;


    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
       
        languageSetting = GetComponentInChildren<LanguageSetting>();
    }

    private void OnEnable()
    {
        GameInstance.AddGraphicCaster(raycaster);
    }
    private void OnDisable()
    {
        GameInstance.RemoveGraphicCaster(raycaster);
    }

    private void Start()
    {
        languageBtn.onClick.AddListener(() =>
        {
            languageSetting.Click();
        });

    }
}
