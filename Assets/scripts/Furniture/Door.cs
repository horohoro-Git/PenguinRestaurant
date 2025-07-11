using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : Furniture
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
        MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);
    }

    private void OnDisable()
    {
      /*  if (setup)
        {
            int[] ar = new int[3] { -10, 0, 10 };
            for (int i = 0; i < 3; i++)
            {
                if (Physics.Raycast(transform.position + Vector3.up, transform.forward * ar[i], out RaycastHit hits, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    Debug.DrawLine(transform.position + Vector3.up, transform.position - transform.forward * ar[i] * float.MaxValue, Color.red, 5);
                    Debug.Log("Hit");
                    GameObject h = hits.collider.gameObject;
                    removeWall = h;
                    h.SetActive(false);
                    transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    break;
                }
            }
        }
        */
    }
}
