using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBlockController : MonoBehaviour
{
    Renderer renderer;
    public Material red;
    public Material green;
    int colorParam = 0;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        
    }

    public void Set(int colorParam)
    {
        this.colorParam = colorParam;
        if(colorParam == 1)
        {
          
            renderer.sharedMaterial = red;
        }
        else
        {
            renderer.sharedMaterial = green;
        }
    }
    public int GetColorParam()
    {
        return colorParam;
    }
}
