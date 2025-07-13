using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangableWall : MonoBehaviour
{

    MeshRenderer meshRender;
    Color normalColor;
    Color highlightColor = new Color(1, 1, 0, 1);
    void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
        normalColor = meshRender.material.color;
    }
  

    public void Highlight(bool visible)
    {
        if (visible)
        {
            if(meshRender != null) meshRender.material.color = highlightColor;
        
          
        }
        else
        {
            if (meshRender != null) meshRender.material.color = normalColor;
        }
        
    }
}
