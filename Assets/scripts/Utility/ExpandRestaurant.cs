using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글



public class ExpandRestaurant : MonoBehaviour
{
    public GameObject[] enableObject;
    public GameObject[] disableObject;
    //public Expand[] expands;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Expand();
        }
    }

    void Expand()
    {
        if (enableObject.Length > i && disableObject.Length > i) {

            enableObject[i].SetActive(true);
            disableObject[i].SetActive(false);
            i++;
        }
        
    }
}
