using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicRaycastingUI : MonoBehaviour
{
    GraphicRaycaster gr;

    private void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }
    private void OnEnable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.AddGraphicCaster(gr);
    }
    private void OnDisable()
    {
        if (gr == null) gr = GetComponent<GraphicRaycaster>();
        GameInstance.RemoveGraphicCaster(gr);
    }
}
