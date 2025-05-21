using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBlockController : MonoBehaviour
{
    private Renderer renderers;
    public int colorParam = 0;
   // MaterialPropertyBlock propBlock;
    private void Awake()
    {
        renderers = GetComponent<Renderer>();
    //    propBlock = new MaterialPropertyBlock();
    }

    public void Set(int colorParam, Material m)
    {
        this.colorParam = colorParam;
        renderers.sharedMaterial = m;
       // propBlock.SetFloat("_ColorBlend", colorParam);
        //renderer.SetPropertyBlock(propBlock);
    }
    public int GetColorParam()
    {
        return colorParam;
    }
}
