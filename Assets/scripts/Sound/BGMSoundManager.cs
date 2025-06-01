using SRDebugger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSoundManager : SoundManager
{
    public List<int> bgmSoundKey = new List<int>();
    public List<AudioClip> bgmClips = new List<AudioClip>();
    private void Awake()
    {
        for (int i = 0; i < bgmSoundKey.Count; i++)
        {
            bgmClips.Add(AssetLoader.loadedSounds[AssetLoader.sounds[bgmSoundKey[i]].Name]);
        }
    }

    public AudioClip BGM()
    {
        int r = UnityEngine.Random.Range(0, bgmClips.Count);
        return bgmClips[r];
    }
}
