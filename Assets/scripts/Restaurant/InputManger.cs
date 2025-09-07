using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Threading;
using static GameInstance;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Numerics;
using System.Text;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using static UnityEngine.InputSystem.InputAction;
//using UnityEngine.InputSystem;
//using UnityEditor.Experimental.GraphView;
//using static DG.Tweening.DOTweenModuleUtils;

public class InputManger : MonoBehaviour
{
    public Transform cameraTrans; // 카메라의 Transform
    public Transform cameraRange; // 카메라 이동 범위
    public float maxCameraLocX; // 카메라의 최대 X 위치
    public float maxCameraLocZ; // 카메라의 최대 Z 위치
    public float minCameraLocX; // 카메라의 최소 X 위치
    public float minCameraLocZ; // 카메라의 최소 Z 위치
    public float cameraSpeed; // 카메라 이동 속도
    //float money; // 현재 돈
    AnimalManager animalManager; // 동물 관리
    RestaurantManager restaurantManager; // 레스토랑 관리
    public UIManager UIManager; // UI 관리
    Vector2 cameraLoc = new Vector2(0, 0); // 카메라 위치
    //public float Money { get { return money; } set { money = value; UIManager.UpdateMoneyText(Money); } } // 돈 속성
    private Vector3 nowPos, prePos;
    private Vector3 movePos;
    public GameObject go;

    public GameObject testTrash;

    public bool InputDisAble { get; set; }
    public bool bCleaningMode; //탁자를 치울 수 있는가
                               //   public bool a;
                               //    Vector3 lastPoint = new Vector3();
    public float dragSpeed = 2.0f;
    public Transform centerObject;       // 화면 중앙 기준 오브젝트 (레이를 쏘는 기준)
    public LayerMask navigationLayer;    // 이동 가능한 영역의 레이어

    private Vector3 dragOrigin;          // 드래그 시작 위치의 월드 좌표
    private bool isDragging = false;
    float diff;
    Vector3 lastDir;
    float dragTimer;
    RaycastHit deltaResult;
    Vector3 dir;
    float reduceSpeed;
    Vector3 targetVector;
    RaycastHit hits;
    Vector3 target;
    //   bool entireStart;
    float doubleClickTimer = 0.2f;
    float lastClick = -1f;
    public Vector3 preLoc;
    public Vector3 curLoc;

    [Range(0f, 50f)]
    public float weight;
    [Range(0, 50)]
    public float height;
    public Garbage TestObject;
    public float size;
//    public AudioSource audioSource;
    public bool inOtherAction;
    public ParticleSystem particleSystems;
    float zOffset;
    Vector3 lastMousePosition;
    RaycastHit hit;
    public bool manyFingers;

    public Table clickedTable;
    EventSystem es;

    public Slider slider;
    public static Camera cachingCamera;
   // [SerializeField]
    PlayerInput playerInput;

    Vector2 currentPoint;
    Vector2 prevPoint;
    Vector3 currentPosition;
    Vector3 deltaPosition;
    Vector3 followPosition;
    Vector3 realPosition;
    Vector3 dummyChar;
    bool bClick;
    List<RaycastResult> results = new List<RaycastResult>();
    Vector3 camVelocity;
    PointerEventData pointerEventData;

    UnlockableBuyer buyer;
    int refX = -999;
    int refY = -999;
    bool checkingHirePos;
    public static List<Vector3> spawnDetects = new List<Vector3>();
    Employee draggingEmployee;
    public GameObject draggingFurniture;

