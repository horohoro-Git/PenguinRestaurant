using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class AnimalShowcase : MonoBehaviour
{
    public List<GameObject> animals = new List<GameObject>();

    public ShowcaseCamera showcaseCamera;

    int layer = 0;
    Coroutine cameraMove;

    private void Start()
    {
        Vector3 init = Camera.main.transform.position;

        showcaseCamera.transform.position = new Vector3(0, init.y, init.z);
        showcaseCamera.cachingCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
    }
    private void Update()
    {
     /*   if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(layer > 0)
            {
                layer--;
                if(cameraMove != null) StopCoroutine(cameraMove);
                cameraMove = StartCoroutine(MoveCamera());
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(layer < animals.Count - 1)
            {
                layer++;
                if (cameraMove != null) StopCoroutine(cameraMove);
                cameraMove = StartCoroutine(MoveCamera());
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            string p = Application.persistentDataPath + "/" + animals[layer].name + ".png";
            showcaseCamera.ScreenShot(p);
           
        }*/
    }


    IEnumerator MoveCamera()
    {
        float timer = 0;

        Vector3 currnet = showcaseCamera.transform.position;
        Vector3 target = new Vector3(layer * 5, currnet.y, currnet.z);
        while (timer <= 0.2f)
        {
            timer += Time.deltaTime;

            Vector3 move = Vector3.Lerp(currnet, target, timer * 5);

            showcaseCamera.transform.position = move;
            yield return null;
        }
    }
}
