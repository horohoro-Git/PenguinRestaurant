using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

    Transform trans;
    private void Awake()
    {
        trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        trans.localEulerAngles = new Vector3(0, 0, trans.localEulerAngles.z - 120 * Time.deltaTime);
    }
}
