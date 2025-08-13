using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials
{

    public int id;
    public bool worked;
    public int count;

    public Tutorials(int id, bool worked)
    {
        this.id = id;
        this.worked = worked;
        count = 0;  
    }
}
