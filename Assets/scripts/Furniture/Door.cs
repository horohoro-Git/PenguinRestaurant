using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool setup;

    public GameObject removeWall;


    public IEnumerator OpenDoor()
    {
        Vector3 cur = Vector3.zero;
        Vector3 target = Vector3.one;
        Vector3 secondtarget = new Vector3(1.2f,1.2f,1.2f);
        float f = 0;
        while(f < 0.2f)
        {
            transform.localScale = Vector3.Lerp(cur, secondtarget, f * 5);
            f += Time.deltaTime;
            yield return null;
        }
        f = 0;
        while(f < 0.1f)
        {
            transform.localScale = Vector3.Lerp(secondtarget, target, f * 10);
            f+= Time.deltaTime;
            yield return null;
        }
        transform.localScale = target;

        Debug.Log("AAA");
    }
}
