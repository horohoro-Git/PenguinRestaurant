using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameInstance;

public class StoreGoods : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject furniture;
    public GameObject furniture_Preview;
    public GameObject currnet;
    public RectTransform itemImage;
    Vector3 origin;
    bool currentCheckArea = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        currentCheckArea = false;
        GameIns.inputManager.inputDisAble = true;
        currnet = Instantiate(furniture_Preview);
        origin = itemImage.position;
      //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameIns.inputManager.CheckClickedUI())
        {
            itemImage.position = Input.mousePosition;
          //  currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);

        }
        else
        {
            itemImage.position = origin;
            Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
            {
                //    currnet.transform.position = hit.point;
                currentCheckArea = GameIns.gridManager.SelectLine(hit.point, currnet, currentCheckArea);
            }
        //    currnet.transform.position = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);
        
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //  if (currnet != null) Destroy(currnet);
        // currnet = null;
        if (currentCheckArea)
        {
            Debug.Log("Success");

        }
        else
        {
            Debug.Log("Fail");
        }

        itemImage.position = origin;
        GameIns.inputManager.inputDisAble = false;
    }

    public void Purchase()
    {
      //  GameInstance.GameIns.inputManager.draggingFurniture = 
    }
}
