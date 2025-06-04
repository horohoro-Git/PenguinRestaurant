using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : SoundManager
{
 
    public List<int> dropPlateSoundKeys = new List<int>();
    public List<int> machinesSoundKeys = new List<int>();
    public List<int> throwSoundKeys = new List<int>();
    public List<int> createSoundKeys = new List<int>();
    public List<int> quackSoundKeys = new List<int>();
    public List<int> happySoundKeys = new List<int>();
    public List<int> eatSoundKeys = new List<int>();
    public Dictionary<int, AudioClip> machinesSounds = new Dictionary<int, AudioClip>();
    [NonSerialized] public List<AudioClip> dropPlateSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> throwSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> createSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> quackSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> happySounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> eatSounds = new List<AudioClip>();



    private void Awake()
    {
        GameInstance.GameIns.gameSoundManager = this;

        for (int i = 0; i < machinesSoundKeys.Count; i++) machinesSounds[machinesSoundKeys[i]] = AssetLoader.loadedSounds[AssetLoader.sounds[machinesSoundKeys[i]].Name];

        for (int i = 0; i < dropPlateSoundKeys.Count; i++) dropPlateSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[dropPlateSoundKeys[i]].Name]);
        for (int i = 0; i < throwSoundKeys.Count; i++) throwSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[throwSoundKeys[i]].Name]);
        for (int i = 0; i < createSoundKeys.Count; i++) createSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[createSoundKeys[i]].Name]);
        for (int i = 0; i < quackSoundKeys.Count; i++) quackSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[quackSoundKeys[i]].Name]);
        for (int i = 0; i < happySoundKeys.Count; i++) happySounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[happySoundKeys[i]].Name]);
        for (int i = 0; i < eatSoundKeys.Count; i++) eatSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[eatSoundKeys[i]].Name]);
    }

    public AudioClip DropPlate()
    {
        int r = UnityEngine.Random.Range(0, dropPlateSounds.Count);
        return dropPlateSounds[r];
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
    public AudioClip Quack()
    {
        int r = UnityEngine.Random.Range(0, quackSounds.Count);
        return quackSounds[r];
    }
    public AudioClip Happy()
    {
        int r = UnityEngine.Random.Range(0, happySounds.Count);
        return happySounds[r];
    }
    public AudioClip Eat()
    {
        int r = UnityEngine.Random.Range(0, eatSounds.Count);
        return eatSounds[r];
    }
}
