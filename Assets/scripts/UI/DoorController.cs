using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class DoorController : MonoBehaviour
{
  //  public GameObject model;
    public GameObject rotateOffset;
    public GraphicRaycaster raycaster;
    public Button applyBtn;
    public Image applyImage;
    public Image applyborderImage;
    private Vector2 lastInputPosition;
    bool hasInput;
    Mouse currentMouse;
    Vector3 prevPos;
    Quaternion prevRot;
    bool isDragging;
    bool place;
    public bool CanPlace
    {
        get { return place; }
        set
        {

            place = value;
            if (!place)
            {
                applyImage.color = gray;
                applyborderImage.color = gray;
            }
            else
            {
                applyborderImage.color = normal;
                applyImage.color = normal;
            }
        }
    }
    static readonly Color gray = new Color32(128, 128, 128, 255);
    static readonly Color normal = new Color32(255, 255, 255, 255);
    [NonSerialized] public GameObject currentWallObject;
    Vector3 extents;
    private void Awake()
    {
        extents = GetComponentInChildren<Collider>().bounds.extents;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        currentMouse = Mouse.current;
#endif
        GameInstance.AddGraphicCaster(raycaster);
        prevPos = transform.position;
        prevRot = transform.rotation;
    }
    private void OnDisable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        currentMouse = Mouse.current;
#endif
        GameInstance.RemoveGraphicCaster(raycaster);
    }

    private void Update()
    {
        if (InputManger.cachingCamera != null)
        {
            if (GameInstance.GameIns.inputManager.CheckClickedUI(1 << 5 | 1 << 14 | 1 << 18)) return;

#if UNITY_IOS || UNITY_ANDROID
            if (Touch.activeTouches.Count == 0) return;

            var touch = Touch.activeTouches[0];
            lastInputPosition = touch.screenPosition;

            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    HandleInputDown(lastInputPosition);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    HandleInputUp();
                    break;
            }

#else
            if (currentMouse.leftButton.isPressed && !isDragging)
            {
                Ray r = InputManger.cachingCamera.ScreenPointToRay(currentMouse.position.ReadValue());
                if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject gameObject = hit.collider.gameObject;
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
                    isDragging = true;
                    GameInstance.GameIns.inputManager.InputDisAble = true;
                    if(gameObject != currentWallObject)
                    {
                        if (currentWallObject != null)
                        {
                            MeshRenderer[] mr = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                            for(int i = 0; i < mr.Length; i++) mr[i].enabled = true;
                        }
                      
                        currentWallObject = gameObject;
                        MeshRenderer[] mrs = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < mrs.Length; i++) mrs[i].enabled = false;
                        Vector3 pos = currentWallObject.transform.position;
                        pos.y = 0;
                        transform.position = pos;
                        rotateOffset.transform.rotation = currentWallObject.transform.rotation;
                        
                        CheckDoorPlacement();
                        //   currentWallObject.SetActive(false);
                    }
                    else
                    {
                       
                        MeshRenderer[] mrs = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < mrs.Length; i++) mrs[i].enabled = false;
                        Vector3 pos = currentWallObject.transform.position;
                        pos.y = 0;
                        transform.position = pos;
                        rotateOffset.transform.rotation = currentWallObject.transform.rotation;      
                        CheckDoorPlacement();
                    }

                }
            }
            if (currentMouse.leftButton.isPressed)
            {
                isDragging = true;
            }
            if (currentMouse.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                GameInstance.GameIns.inputManager.InputDisAble = false;
            }
