using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class TrashCan : Furniture, IObjectOffset
{
    public Transform throwPos;
  //  public GameObject throwPlace;
    public Transform transforms;

    [field: SerializeField]
    public Transform offset { get; set; }

    private void Awake()
    {
        transforms = transform;
    }
    public override void Start()
    {
        GameInstance.GameIns.workSpaceManager.trashCans.Add(this);
        base.Start();
    }
}
