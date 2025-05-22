using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    bool spawnDelay; 
    public bool SpawnTarget()
    {
        if (spawnDelay) return false;

        spawnDelay = true;
        StartCoroutine(SpawnDelay());
        return true;
    }
    
    IEnumerator SpawnDelay()
    {
        yield return CoroutneManager.waitForone;
        spawnDelay = false;
    }
}
