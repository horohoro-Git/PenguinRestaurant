using SRDebugger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSoundManager : SoundManager
{
    public AudioSource Audios { get; set; }
    public List<int> bgmSoundKey = new List<int>();
    public Dictionary<int, AudioClip> bgmClips = new Dictionary<int, AudioClip>();
    Coroutine bgmCoroutine;
    private void Awake()
    {
        
        DontDestroyOnLoad(this);
        Audios = GetComponent<AudioSource>();
        GameInstance.GameIns.bgMSoundManager = this;
    }

    private void Start()
    {
        Audios.volume = App.gameSettings.soundBackgrounds == true ? 1 : 0;
    }
    public void Setup()
    {
        for (int i = 0; i < bgmSoundKey.Count; i++) bgmClips[bgmSoundKey[i]] = AssetLoader.loadedSounds[AssetLoader.sounds[bgmSoundKey[i]].Name];
       // BGM(910000, 0.4f);
    }
  
    public void BGMChange(int index, float vol)
    {
        if(bgmCoroutine != null) StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(Changing(index, vol));
       
    }

    IEnumerator Changing(int index, float vol)
    {
        float f = 0;
        float volume = Audios.volume;
        while (f <= 0.2f)
        {
            int volumeMultiply = App.gameSettings.soundBackgrounds ? 1 : 0;
            Audios.volume = volume * (0.2f - f) * 5 * volumeMultiply;
            f += Time.unscaledDeltaTime;
            yield return null;
        }


        f = 0;
        Audios.clip = bgmClips[index];
        Audios.Play();
        while (f <= 0.1f)
        {
            int volumeMultiply = App.gameSettings.soundBackgrounds ? 1 : 0;
            Audios.volume = vol * (f) * 10f * volumeMultiply;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        Audios.volume = App.gameSettings.soundBackgrounds ? 1 : 0;
    }
}
