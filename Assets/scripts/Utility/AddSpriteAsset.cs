using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class AddSpriteAsset : MonoBehaviour
{
    public string spriteAssetName;
    private void Awake()
    {
        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>(spriteAssetName);

        GetComponent<TMP_Text>().spriteAsset = spriteAsset;
    }
}
