using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTarget : MonoBehaviour
{
    public enum WorkSpaceType
    {
        None,
        Counter,
        Table,
        FoodMachine
    }

    public int money;

    public MachineType type;
    public WorkSpaceType workSpaceType;
  //  public GameObject targetObject;

}
