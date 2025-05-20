using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFoodSprite : MonoBehaviour
{
    public string assetName;
    private void Awake()
    {
        if(AssetLoader.loadedAtlases.ContainsKey("Foods")) GetComponent<SpriteRenderer>().sprite = AssetLoader.loadedAtlases["Foods"].GetSprite(assetName);
    }
}
