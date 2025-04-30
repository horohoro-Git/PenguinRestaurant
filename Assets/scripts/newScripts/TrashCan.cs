using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class TrashCan : MonoBehaviour
{
    public Transform throwPos;
  //  public GameObject throwPlace;
    public Transform transforms;

    private void Awake()
    {
        transforms = transform;
    }
}
