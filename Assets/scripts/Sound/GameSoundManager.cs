using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : SoundManager
{
 
    public List<int> machinesSoundKeys = new List<int>();
    public List<int> throwSoundKeys = new List<int>();
    public List<int> createSoundKeys = new List<int>();
    public Dictionary<int, AudioClip> machinesSounds = new Dictionary<int, AudioClip>();
    [NonSerialized] public List<AudioClip> throwSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> createSounds = new List<AudioClip>();



    private void Awake()
    {
        GameInstance.GameIns.gameSoundManager = this;

        for (int i = 0; i < machinesSoundKeys.Count; i++) machinesSounds[machinesSoundKeys[i]] = AssetLoader.loadedSounds[AssetLoader.sounds[machinesSoundKeys[i]].Name];

        for (int i = 0; i < throwSoundKeys.Count; i++) throwSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[throwSoundKeys[i]].Name]);
        for (int i = 0; i < createSoundKeys.Count; i++) createSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[createSoundKeys[i]].Name]);
    }



    public AudioClip MachineSound(int key)
    {
        return machinesSounds[key];
    }

    public AudioClip ThrowSound()
    {
        int r = UnityEngine.Random.Range(0, throwSounds.Count);
        return throwSounds[r];
    }
    public AudioClip CreateFood()
    {
        return createSounds[0];
    }
}
