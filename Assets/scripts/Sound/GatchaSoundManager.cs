using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatchaSoundManager : SoundManager
{
    public List<int> unlockSoundKeys = new List<int>();
    public List<int> gradeupSoundKeys = new List<int>();
    public List<int> purchaseSoundKeys = new List<int>();
    [NonSerialized] public List<AudioClip> unlockSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> gradeupSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> purchaseSounds = new List<AudioClip>();
    private void Awake()
    {
        GameInstance.GameIns.gatchaSoundManager = this;
        for (int i = 0; i < unlockSoundKeys.Count; i++) unlockSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[unlockSoundKeys[i]].Name]);
        for(int i = 0;i < gradeupSoundKeys.Count; i++) gradeupSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[gradeupSoundKeys[i]].Name]);
        for(int i = 0;i < purchaseSoundKeys.Count; i++) purchaseSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[purchaseSoundKeys[i]].Name]);
    }

    public AudioClip Unlock()
    {
        int r = UnityEngine.Random.Range(0, unlockSounds.Count);
        return unlockSounds[r];
    }

    public AudioClip GradeUp()
    {
        int r = UnityEngine.Random.Range(0, gradeupSounds.Count);
        return gradeupSounds[r];
    }

    public AudioClip Purchase()
    {
        int r = UnityEngine.Random.Range(0, purchaseSounds.Count);
        return purchaseSounds[r];
    }
}
