using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandDetails : MonoBehaviour
{
    public List<GameObject> removeObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < removeObjects.Count; i++) 
        {
            removeObjects[i].SetActive(false);
        }
    }

}
