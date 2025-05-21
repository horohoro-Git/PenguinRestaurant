using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class PackageFood : Food
{
    public Transform[] packageTrans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   /* void Update()
    {
        for(int i=0; i< packageTrans.Length; i++)
        {
            Food f = packageTrans[i].GetComponentInChildren<Food>();
            if (f!=null)
            {
                f.transform.localPosition = Vector3.zero;
            }
        }
    }*/
}
