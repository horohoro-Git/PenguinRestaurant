using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//?œê?

public static class GarbageManager
{
    static Queue<Garbage> deActivatedGarbages = new Queue<Garbage>();
    static List<Garbage> activatedGarbages = new List<Garbage>();
    public static GameObject garbageCollects;
    public static void NewGarbage(Garbage garbage, int count)
    {
        garbageCollects = new GameObject();
        garbageCollects.name = "GarbageCollects";
        garbageCollects.transform.position = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            Garbage g = GameObject.Instantiate(garbage, garbageCollects.transform);

            g.gameObject.SetActive(false);
            deActivatedGarbages.Enqueue(g);
        
        }
    }

    public static Garbage CreateGarbage()
    {
      
        if (deActivatedGarbages.Count == 0)
        {
            NewGarbage(GameInstance.GameIns.workSpaceManager.garbage, 10);
        }

        Garbage g = deActivatedGarbages.Dequeue();
        g.gameObject.SetActive(true);
        activatedGarbages.Add(g);

        return g;
    }

    public static void ClearGarbage(Garbage garbage)
    {
        garbage.gameObject.SetActive(false);
        deActivatedGarbages.Enqueue(garbage);
        activatedGarbages.Remove(garbage);
    }
}
