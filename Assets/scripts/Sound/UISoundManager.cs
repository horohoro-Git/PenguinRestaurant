using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetLoader;
using Random = UnityEngine.Random;

public class UISoundManager : MonoBehaviour
{
    public List<int> uiClickSoundKey = new List<int>();
    public List<int> uiPopSoundKey = new List<int>();
    public List<int> furnitureClickSoundKey = new List<int>();
    public List<int> furniturePlaceSoundKey = new List<int>();
    public List<int> furniturePurchaseSoundKey = new List<int>();
    public List<int> moneySoundKey = new List<int>();
    public List<int> fishesSoundKey = new List<int>();
    public List<int> fishSoundKey = new List<int>();
    public List<int> spreadSoundKey = new List<int>();
    public List<int> foldSoundKey = new List<int>();
    [NonSerialized] public List<AudioClip> uiClickClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> uiPopClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> furnitureClickClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> furniturePlaceClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> furniturePurchaseClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> moneyClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> fishesClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> fishClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> spreadClips = new List<AudioClip>();
    [NonSerialized] public List<AudioClip> foldClips = new List<AudioClip>();
    private void Awake()
    {
        
        GameInstance.GameIns.uISoundManager = this;
        if (GameInstance.GameIns.assetLoader != null)
        {
            for (int i = 0; i < uiClickSoundKey.Count; i++) uiClickClips.Add(loadedSounds[sounds[uiClickSoundKey[i]].Name]);
            for (int i = 0; i < uiPopSoundKey.Count; i++) uiPopClips.Add(loadedSounds[sounds[uiPopSoundKey[i]].Name]);
            for (int i = 0; i < furnitureClickSoundKey.Count; i++) furnitureClickClips.Add(loadedSounds[sounds[furnitureClickSoundKey[i]].Name]);
            for (int i = 0; i < furniturePlaceSoundKey.Count; i++) furniturePlaceClips.Add(loadedSounds[sounds[furniturePlaceSoundKey[i]].Name]);
            for (int i = 0; i < furniturePurchaseSoundKey.Count; i++) furniturePurchaseClips.Add(loadedSounds[sounds[furniturePurchaseSoundKey[i]].Name]);
            for (int i = 0; i < moneySoundKey.Count; i++) moneyClips.Add(loadedSounds[sounds[moneySoundKey[i]].Name]);
            for (int i = 0; i < fishesSoundKey.Count; i++) fishesClips.Add(loadedSounds[sounds[fishesSoundKey[i]].Name]);
            for (int i = 0; i < fishSoundKey.Count; i++) fishClips.Add(loadedSounds[sounds[fishSoundKey[i]].Name]);
            for (int i = 0; i < spreadSoundKey.Count; i++) spreadClips.Add(loadedSounds[sounds[spreadSoundKey[i]].Name]);
            for (int i = 0; i < foldSoundKey.Count; i++) foldClips.Add(loadedSounds[sounds[foldSoundKey[i]].Name]);
        }
    }

    public AudioClip UIClick()
    {
        int r = Random.Range(0, uiClickClips.Count);
        return uiClickClips[r];
    }

    public AudioClip UIPop()
    {
        int r = Random.Range(0, uiPopClips.Count);
        return uiPopClips[r];
    }

    public AudioClip FurnitureClick()
    {
        int r = Random.Range(0, furnitureClickClips.Count);
        return furnitureClickClips[r];
    }
    public AudioClip FurniturePlace()
    {
        int r = Random.Range(0, furniturePlaceClips.Count);
        return furniturePlaceClips[r];
    }
    public AudioClip FurniturePurchase()
    {
        int r = Random.Range(0, furniturePurchaseClips.Count);
        return furniturePurchaseClips[r];
    }
    public AudioClip Money()
    {
        int r = Random.Range(0, moneyClips.Count);
        return moneyClips[r];
    }
    public AudioClip Fishes()
    {
        int r = Random.Range(0, fishesClips.Count);
        return fishesClips[r];
    }
    public AudioClip Fish()
    {
        int r = Random.Range(0, fishClips.Count);
        return fishClips[r];
    }
    public AudioClip Spread()
    {
        int r = Random.Range(0, spreadClips.Count);
        return spreadClips[r];
    }
    public AudioClip Fold()
    {
        int r = Random.Range(0, foldClips.Count);
        return foldClips[r];
    }
}
