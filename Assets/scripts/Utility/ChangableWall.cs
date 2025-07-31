using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangableWall : MonoBehaviour
{

    MeshRenderer meshRender;

    Color highlightColor = new Color(1, 1, 0, 1);
    MaterialPropertyBlock propertyBlock;
    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        meshRender = GetComponent<MeshRenderer>();
    }
  

    public void Highlight(bool visible)
    {
        if (visible)
        {
            if (meshRender != null)
            {
                meshRender.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_BaseColor", highlightColor);
                meshRender.SetPropertyBlock(propertyBlock);
            }

        }
        else
        {
            if (meshRender != null)
            {
                meshRender.SetPropertyBlock(null);
            
            }
        }

    }
}
