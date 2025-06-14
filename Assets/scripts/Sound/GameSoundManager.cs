using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
 
    public List<int> dropPlateSoundKeys = new List<int>();
    public List<int> machinesSoundKeys = new List<int>();
    public List<int> throwSoundKeys = new List<int>();
    public List<int> createSoundKeys = new List<int>();
    public List<int> extensionSoundKeys = new List<int>();
    public List<int> hitSoundKeys = new List<int>();
    public List<int> painSoundKeys = new List<int>();
    public List<int> quackSoundKeys = new List<int>();
    public List<int> happySoundKeys = new List<int>();
    public List<int> eatSoundKeys = new List<int>();
    public List<int> laughAtSoundKeys = new List<int>();
    public List<int> angryScreamSoundKeys = new List<int>();
    public List<int> sadScreamScreamSoundKeys = new List<int>();
    public List<int> levelupSoundKeys = new List<int>();
    public Dictionary<int, AudioClip> machinesSounds = new Dictionary<int, AudioClip>();
    [NonSerialized] public List<AudioClip> dropPlateSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> throwSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> createSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> extensionSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> hitSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> painSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> quackSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> happySounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> eatSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> laughAtSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> angryScreamSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> sadScreamSounds = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> levelupSounds = new List<AudioClip>();



    private void Awake()
    {
        GameInstance.GameIns.gameSoundManager = this;

        for (int i = 0; i < machinesSoundKeys.Count; i++) machinesSounds[machinesSoundKeys[i]] = AssetLoader.loadedSounds[AssetLoader.sounds[machinesSoundKeys[i]].Name];

        for (int i = 0; i < dropPlateSoundKeys.Count; i++) dropPlateSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[dropPlateSoundKeys[i]].Name]);
        for (int i = 0; i < throwSoundKeys.Count; i++) throwSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[throwSoundKeys[i]].Name]);
        for (int i = 0; i < createSoundKeys.Count; i++) createSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[createSoundKeys[i]].Name]);
        for (int i = 0; i < extensionSoundKeys.Count; i++) extensionSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[extensionSoundKeys[i]].Name]);
        for (int i = 0; i < quackSoundKeys.Count; i++) quackSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[quackSoundKeys[i]].Name]);
        for (int i = 0; i < hitSoundKeys.Count; i++) hitSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[hitSoundKeys[i]].Name]);
        for (int i = 0; i < painSoundKeys.Count; i++) painSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[painSoundKeys[i]].Name]);
        for (int i = 0; i < happySoundKeys.Count; i++) happySounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[happySoundKeys[i]].Name]);
        for (int i = 0; i < eatSoundKeys.Count; i++) eatSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[eatSoundKeys[i]].Name]);
        for (int i = 0; i < laughAtSoundKeys.Count; i++) laughAtSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[laughAtSoundKeys[i]].Name]);
        for (int i = 0; i < angryScreamSoundKeys.Count; i++) angryScreamSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[angryScreamSoundKeys[i]].Name]);
        for (int i = 0; i < sadScreamScreamSoundKeys.Count; i++) sadScreamSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[sadScreamScreamSoundKeys[i]].Name]);
        for (int i = 0; i < levelupSoundKeys.Count; i++) levelupSounds.Add(AssetLoader.loadedSounds[AssetLoader.sounds[levelupSoundKeys[i]].Name]);
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

    public AudioClip Extension()
    {
        int r = UnityEngine.Random.Range(0, extensionSounds.Count);
        return extensionSounds[r];
    }
    public AudioClip Hit()
    {
        int r = UnityEngine.Random.Range(0,hitSounds.Count);
        return hitSounds[r];
    }

    public AudioClip Pain()
    {
        int r = UnityEngine.Random.Range(0, painSounds.Count);
        return painSounds[r];
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

    public AudioClip LaughAt()
    {
        int r = UnityEngine.Random.Range(0, laughAtSounds.Count);
        return laughAtSounds[r];
    }

    public AudioClip Angry()
    {
        int r = UnityEngine.Random.Range(0, angryScreamSounds.Count);
        return angryScreamSounds[r];
    }
    public AudioClip Sad()
    {
        int r = UnityEngine.Random.Range(0, sadScreamSounds.Count);
        return sadScreamSounds[r];
    }
    public AudioClip LevelUp()
    {
        int r = UnityEngine.Random.Range(0, levelupSounds.Count);
        return levelupSounds[r];
    }
}
