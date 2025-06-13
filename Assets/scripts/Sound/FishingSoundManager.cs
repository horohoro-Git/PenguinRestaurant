using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetLoader;
public class FishingSoundManager : SoundManager
{
 
    public List<int> dropletSoundKeys = new List<int>();
    public List<int> waterSplashSoundKeys = new List<int>();   

    List<AudioClip> dropletSounds = new List<AudioClip>();
    List<AudioClip> waterSplashSounds = new List<AudioClip>();

    private void Awake()
    {
        GameInstance.GameIns.fishingSoundManager = this;

        for (int i = 0; i < dropletSoundKeys.Count; i++) dropletSounds.Add(loadedSounds[sounds[dropletSoundKeys[i]].Name]);
        for (int i = 0; i < waterSplashSoundKeys.Count; i++) waterSplashSounds.Add(loadedSounds[sounds[waterSplashSoundKeys[i]].Name]);
    }

    public AudioClip DropletSound()
    {
        int r = Random.Range(0, dropletSounds.Count);
        return dropletSounds[r];
    }

    public AudioClip WaterSplashSound()
    {
        int r = Random.Range(0, waterSplashSounds.Count);
        return waterSplashSounds[r];
    }
}
