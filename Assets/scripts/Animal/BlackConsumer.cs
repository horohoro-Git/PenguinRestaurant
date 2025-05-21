using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameInstance;

public class BlackConsumer : AnimalController
{
    bool bRunaway;
   // public AnimalStruct animalStruct { get; set; }
    void CauseTrouble()
    {
        if (!bRunaway)
        {
            List<Table> tables = GameIns.workSpaceManager.tables;
            tables = tables.OrderBy(t => t.transform.position - trans.position).ToList();

            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i].foodStacks.Count > 0)
                {
                    FoundTheTarget();
                    return;
                }
            }

            Patrol();
        }
        else
        {
            RunAway();
        }
    }
   
    void Patrol()
    {

    }

    void FoundTheTarget()
    {

    }

    void StealFood()
    {

    }

    void RunAway()
    {
        bRunaway = false;
    }
}
