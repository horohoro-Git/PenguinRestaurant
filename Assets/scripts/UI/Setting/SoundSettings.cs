using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public Button effectBtn;
    public Button bgmBtn;
    public GameObject effectChecked;
    public GameObject bgmChecked;
    bool bgmSound;
    bool effectSound;

    private void Awake()
    {
        if (App.gameSettings.soundEffects)
        {
            effectSound = true;
            effectChecked.gameObject.SetActive(true);
        }
        else
        {
            effectSound = false;
            effectChecked.gameObject.SetActive(false);
        }
        if (App.gameSettings.soundBackgrounds)
        {
            bgmSound = true;
            bgmChecked.gameObject.SetActive(true);
        }
        else
        {
            bgmSound = false;
            bgmChecked.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        effectBtn.onClick.AddListener(() => {
            if (App.gameSettings.soundEffects)
            {
                effectChecked.SetActive(false);
                foreach(var v in SoundManager.Instance.currentPlayingAudioSouces) v.volume = 0;
         
                foreach(var v in GameInstance.GameIns.workSpaceManager.foodMachines) v.audioSource.volume = 0;
          
                App.gameSettings.soundEffects = false;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
            else
            {
                foreach (var v in SoundManager.Instance.currentPlayingAudioSouces) v.volume = 1f;
                foreach (var v in GameInstance.GameIns.workSpaceManager.foodMachines) v.audioSource.volume = 1f;
                effectChecked.SetActive(true);
                App.gameSettings.soundEffects = true;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
        });

        bgmBtn.onClick.AddListener(() => { 
            if(App.gameSettings.soundBackgrounds)
            {
                bgmChecked.SetActive(false);
                App.gameSettings.soundBackgrounds = false;
                GameInstance.GameIns.bgMSoundManager.audio.volume = 0;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
            else
            {
                bgmChecked.SetActive(true);
                App.gameSettings.soundBackgrounds = true;
                GameInstance.GameIns.bgMSoundManager.audio.volume = 1;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
        });
    }
}