#endif

        }
    }
    public void Apply()
    {
        if (CanPlace)
        {
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
            Door door = GameInstance.GameIns.restaurantManager.door;
            door.interactCollide.gameObject.layer = 13;
            if (transform.position != GameInstance.GameIns.restaurantManager.door.transform.position)
            {
                door.removeWall.SetActive(true);
                //     MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, door.removeWall.GetComponentInChildren<Collider>(), true);
                door.transform.position = transform.position;
                door.transform.rotation = rotateOffset.transform.rotation * Quaternion.Euler(0, -90, 0);
            }
            if (currentWallObject != null)
            {
                door.removeWall = currentWallObject;
                MeshRenderer[] mr = door.removeWall.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < mr.Length; i++) mr[i].enabled = true;
                door.removeWall.gameObject.SetActive(false);
            }
            foreach (var v in GameInstance.GameIns.restaurantManager.changableWalls) v.Highlight(false);

            MeshRenderer meshRenderer = door.GetComponentInChildren<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            for (int i = 0; i < door.doorMat.Count; i++)
            {
                materials[i] = door.doorMat[i];
            }
            meshRenderer.materials = materials;
            GameInstance.GameIns.restaurantManager.ApplyPlaced(door, null, true);

            SaveLoadSystem.SaveRestaurantBuildingData();
            
            GameInstance.GameIns.restaurantManager.restaurantparams = SaveLoadSystem.LoadRestaurantBuildingData();
            gameObject.SetActive(false);
        }

    }

    public void Cancel()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
        if (currentWallObject != null)
        {
            MeshRenderer[] mr = currentWallObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mr.Length; i++) mr[i].enabled = true;
        }
        currentWallObject = null;   
        Door door = GameInstance.GameIns.restaurantManager.door;
        door.interactCollide.gameObject.layer = 13;
        MeshRenderer meshRenderer = door.GetComponentInChildren<MeshRenderer>();
        Material[] materials = meshRenderer.materials;
        for (int i = 0; i < door.doorMat.Count; i++)
        {
            materials[i] = door.doorMat[i];
        }
        meshRenderer.materials = materials;
        foreach (var v in GameInstance.GameIns.restaurantManager.changableWalls) v.Highlight(false);
     
        gameObject.SetActive(false);
    }

    private void HandleInputDown(Vector2 inputPosition)
    {
      
        Ray ray = InputManger.cachingCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 16 | 1 << 19))
        {
            GameObject gameObject = hit.collider.gameObject;
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
            isDragging = true;
            GameInstance.GameIns.inputManager.InputDisAble = true;
            if (gameObject != currentWallObject)
            {
                if (currentWallObject != null)
                {
                    currentWallObject.GetComponent<MeshRenderer>().enabled = true;
                }
                currentWallObject = gameObject;
                currentWallObject.GetComponent<MeshRenderer>().enabled = false;
                Vector3 pos = currentWallObject.transform.position;
                pos.y = 0;
                transform.position = pos;
                rotateOffset.transform.rotation = currentWallObject.transform.rotation;

                CheckDoorPlacement();
            }
            else
            {
                MeshRenderer[] mrs = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < mrs.Length; i++) mrs[i].enabled = false;
                Vector3 pos = currentWallObject.transform.position;
                pos.y = 0;
                transform.position = pos;
                rotateOffset.transform.rotation = currentWallObject.transform.rotation;
                CheckDoorPlacement();
            }
        }
    }

    private void HandleDrag(Vector2 inputPosition)
    {
        Ray r = InputManger.cachingCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1 << 17 | 1 << 19))
        {
         //   GameInstance.GameIns.gridManager.SelectLine(hit.point, this, canPlace, storeGoods.goods.type);
        }
    }

    private void HandleInputUp()
    {
        isDragging = false;
        GameInstance.GameIns.inputManager.InputDisAble = false;
    }

    public bool CheckDoorPlacement()
    {
        Collider collider = GetComponentInChildren<Collider>();
        Vector3 center = transform.position;

        Vector3 ex = Quaternion.Euler(0, rotateOffset.transform.eulerAngles.y, 0) * extents;
        float x1 = center.x - ex.x;
        float x2 = center.x + ex.x;
        float minX = x1 > x2 ? x2 : x1;
        float maxX = x1 > x2 ? x1 : x2;
        float z1 = center.z - ex.z;
        float z2 = center.z + ex.z;
        float minZ = z1 > z2 ? z2 : z1;
        float maxZ = z1 > z2 ? z1 : z2;

        float xMin2 = Mathf.FloorToInt(minX / 2.5f) * 2.5f;
        float xMax2 = Mathf.FloorToInt(maxX / 2.5f) * 2.5f;
        float zMin2 = Mathf.FloorToInt(minZ / 2.5f) * 2.5f;
        float zMax2 = Mathf.FloorToInt(maxZ / 2.5f) * 2.5f;
        float rotate = rotateOffset.transform.eulerAngles.y / 90;
        bool isVertical = (rotate) % 2 == 1 ? true : false;
        int rotateLevel = (int)rotate;
        int gridXMin2 = Mathf.FloorToInt((xMin2 - GameInstance.GameIns.calculatorScale.minX) / 2.5f) + ((isVertical == true) ? 2 : (rotateLevel < 2 ? 0 : 1));// + (int)Mathf.Abs(ex.x);
        int gridXMax2 = Mathf.FloorToInt((xMax2 - GameInstance.GameIns.calculatorScale.minX) / 2.5f) - ((isVertical == true) ? 1 : (rotateLevel < 2 ? 1 : 0));
        int gridZMin2 = Mathf.FloorToInt((zMin2 - GameInstance.GameIns.calculatorScale.minY) / 2.5f) + ((isVertical == true) ? (rotateLevel < 2 ? 1 : 0) : 2);// + (int)Mathf.Abs(ex.z);
        int gridZMax2 = Mathf.FloorToInt((zMax2 - GameInstance.GameIns.calculatorScale.minY) / 2.5f) - ((isVertical == true) ? (rotateLevel < 2 ? 0 : 1) : 1);

        bool check = true;
        for (int i = gridXMin2; i <= gridXMax2; i++)
        {
            for (int j = gridZMin2; j <= gridZMax2; j++)
            {
                int index = MoveCalculator.GetIndex(i, j);
             //   GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    gameObject2.transform.position = new Vector3(i * 2.5f + GameInstance.GameIns.calculatorScale.minX, 0, j * 2.5f + GameInstance.GameIns.calculatorScale.minY);
                if (!(index < GameInstance.GameIns.gridManager.trashCanGrids.Length && GameInstance.GameIns.gridManager.trashCanGrids[index] == 0))
                {
                    check = false;
                    break;
                }
            }
        }

        if (check)
        {
            applyBtn.enabled = true;
        }
        else
        {
            applyBtn.enabled = false;

        }
        CanPlace = check;
        return check;
    }
}
