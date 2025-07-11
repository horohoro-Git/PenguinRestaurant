using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public AnimalController animal;// { get; set; }
   // public AnimalController animal;
    public Transform transforms;
    Table table;

    bool disabled = false;
    public bool isDisEnabled
    {
        get { return disabled; }
        set
        {
            if (disabled != value)
            {
                disabled = value;
                if (disabled)
                {
                    table.disableNum++;
                }
                else
                {
                    table.disableNum--;
                }
            }
        }
    }

    private void Awake()
    {
        transforms = transform;
        table = GetComponentInParent<Table>();
    }
}
