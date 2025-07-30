using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSprite : MonoBehaviour
{
    public bool bImage;
    public string atlasName;
    public string assetName;
    private void Awake()
    {
        if (!bImage)
        {
            if (AssetLoader.loadedAtlases.ContainsKey(atlasName)) GetComponent<SpriteRenderer>().sprite = AssetLoader.loadedAtlases[atlasName].GetSprite(assetName);
            //GetComponent<SpriteRenderer>().material.renderQueue = 2800;
        }
        else
        {
            if (AssetLoader.loadedAtlases.ContainsKey(atlasName)) GetComponent<Image>().sprite = AssetLoader.loadedAtlases[atlasName].GetSprite(assetName);
        }
    }
}
