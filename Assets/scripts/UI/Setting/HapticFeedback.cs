using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HapticFeedback : Settings
{
    public Button button;
    public GameObject checkImage;
    private void Awake()
    {
        if (App.gameSettings.hapticFeedback)
        {
            checkImage.gameObject.SetActive(true);
        }
        else
        {
            checkImage.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if(App.gameSettings.hapticFeedback)
            {
                App.gameSettings.hapticFeedback = false;    
                checkImage.gameObject.SetActive(false);
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
            else
            {
                App.gameSettings.hapticFeedback = true;
                checkImage.gameObject.SetActive(true);
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
        });
    }
}