    bool justTouch;
    public Vector3 latestDirection;
    int obstacleMask = 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19;
    int objectMask = 1 << 3 | 1 << 11 | 1 << 13 | 1 << 10 | 1 << 22;
    int uiMask = 1 << 5 | 1 << 14 | 1 << 18;
    private void Awake()
    {
        es = GetComponent<EventSystem>();
      
        GameInstance.GameIns.inputManager = this;
    //    ped = new PointerEventData(es);
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Start()
    {
        restaurantManager = GetComponent<RestaurantManager>();
        animalManager = GetComponent<AnimalManager>();
        zOffset = cameraTrans.position.z - Camera.main.transform.position.z;
     //   originSize = Camera.main.orthographicSize;
        cachingCamera = Camera.main;    
    }

    void Update()
    {
      //  Utility.CheckHirable(cameraRange.transform.position, ref refX, ref refY, true, true, false);
    }


    private void OnEnable()
    {
        /*#if UNITY_ANDROID
                EnhancedTouchSupport.Enable();
                InputSystem.EnableDevice(Touchscreen.current);
        #endif*/
        playerInput.actions["PointerPosition"].performed -= ScreenPoint;
        playerInput.actions["PointerPosition"].performed += ScreenPoint;
        playerInput.actions["ClickPosition"].started -= StartClick;
        playerInput.actions["ClickPosition"].started += StartClick;
        playerInput.actions["ClickPosition"].canceled -= EndClick;
        playerInput.actions["ClickPosition"].canceled += EndClick;
      //  playerInput.actions["EscapeTab"].started += (InputAction.CallbackContext callbackContext)=> { Debug.Log("A"); };

    }

    private void OnDisable()
    {
#if UNITY_ANDROID
        EnhancedTouchSupport.Disable();
#endif
        if (playerInput != null)
        {
            playerInput.actions["PointerPosition"].performed -= ScreenPoint;
            playerInput.actions["ClickPosition"].started -= StartClick;
            playerInput.actions["ClickPosition"].canceled -= EndClick;
        }
        cts.Cancel();
    }
    private static CancellationTokenSource cts = new CancellationTokenSource();
    public static CancellationToken cancelToken => cts.Token;
    Coroutine coroutines;
    Coroutine coroutines2;
    void ScreenPoint(InputAction.CallbackContext callbackContext)
    {
   
        Vector2 vector2 = callbackContext.ReadValue<Vector2>();
        
        if (justTouch)
        {
            prevPoint = vector2;
            currentPoint = vector2;
            justTouch = false;
        }
        else
        {
      
            prevPoint = currentPoint;
            currentPoint = vector2;

            if (bClick)
            {
                if (InputDisAble) return;
            
                if (Physics.Raycast(cachingCamera.ScreenPointToRay(currentPoint), out RaycastHit hitInfo, float.MaxValue, 1)) currentPosition = hitInfo.point;
                if (Physics.Raycast(cachingCamera.ScreenPointToRay(prevPoint), out RaycastHit hitInfos, float.MaxValue, 1)) deltaPosition = hitInfos.point;

                if (!isDragging)
                {

                    if (coroutines != null) StopCoroutine(coroutines);
                    coroutines = StartCoroutine(Drag());
                }
                else
                {
                    if (buyer != null)
                    {
                        buyer.MouseUp();
                        buyer = null;
                    }
                }
            }
            else
            {
              //  ((Action)EventManager.Publish(3))?.Invoke();
            }
        }
    }

    void StartClick(InputAction.CallbackContext callbackContext)
    {
    
        camVelocity = Vector3.zero;

        if (cachingCamera == null) cachingCamera = Camera.main;
        if (CheckClickedUI(uiMask)) return;


        followPosition = realPosition;
        prevPoint = currentPoint;
        deltaPosition = currentPosition;
        isDragging = false;
        bClick = true;
        justTouch = true;

        ((Action<Vector3>)EventManager.Publish(2))?.Invoke(currentPoint);
        ((Action)EventManager.Publish(4))?.Invoke();

        if (GameIns.uiManager.tutorialPlaying) return;
        StartClickIngameObject(currentPoint);
    }
    void EndClick(InputAction.CallbackContext callbackContext)
    {
        if (GameIns.uiManager)
        {
            if (GameIns.uiManager.gameGuide)
            {
                GameIns.uiManager.GetComponent<Animator>().SetTrigger("unhighlight");
                GameIns.uiManager.gameGuide = false;
            }
        }
        if (draggingEmployee != null)
        {
            InputDisAble = false;
            draggingEmployee.UnDragged();
            draggingEmployee = null;
            
        }
     //   ((Action)EventManager.Publish(3))?.Invoke();
/*#if UNITY_ANDROID
        // if(Touchscreen.current == null || Touchscreen.current.touches.Count > 0) return;
#endif*/
        bClick = false;
        if (buyer != null)
        {
            buyer.MouseUp();
            buyer = null;
        }

        if (CheckClickedUI(uiMask) || GameIns.store.scrolling.isWorking) return;

        if (GameIns.uiManager.tutorialPlaying) return;

        EndClickIngameObject(currentPoint);

       // ((Action<Vector3>)EventManager.PublishWithDestory(1))?.Invoke(currentPoint);
    }
    void StartClickIngameObject(Vector3 pos)
    {
        Ray ray = cachingCamera.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, objectMask))// 1 << 3 | 1 << 11 | 1 << 13 | 1 << 10 | 1 << 22))
        {
            GameObject go = hit.collider.gameObject;
            if (go.TryGetComponent<UnlockableBuyer>(out UnlockableBuyer unlockableBuyer))
            {
                buyer = unlockableBuyer;
                buyer.MouseDown();

            }
            else if (Utility.TryGetComponentInParent<Animal>(go, out Animal animal))
            {
                if (Utility.TryGetComponentInChildren<Employee>(animal.gameObject, out Employee employee))
                {
                    if (draggingEmployee == null && !employee.falling && !employee.pause)
                    {

                        InputDisAble = true;
                        draggingEmployee = employee;
                        draggingEmployee.Dragged();
                        draggingEmployee.pause = true;
                    }
                    //GameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(employee);
                }
                else if(go.TryGetComponent(out Fish fish))
                {
                    if(fish.bFloating)
                    {
                        fish.Caught();
                    }
                }
            }
            else if(go.TryGetComponent(out BlackConsumer blackConsumer))
            {
                blackConsumer.Caught();
            }
            else if (go.TryGetComponent<Table>(out Table table))
            {
                if (table.isDirty && !table.interacting && !RestaurantManager.tutorialEventKeys.Contains(TutorialEventKey.NoCleaning))
                {
                    clickedTable = table;
                    clickedTable.canTouchable = false;
                    clickedTable.interacting = true;
                    clickedTable.CleanTableManually();
                    //   targetVector = dragTouch.position;
                }
            }
        }
    }
    //인게임 오브젝트 상호작용
    void EndClickIngameObject(Vector3 pos)
    {
        Ray ray = cachingCamera.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 13))
        {
            GameObject go = hit.collider.gameObject;
            if (Utility.TryGetComponentInParent<Furniture>(go, out Furniture furniture))
            {
                if (furniture.canTouchable)
                {
                    GameInstance.GameIns.applianceUIManager.ShowApplianceInfo(furniture);
                }
            }
        }
    }

    IEnumerator Drag()
    {
        //    try
        {
            isDragging = true;
            followPosition = cameraTrans.position;
            realPosition = followPosition;
            float currentSmoothTime = 0.2f;
            float distanceFactor = 0f;
            Vector3 test = Vector3.zero;
            while (isDragging)
            {
                if (App.restaurantTimeScale == 1)
                {
                    Vector3 l = deltaPosition - currentPosition;
                    float m = l.magnitude;
                    Vector3 n = l.normalized;

                    if (test == Vector3.zero && !bClick)
                    {
                        Vector3 dir = (realPosition - followPosition).normalized;
                        float moveDistance = (realPosition - followPosition).magnitude;
                        test = cameraTrans.position + dir * (moveDistance * 2); //cameraTrans.position + (realPosition - followPosition).magnitude * 2 * (realPosition - followPosition).normalized;
                        float remainingDistance = Vector3.Distance(cameraTrans.position, test);
                        distanceFactor = Mathf.Clamp01(remainingDistance / 25f);

                    }
                    // Debug.Log(realPosition);
                    if (bClick)
                    {
                        realPosition += n * m;

                        Vector3 move = Vector3.SmoothDamp(followPosition, realPosition, ref camVelocity, 0.2f);
                        //Vector3 lerps = Vector3.Lerp(followPosition, realPosition, 0.2f);

                        //   Debug.DrawLine(cameraRange.position, cameraRange.position + n * camVelocity.magnitude, Color.red, 5f);

                        Vector3 d = move - cameraTrans.position;
                        if (CheckRayMove(d))
                        {
                            if (!InputDisAble) cameraTrans.position = new Vector3(move.x, 0, move.z);
                            //        Utility.CheckHirable(cameraRange.position, ref refX, ref refY);

                        }

                    }
                    else
                    {

                        // targetSmoothTime = Mathf.Lerp(0.2f, 1f, distanceFactor);
                        currentSmoothTime = Mathf.Lerp(0.2f, distanceFactor, 0.05f);
                        Vector3 move = Vector3.SmoothDamp(followPosition, test, ref camVelocity, currentSmoothTime);
                        Vector3 d = move - cameraTrans.position;
                        if (CheckRayMove(d))
                        {
                            if (!InputDisAble) cameraTrans.position = new Vector3(move.x, 0, move.z);
                            //  Utility.CheckHirable(cameraRange.position, ref refX, ref refY);
                        }

                    }
                    followPosition = cameraTrans.position;

                    deltaPosition = currentPosition;
                    if (camVelocity.magnitude < 0.01f) break;
                }
                // await UniTask.NextFrame(cancellationToken: cancellationToken);
                yield return null;
            }
            //     camVelocity = Vector3.zero; 
            isDragging = false;
        }
    }

    IEnumerator Blocking(Vector3 move)
    {
        float i = 0;
        isDragging = false;
        while (i < 0.5f)
        {
            if (App.restaurantTimeScale == 1)
            {
                if (!InputDisAble) cameraTrans.position -= move * Time.deltaTime;
             
                //   Utility.CheckHirable(cameraRange.position, ref refX, ref refY, checkingHirePos);
                i += Time.deltaTime;
            }
            yield return null;
        }
    }
    bool CheckRayMove(Vector3 move)
    {
        double d = 0;
        while (d <= 1)
        {
            Vector3 lerp = Vector3.Lerp(cameraRange.position, (cameraRange.position + move * 1.05f), (float)d);

            Vector3 origin = cameraRange.position;
            Vector3 target = lerp;
            Vector3 direction = (target - origin).normalized;
            float distance = Vector3.Distance(origin, target);

            //  Debug.DrawRay(origin, direction * distance, Color.red, 1f);
            //  bool hitBlock = Physics.Raycast(origin, direction, out hit, distance, 1 << 7 | 1 << 8);
            bool hitBlock = Physics.CheckSphere(origin + direction * distance, 0.2f, obstacleMask); //1 << 7 | 1 << 8 | 1 << 16 | 1 << 19);
            if (hitBlock)
            {
                followPosition = cameraTrans.position;
                realPosition = followPosition;
                direction.y = 0;
                if(direction != Vector3.zero) latestDirection = direction;
                Stuck(direction);
                return false;
            }
            d += 0.05f;
        }
        return true;
    }
    public void Stuck(Vector3 dir)
    {
        if (coroutines2 != null) StopCoroutine(coroutines2);
        coroutines2 = StartCoroutine(Blocking(dir));
    }

    public bool CheckHire()
    {
        checkingHirePos = Utility.CheckHirable(cameraRange.position, ref refX, ref refY, checkingHirePos);
        return checkingHirePos;
    }
    public bool CheckClickedUI(int layer)
    {
        results.Clear();
        if (pointerEventData == null) pointerEventData = new PointerEventData(EventSystem.current);
        for (int i = 0; i < GameInstance.graphicRaycasters.Count; i++)
        {
            pointerEventData.position = currentPoint;
            GameInstance.graphicRaycasters[i].Raycast(pointerEventData, results);

            if(results.Count > 0)
            {
                int l = layer >> results[0].gameObject.layer;
                l &= 1;
                if (l == 1) return true;
                else return false;
              
            }

        }
        return false;
    }
 
    public Vector3 lastLoc;

    public void FollowTarget(Animal animal)
    {
        InputDisAble = true;
    }

    IEnumerator FollowingTarget(Animal animal)
    {
        Vector3 temp = cameraTrans.position;
        Vector3 t;
        //타겟으로 이동
        float f = 0;
        while(true)
        {
            t = Vector3.Lerp(cameraTrans.position, animal.trans.position, f);

            f += Time.unscaledDeltaTime / 2f;

            if(f >= 1)
            {
                cameraTrans.position = temp;
                break;
            }
            yield return null;
        }





        //원래 위치로 이동

        InputDisAble = false;
    }
}
