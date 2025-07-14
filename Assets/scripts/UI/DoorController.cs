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
    private Vector2 lastInputPosition;
    bool hasInput;
    Mouse currentMouse;
    Vector3 prevPos;
    Quaternion prevRot;
    bool isDragging;


    GameObject currentWallObject;
  
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
                    if(gameObject != currentWallObject)
                    {
                        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
                        if (currentWallObject != null)
                        {
                            MeshRenderer[] mr = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                            for(int i = 0; i < mr.Length; i++) mr[i].enabled = true;
                           
                        }
                        isDragging = true;
                        GameInstance.GameIns.inputManager.inputDisAble = true;
                        currentWallObject = gameObject;
                        MeshRenderer[] mrs = currentWallObject.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < mrs.Length; i++) mrs[i].enabled = false;
                        Vector3 pos = currentWallObject.transform.position;
                        pos.y = 0;
                        transform.position = pos;
                        rotateOffset.transform.rotation = currentWallObject.transform.rotation;

                        //   currentWallObject.SetActive(false);
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
                GameInstance.GameIns.inputManager.inputDisAble = false;
            }
#endif
      
        }
    }
    public void Apply()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
        Door door = GameInstance.GameIns.restaurantManager.door;
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
        GameInstance.GameIns.restaurantManager.ApplyPlaced(door);
        
        SaveLoadSystem.SaveRestaurantBuildingData();
        gameObject.SetActive(false);


    }

    public void Cancel()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
        if (currentWallObject != null)
        {
            MeshRenderer[] mr = currentWallObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mr.Length; i++) mr[i].enabled = true;
        }
        Door door = GameInstance.GameIns.restaurantManager.door;
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
            if (gameObject != currentWallObject)
            {
                SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.FurnitureClick(), 0.2f);
                if (currentWallObject != null)
                {
                    currentWallObject.GetComponent<MeshRenderer>().enabled = true;
                }
                isDragging = true;
                GameInstance.GameIns.inputManager.inputDisAble = true;
                currentWallObject = gameObject;
                currentWallObject.GetComponent<MeshRenderer>().enabled = false;
                Vector3 pos = currentWallObject.transform.position;
                pos.y = 0;
                transform.position = pos;
                rotateOffset.transform.rotation = currentWallObject.transform.rotation;
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
        GameInstance.GameIns.inputManager.inputDisAble = false;
    }
}
