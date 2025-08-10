using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
public class PlaceController : MonoBehaviour
{
    public GameObject model;
    public GameObject offset;
    public Image applyImage;
    public Image applyborderImage;
    public GraphicRaycaster raycaster;
    

    [NonSerialized] public int[] rotates = new int[4] { 0, 270, 180, 90 };
    public Vector3[] rotateOffsets = new Vector3[4];
    int level = 0;

    bool place;
    public Vector2 offsetVector = new Vector2();
    static readonly Color gray = new Color32(128, 128, 128, 255);
    static readonly Color normal = new Color32(255, 255, 255, 255);
    bool isDragging;
    public bool purchasedObject;
    public Furniture currentFurniture { get; set; }

    public List<Vector2> temp = new List<Vector2>();
    public List<Vector2> tempTable = new List<Vector2>();
    public List<Vector2> tempTableCenter = new List<Vector2>();
    public List<Vector2> temptrashcan = new List<Vector2>();
    private Vector2 lastInputPosition;
    bool hasInput;
    [NonSerialized] public bool spawnAnimation;
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

    Mouse currentMouse;
    public Queue<int> placedArea = new Queue<int>();

    Vector3 prevPos;
    Quaternion prevRot;
    private void Start()
    {
        applyImage.gameObject.layer = 5;
        applyborderImage.gameObject.layer = 5;
    }
    private void OnEnable()
    {
       /* if (spawnAnimation)
        {
            spawnAnimation = false;
            StartCoroutine(ScaleAnimation());
        }*/
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

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (isDragging)
                    {
                        HandleDrag(lastInputPosition);
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    HandleInputUp();
                    break;
            }
        
#else
            if (currentMouse.leftButton.isPressed && isDragging)
            {
                Ray r = InputManger.cachingCamera.ScreenPointToRay(currentMouse.position.ReadValue());
                if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
                {
                    GameInstance.GameIns.gridManager.SelectLine(hit.point, this, canPlace, storeGoods.goods.type);
                }
            }
            if (currentMouse.leftButton.wasPressedThisFrame)
            {
                Ray ray = InputManger.cachingCamera.ScreenPointToRay(currentMouse.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 17))
                {
                    if (hit.collider.gameObject.GetComponentInParent<PlaceController>() == this)
                    {
                        GameInstance.GameIns.inputManager.InputDisAble = true;
                        isDragging = true;
                    }
                }
            }

            if (currentMouse.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                GameInstance.GameIns.inputManager.InputDisAble = false;
            }
#endif
            /*if (Input.GetMouseButton(0) && isDragging)
            {
                Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
                {
                    //    currnet.transform.position = hit.point;
                    GameInstance.GameIns.gridManager.SelectLine(hit.point, this, canPlace);
                }
            }

            if (Input.GetMouseButtonDown(0))
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
            }*/

        }
    }
    public void Rotate()
    {
        if(level < 3) level++;
        else level = 0;
      
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
        offset.transform.rotation = Quaternion.Euler(0, rotates[level], 0);
        offset.transform.localPosition = rotateOffsets[level];
        GameInstance.GameIns.gridManager.CheckObject(this, storeGoods.goods.type);
    }

    public void Apply()
    {
        if (canPlace)
        {
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);

            GameInstance.GameIns.gridManager.ApplyGird(this, offset.transform.position, storeGoods.goods.type);
            if (purchasedObject)
            {
                GameInstance.GameIns.gridManager.RemoveCell();
                MoveCalculator.ApplyCheckAreaWithBounds(placedArea, false);
                placedArea.Clear();
            }
            GameInstance.GameIns.gridManager.RemoveSelect();

            storeGoods.PlaceGoods(rotateOffsets[level], level, currentFurniture);
            if (!purchasedObject)
            {
                storeGoods.Purchase();
#if UNITY_ANDROID || UNITY_IOS
                if(App.gameSettings.hapticFeedback) Handheld.Vibrate();
#endif
            }
            storeGoods.RemoveGoodsPreview();

            GameInstance.GameIns.gridManager.VisibleGrid(false);
        }
    }

    public void Cancel()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIClick(), 0.2f);
        if (purchasedObject)
        {
            GameInstance.GameIns.gridManager.Revert(this);
            placedArea.Clear();
        }
        GameInstance.GameIns.gridManager.RemoveCell();
        GameInstance.GameIns.gridManager.RemoveSelect();
        storeGoods.RemoveGoodsPreview();
        GameInstance.GameIns.gridManager.VisibleGrid(false); 
    }

    public Vector3 GetLocation()
    {
        Vector3 returnVector = Vector3.zero;

        return returnVector;
    }

    public void SetLevel(int level)
    {
        this.level = level;
        offset.transform.rotation = Quaternion.Euler(0, rotates[level], 0);
        offset.transform.localPosition = rotateOffsets[level];
    }
    

    private void HandleInputDown(Vector2 inputPosition)
    {
        Ray ray = InputManger.cachingCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 17))
        {
            if (hit.collider.gameObject.GetComponentInParent<PlaceController>() == this)
            {
                GameInstance.GameIns.inputManager.InputDisAble = true;
                isDragging = true;
            }
        }
    }

    private void HandleDrag(Vector2 inputPosition)
    {
        Ray r = InputManger.cachingCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1))
        {
            GameInstance.GameIns.gridManager.SelectLine(hit.point, this, canPlace, storeGoods.goods.type);
        }
    }

    private void HandleInputUp()
    {
        isDragging = false;
        GameInstance.GameIns.inputManager.InputDisAble = false;
    }

    IEnumerator ScaleAnimation()
    {
        Vector3 origin = model.transform.localScale;
        float f = 0;
        Vector3 start = origin * 0.5f;
        Vector3 target1 = origin * 1.2f;
        Vector3 target2 = origin;
;       while (f <= 0.25f)
        {
            Vector3 targetPos = Vector3.Lerp(start, target1, f / 0.25f);
            model.transform.localScale = targetPos;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        model.transform.localScale = target1;
        f = 0;
        while (f <= 0.1f)
        {
            Vector3 targetPos = Vector3.Lerp(target1, target2, f / 0.1f);
            model.transform.localScale = targetPos;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        model.transform.localScale = target2;
    }
}
