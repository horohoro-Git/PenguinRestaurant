using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : SoundManager
{
    public List<int> uiClickSoundKey = new List<int>();
    [NonSerialized] public List<AudioClip> uiClickClips = new List<AudioClip>();
    private void Awake()
    {
        GameInstance.GameIns.uISoundManager = this;
        for (int i = 0; i < uiClickSoundKey.Count; i++)
        {
            uiClickClips.Add(AssetLoader.loadedSounds[AssetLoader.sounds[uiClickSoundKey[i]].Name]);
        }
    }

    public AudioClip UIClick()
    {
        int r = UnityEngine.Random.Range(0, uiClickClips.Count);
        return uiClickClips[r];
    }
}
