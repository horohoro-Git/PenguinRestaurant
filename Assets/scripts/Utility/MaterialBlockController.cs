using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBlockController : MonoBehaviour
{
    Renderer renderer;
    public int colorParam = 0;
   // MaterialPropertyBlock propBlock;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    //    propBlock = new MaterialPropertyBlock();
    }

    public void Set(int colorParam, Material m)
    {
        this.colorParam = colorParam;
        renderer.sharedMaterial = m;
       // propBlock.SetFloat("_ColorBlend", colorParam);
        //renderer.SetPropertyBlock(propBlock);
    }
    public int GetColorParam()
    {
        return colorParam;
    }
}
