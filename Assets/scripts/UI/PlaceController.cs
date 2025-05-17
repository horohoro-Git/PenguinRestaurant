using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceController : MonoBehaviour
{
    public GameObject offset;
    int[] rotates = new int[4] { 0, 90, 180, 270 };
    public Vector3[] rotateOffsets = new Vector3[4];
    int level = 0;
    public void Rotate()
    {
        if(level < 3) level++;
        else level = 0;
        
        offset.transform.rotation = Quaternion.Euler(0, rotates[level], 0);
        offset.transform.localPosition = rotateOffsets[level];
        GameInstance.GameIns.gridManager.CheckObject(gameObject);
    }

    public void Apply()
    {

    }

    public void Cancel()
    {

    }
}
