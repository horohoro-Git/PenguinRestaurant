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
        
    }

    private void Start()
    {
        effectBtn.onClick.AddListener(() => {
            if (effectSound)
            {
                effectChecked.SetActive(false);
                effectSound = false;
            }
            else
            {
                effectChecked.SetActive(true);
                effectSound = true;
            }
        });

        bgmBtn.onClick.AddListener(() => { 
            if(bgmSound)
            {
                bgmChecked.SetActive(false);
                bgmSound = false;   
            }
            else
            {
                bgmChecked.SetActive(true);
                bgmSound = true;
            }
        });
    }
}
