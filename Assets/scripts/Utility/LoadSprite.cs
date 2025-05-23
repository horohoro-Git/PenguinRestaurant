using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSprite : MonoBehaviour
{
    public string atlasName;
    public string assetName;
    private void Awake()
    {
        if(AssetLoader.loadedAtlases.ContainsKey(atlasName)) GetComponent<SpriteRenderer>().sprite = AssetLoader.loadedAtlases[atlasName].GetSprite(assetName);
    }
}
