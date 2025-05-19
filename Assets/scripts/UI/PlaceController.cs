using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceController : MonoBehaviour
{
    public GameObject offset;
    public Image applyImage;
    public Image applyborderImage;
    public GraphicRaycaster raycaster;
    int[] rotates = new int[4] { 0, 270, 180, 90 };
    public Vector3[] rotateOffsets = new Vector3[4];
    int level = 0;

    bool place;
    public Vector2 offsetVector = new Vector2();
    static readonly Color gray = new Color32(128, 128, 128, 255);
    static readonly Color normal = new Color32(255, 255, 255, 255);
    bool isDragging;
    public bool canPlace { get { return place; }  set { 
        
            place = value;
            if(!place)
            {
                applyImage.color = gray;
                applyborderImage.color = gray;
            }
            else
            {
                applyborderImage.color = normal;
                applyImage.color = normal;
            }
        } }

    [NonSerialized]
    public List<BoxCollider> colliders = new List<BoxCollider>();
    [NonSerialized] public StoreGoods storeGoods;

    private void Start()
    {
        applyImage.gameObject.layer = 5;
        applyborderImage.gameObject.layer = 5;
    }
    private void OnEnable()
    {
        GameInstance.AddGraphicCaster(raycaster);
    }
    private void OnDisable()
    {
        GameInstance.RemoveGraphicCaster(raycaster);
    }

    private void Update()
    {
        if(InputManger.cachingCamera != null)
        {
            if(Input.GetMouseButton(0) && isDragging)
            {
                Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
                {
                    //    currnet.transform.position = hit.point;
                    GameInstance.GameIns.gridManager.SelectLine(hit.point, this, canPlace);
                }
            }

            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 17))
                {
                    if (hit.collider.gameObject.GetComponentInParent<PlaceController>() == this)
                    {
                        GameInstance.GameIns.inputManager.inputDisAble = true;
                        isDragging = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                GameInstance.GameIns.inputManager.inputDisAble = false;
            }
        }
    }
    public void Rotate()
    {
        if(level < 3) level++;
        else level = 0;
        
        offset.transform.rotation = Quaternion.Euler(0, rotates[level], 0);
        offset.transform.localPosition = rotateOffsets[level];
        GameInstance.GameIns.gridManager.CheckObject(this);
    }

    public void Apply()
    {
        if (canPlace)
        {
            GameInstance.GameIns.gridManager.ApplyGird();
            storeGoods.PlaceGoods(rotateOffsets[level]);
            // GameInstance.GameIns.gridManager.RemoveCell();
            storeGoods.RemoveGoodsPreview();
        }
    }

    public void Cancel()
    {
        GameInstance.GameIns.gridManager.RemoveCell();
        storeGoods.RemoveGoodsPreview();
    }

    public Vector3 GetLocation()
    {
        Vector3 returnVector = Vector3.zero;



        return returnVector;
    }
}
