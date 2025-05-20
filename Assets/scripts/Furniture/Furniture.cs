using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public int id { get; set; }

    [field: SerializeField]
    public WorkSpaceType spaceType { get; set; }

    public bool spawned;

}
