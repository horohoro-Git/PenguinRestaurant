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

    public bool inputDisAble { get; set; }
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
    public AudioSource audioSource;
    public bool inOtherAction;
    public ParticleSystem particleSystems;
    float zOffset;
    Vector3 lastMousePosition;
    RaycastHit hit;
    public bool manyFingers;

    public Table clickedTable;
    EventSystem es;

    PointerEventData ped;
    //  bool dragging=false;
    //private Vector3 dragOrigin;
    public Slider slider;
    public static Camera cachingCamera;
    [SerializeField]
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
    int refX;
    int refY;
    bool checkingHirePos;
    public static List<Vector3> spawnDetects = new List<Vector3>();
    Employee draggingEmployee;
    public GameObject draggingFurniture;
    private void Awake()
    {
        es = GetComponent<EventSystem>();
        Application.targetFrameRate = 60;//(int)Screen.currentResolution.refreshRateRatio.numerator;
      //  GameInstance.GameIns.uiManager = UIManager;
        GameInstance.GameIns.inputManager = this;
        ped = new PointerEventData(es);
       
    }
   
    // Start는 첫 프레임 전에 호출됩니다.
    void Start()
    {
      
        //Money = 100000; // 시작할 때 돈을 10000으로 설정
        restaurantManager = GetComponent<RestaurantManager>(); // RestaurantManager 컴포넌트 가져오기
        animalManager = GetComponent<AnimalManager>(); // AnimalManager 컴포넌트 가져오기
     //   animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 직원 동물 생성
        //animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
        zOffset = cameraTrans.position.z - Camera.main.transform.position.z;


        originSize = Camera.main.orthographicSize;
    }

    void Update()
    {
       // Utility.CheckHirable(cameraRange.position, ref refX, ref refY, true, true);
        if (Input.GetKey(KeyCode.O))
        {
           // for (int i = 0; i < 1; i++)
            {
            //    SaveLoadTest.WriteData();
              //  SaveLoadTest.ReadData();
            }
            cachingCamera.orthographicSize -= 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.P))
        {
            cachingCamera.orthographicSize += 5f * Time.deltaTime;
          //  SaveLoadTest.ReadData();
            // Camera.main.orthographicSize += 5f * Time.deltaTime;
        }

        if(!isDragging && bClick)
        {
            if(buyer == null)
            {
                StartClickIngameObject(currentPoint);
            }
        }


    }


    private void OnEnable()
    {
        playerInput.actions["PointerPosition"].performed -= ScreenPoint;
        playerInput.actions["PointerPosition"].performed += ScreenPoint;
        playerInput.actions["ClickPosition"].started -= StartClick;
        playerInput.actions["ClickPosition"].started += StartClick;
        playerInput.actions["ClickPosition"].canceled -= EndClick;
        playerInput.actions["ClickPosition"].canceled += EndClick;

    }

    private void OnDisable()
    {
        playerInput.actions["PointerPosition"].performed -= ScreenPoint;
        playerInput.actions["ClickPosition"].started -= StartClick;
        playerInput.actions["ClickPosition"].canceled -= EndClick;
        cts.Cancel();
    }
    private static CancellationTokenSource cts = new CancellationTokenSource();
    public static CancellationToken cancelToken => cts.Token;
    Coroutine coroutines;
    Coroutine coroutines2;
 //   CancellationToken cancellation = new CancellationToken();
    void ScreenPoint(InputAction.CallbackContext callbackContext)
    {
      

        Vector2 vector2 = callbackContext.ReadValue<Vector2>();
        prevPoint = currentPoint;
        currentPoint = vector2;

#if UNITY_ANDROID
        if(Touchscreen.current.touches.Count >= 2)
        {
            if(Touchscreen.current.touches.Count == 2)
            {

            }
            return;
        }
#endif
        if (bClick)
        {
            if (inputDisAble) return;
            if (!Utility.IsInsideCameraViewport(currentPoint, cachingCamera))
            {
                return;
            }
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
            ((Action)EventManager.Publish(3))?.Invoke();
        }

    }

    void StartClick(InputAction.CallbackContext callbackContext)
    {
        camVelocity = Vector3.zero;
        if (cachingCamera == null) cachingCamera = Camera.main;
        if (CheckClickedUI(1 << 5 | 1 << 14 | 1 << 18)) return;
        if (Utility.IsInsideCameraViewport(currentPoint, cachingCamera))
        {
           
            isDragging = false;
            bClick = true;
            

            ((Action<Vector3>)EventManager.Publish(2))?.Invoke(currentPoint);
            ((Action)EventManager.Publish(4))?.Invoke();
            StartClickIngameObject(currentPoint);
        }
    }
    void EndClick(InputAction.CallbackContext callbackContext)
    {
        if (draggingEmployee != null)
        {
            inputDisAble = false;
            draggingEmployee.UnDragged();
            draggingEmployee = null;
        }
        ((Action)EventManager.Publish(3))?.Invoke();
#if UNITY_ANDROID
         if(Touchscreen.current.touches.Count > 0) return;
#endif
        bClick = false;
        if (buyer != null)
        {
            buyer.MouseUp();
            buyer = null;
        }
         
        if (CheckClickedUI(1 << 5 | 1 << 14 | 1 << 18)) return;
        if (!isDragging)
        {
            //인게임 오브젝트 클릭
            EndClickIngameObject(currentPoint);

            ((Action<Vector3>)EventManager.PublishWithDestory(1))?.Invoke(currentPoint);
           
      
        }
    }
    void StartClickIngameObject(Vector3 pos)
    {
        Ray ray = cachingCamera.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3 | 1 << 11 | 1 << 10))
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
                    if(draggingEmployee == null && !employee.falling && !employee.pause)
                    {
                        
                        inputDisAble = true;
                        draggingEmployee = employee;
                        draggingEmployee.Dragged();
                        draggingEmployee.pause = true;
                    }
                    //GameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(employee);
                }
            }
            else if (go.TryGetComponent<Table>(out Table table))
            {
                if (table.isDirty && !table.interacting)
                {
                    clickedTable = table;
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

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<13))
        {
            GameObject go = hit.collider.gameObject;
            if(Utility.TryGetComponentInParent<Furniture>(go, out Furniture furniture))
            {
                GameInstance.GameIns.applianceUIManager.ShowApplianceInfo(furniture);
            }
        }
    }

  //  async UniTask
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
                //Debug.Log(realPosition + " real");
                //Debug.Log(followPosition + " follow");
                Vector3 l = deltaPosition - currentPosition;
                float m = l.magnitude;
                Vector3 n = l.normalized;

                if (test == Vector3.zero && !bClick)
                {
                    Vector3 dir = (realPosition - followPosition).normalized;
                    float moveDistance = (realPosition - followPosition).magnitude;
                    test = cameraTrans.position + dir *( moveDistance * 2); //cameraTrans.position + (realPosition - followPosition).magnitude * 2 * (realPosition - followPosition).normalized;
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

                    Vector3 d =  move - cameraTrans.position;
                    if (CheckRayMove(d))
                    {
                        if (!inputDisAble) cameraTrans.position = new Vector3(move.x,0,move.z);
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
                        if (!inputDisAble) cameraTrans.position = new Vector3(move.x, 0, move.z);
                      //  Utility.CheckHirable(cameraRange.position, ref refX, ref refY);
                    }
                   
                }
                followPosition = cameraTrans.position;

                deltaPosition = currentPosition;
                if (camVelocity.magnitude < 0.01f) break;
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
        while(i < 0.5f)
        {
            if(!inputDisAble) cameraTrans.position -= move * Time.deltaTime;
         //   Utility.CheckHirable(cameraRange.position, ref refX, ref refY, checkingHirePos);
            i += Time.deltaTime;
            yield return null;
        }
    }
    bool CheckRayMove(Vector3 move)
    {
        double d = 0;
        while (d <= 1)
        {
            Vector3 lerp = Vector3.Lerp(cameraRange.position , (cameraRange.position + move * 1.05f), (float)d);

            Vector3 origin = cameraRange.position;
            Vector3 target = lerp;
            Vector3 direction = (target - origin).normalized;
            float distance = Vector3.Distance(origin, target);
   
          //  Debug.DrawRay(origin, direction * distance, Color.red, 1f);
          //  bool hitBlock = Physics.Raycast(origin, direction, out hit, distance, 1 << 7 | 1 << 8);
            bool hitBlock = Physics.CheckSphere(origin + direction * distance, 0.2f, 1 << 7 | 1 << 8 | 1 << 16);
            if (hitBlock)
            {
                followPosition = cameraTrans.position;
                realPosition = followPosition;
                direction.y = 0;
                if(coroutines2 != null) StopCoroutine(coroutines2);
                coroutines2 = StartCoroutine(Blocking(direction));

                return false;
            }
            d += 0.05f;
        }
        return true;
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

            for(int j = 0; j < results.Count; j++)
            {
                int l = layer >> results[j].gameObject.layer;
                l &= 1;
                if(results[j].gameObject.layer != 0 && l == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
           
        }
        return false;
    }

   /* public static void CheckHirable(Vector3 center, ref int x, ref int y, bool forcedCheck = false)
    {
        Vector3 loc = center;

        int cameraPosX = (int)((loc.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int cameraPosZ = (int)((loc.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

        if(x == cameraPosX && y == cameraPosZ && !forcedCheck) return;
        x = cameraPosX; y = cameraPosZ;

        spawnDetects.Clear();


        float tileSize = GameIns.calculatorScale.distanceSize;
  
        float radiusInGrid = (cachingCamera.orthographicSize / 2.5f) / tileSize;
        float radiusSqr = radiusInGrid * radiusInGrid;

        int minX = Mathf.FloorToInt(cameraPosX - radiusInGrid);
        int maxX = Mathf.CeilToInt(cameraPosX + radiusInGrid);
        int minY = Mathf.FloorToInt(cameraPosZ - radiusInGrid);
        int maxY = Mathf.CeilToInt(cameraPosZ + radiusInGrid);

        for (int xx = minX; xx <= maxX; xx++)
        {
            for (int yy = minY; yy <= maxY; yy++)
            {   
                float dx = xx - cameraPosX;
                float dy = yy - cameraPosZ;

                if (dx * dx + dy * dy <= radiusSqr)
                {
                    if (!Utility.ValidCheck(yy, xx)) continue;

                    bool isBlocked = MoveCalculator.GetBlocks[yy, xx];

                    float worldX = GameIns.calculatorScale.minX + xx * tileSize;
                    float worldZ = GameIns.calculatorScale.minY + yy * tileSize;
                    Vector3 worldPos = new Vector3(worldX, 0, worldZ);

                    Color debugColor = isBlocked ? Color.red : Color.green;
                    Debug.DrawRay(worldPos, Vector3.up * 0.5f, debugColor, 0.1f);
                    if(!isBlocked) spawnDetects.Add(worldPos);
                }
            }
        }

        if(spawnDetects.Count > 0) GameIns.applianceUIManager.UnlockHire(true);
        else GameIns.applianceUIManager.UnlockHire(false);
    }*/


    /*bool RayMove(Vector3 direction, float speed)
    {
        double d = 0;
        float currentSpeed = 0;
        bool check = false;
        Vector3 hitpoibt = Vector3.zero;
        while (d <= 1)
        {
            float lerp = Mathf.Lerp(0, speed, (float)d);
          
            RaycastHit h1;
            bool checkingBlock = Physics.Raycast(cameraRange.position, direction, out h1, lerp, 1 << 7 | 1 << 8);
            if (!checkingBlock)
            {
                currentSpeed = lerp;
            }
            else
            {
                hitpoibt = h1.point;
                check = true;
                break;
            }
            d += 0.1;
        }
        if (check == true)
        {
            Vector3 pos = cameraRange.position;
            Vector3 addPos = pos + direction * currentSpeed;
            Vector3 orginPos = pos + direction * speed * Time.deltaTime;
            if ((pos - hitpoibt).magnitude > (pos - orginPos).magnitude)
            {
                cameraTrans.position += direction * speed * Time.deltaTime;
            }
            else
            {
                cameraTrans.position += direction * currentSpeed * Time.deltaTime;
                return false;
            }
        }
        else
        {
            cameraTrans.position += direction * speed * Time.deltaTime;
        }
        
        return true;
    }*/
    bool RayMove(Vector3 direction, float speed)
    {
        float d = 0f;
        float currentSpeed = 0f;
        Vector3 hitPoint = Vector3.zero;

        while (d <= 1f)
        {
            float lerp = Mathf.Lerp(0, speed, d);

            RaycastHit hit;
            bool hitBlock = Physics.Raycast(cameraRange.position, direction, out hit, lerp, 1 << 7 | 1 << 8);
            Debug.DrawLine(cameraRange.position, cameraRange.position + direction * lerp, Color.red, 1);
            if (!hitBlock)
            {
                currentSpeed = lerp;
            }
            else
            {
                hitPoint = hit.point;
                break;
            }

            d += 0.1f;
        }

        //현재 프레임과 이전 프레임 지점의 차이로 거리를 계산
        // 레이캐스팅으로 얻은 값을 기준으로 이동
        if (hitPoint != Vector3.zero)
        {
            //최대 거리 이동 지점이 벽에 막혀 있을 때
            Vector3 pos = cameraRange.position;
            Vector3 nextPosition = pos + direction * currentSpeed;
            Vector3 predictedPosition = pos + direction * speed * Time.deltaTime;

            if ((pos - hitPoint).magnitude > (pos - predictedPosition).magnitude)
            {
                cameraTrans.position += direction * speed * Time.deltaTime;
            }
            else
            {
                cameraTrans.position += direction * currentSpeed * Time.deltaTime;
                return false;
            }
        }
        else
        {
            //벽에 막히지 않음
            cameraTrans.position += direction * speed * Time.deltaTime;
        }
        return true;
    }
    float originSize;
    Vector3 deltaMouse;
    Vector3 cameraSpeeds;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    public void DragScreen_WindowEditor(bool draged = false)
    {
        if (Input.GetMouseButtonDown(0) && !inOtherAction || draged)
        {
            if (Utility.IsInsideCameraViewport(Input.mousePosition, cachingCamera))
            {
                GraphicRaycaster ggr = GameInstance.GameIns.uiManager.graphicRaycaster; //GetComponent<GraphicRaycaster>();
                ped.position = Input.mousePosition;
                raycastResults.Clear();
                ggr.Raycast(ped, raycastResults);
                bool chck = false;

                for (int i = 0; i < raycastResults.Count; i++)
                {
                    if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                    {
                        chck = true;
                        break;
                    }
                }

                if (!chck)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) || draged)
                    {
                        dragOrigin = hit.point;
                        isDragging = true;
                        deltaResult = hit;
                    }
                    if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 11))
                    {

                        if (h.collider.TryGetComponent<Table>(out Table getTable))
                        {
                            if (getTable.isDirty && !getTable.interacting)
                            {
                                clickedTable = getTable;
                                clickedTable.interacting = true;
                                clickedTable.CleanTableManually();
                                targetVector = Input.mousePosition;
                            }
                        }
                    }
                }
            }
        }

        if (isDragging)
        {
            if (Utility.IsInsideCameraViewport(Input.mousePosition, cachingCamera))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 currentPoint = hit.point;
                    Vector3 moveDirection = dragOrigin - currentPoint;
                    dir = GameInstance.GetVector3(moveDirection.x, 0, moveDirection.z);

                    if (RayMove(dir, cameraSpeed))
                    {
                        dragTimer = (deltaResult.point - hit.point).magnitude;

                        diff = cameraSpeed / 2;
                        dragTimer = 1f < dragTimer ? 1f : dragTimer;
                        dragTimer = 0.5f > dragTimer ? 0.5f : dragTimer;
                        reduceSpeed = (cameraSpeed / 2) / dragTimer;

                        float dis = dir.magnitude;
                        if (dis <= 0.1f)
                        {
                            dragOrigin = hit.point;
                        }
                    }
                    else
                    {
                        dragOrigin = hit.point;
                    }
                    deltaResult = hit;
                }
            }
        }
        else
        {

            if (dragTimer > 0)
            {
                dragTimer -= Time.deltaTime;
                Vector3 currentPoint = hit.point;
                if (RayMove(dir, diff) == false) dragTimer = 0;
                diff -= Time.deltaTime * reduceSpeed;
            }
        }

        if (Input.GetMouseButtonUp(0) && !inOtherAction)
        {
           // if (Utility.IsInsideCameraViewport(Input.mousePosition, Camera.main))
            {
                isDragging = false;
                /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                 if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                 {
                     dragOrigin = hit.point;
                     deltaResult = hit;
                 }*/
                deltaMouse = Input.mousePosition;
            }
        }
    }
    Vector3 tempCameraLoc;
    public IEnumerator BecomeToOrgin()
    {
        float retunSize =  cachingCamera.orthographicSize;

        double r = cachingCamera.orthographicSize;
        float currentSize = (float)r;
        double t = 0;

        Ray ray2 = cachingCamera.ScreenPointToRay(targetVector);
        if (Physics.Raycast(ray2, out RaycastHit hitres2, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hitres2.point;
        }
            //    StartCoroutine(MoveCamera());//cameraTrans.DOMove(tempCameraLoc, 5f);
        while (t < 1)
        {
            t += 5 *Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, originSize, (float)t);
            cachingCamera.orthographicSize = currentSize;
            Vector3 newLoc;
             Ray ray = cachingCamera.ScreenPointToRay(targetVector);
            if (Physics.Raycast(ray, out RaycastHit hitres, Mathf.Infinity, LayerMask.GetMask("Default")))
            { 
                newLoc = hitres.point;
                float s = (target - newLoc).magnitude;
                Vector3 dir = (target - newLoc).normalized;
                if (RayMove(dir, s / Time.deltaTime) == false)
                {
                    cachingCamera.orthographicSize = retunSize;
                    //         cameraTrans.position = cV;
                    yield break;
                }

                retunSize = currentSize;
            }
            yield return null;
        }
    }

    IEnumerator MoveCamera()
    {
        bool t = true;
#if HAS_DOTWEEN
        cameraTrans.DOMove(tempCameraLoc, 0.5f).SetEase(Ease.InBack).OnComplete(() => { t = false; });
#endif
        while (t)
        {
            yield return null;
        }
    }

    IEnumerator ViewEntireScreen()
    {
        float retunSize = cachingCamera.orthographicSize;

        if(Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
        if(Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
        Ray ray = cachingCamera.ScreenPointToRay(targetVector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hit.point;
        }
        double r = cachingCamera.orthographicSize;
        float currentSize = (float)r;
        double t = 0;
        while (t < 1)
        {
          //      retunSize = currentSize;
            t += 1 * Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, 50, (float)t);

            //     Camera.main.WorldToScreenPoint(targetVector);

            cachingCamera.orthographicSize = currentSize;
            Vector3 cV = cameraTrans.position;
            Ray ray2 = cachingCamera.ScreenPointToRay(targetVector);

            if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                Vector3 dir = target - hit2.point;
                float s = dir.magnitude;
                Vector3 d = GameInstance.GetVector3(dir.x, 0, dir.z).normalized; //new Vector3(dir.x, 0, dir.z).normalized;
                //  cameraTrans.Translate(d * Time.deltaTime * 50);
               if (RayMove(d, s/ Time.deltaTime) == false)
                {
                    tempCameraLoc = cameraTrans.position;
                    cachingCamera.orthographicSize = retunSize;
           //         cameraTrans.position = cV;
                    yield break;
                }
               else
                {
                    tempCameraLoc = cameraTrans.position;
                }
                //      target = hit2.point;
                //Camera.main.orthographicSize = retunSize;
                //  hit = hit2; 
                //     cameraTrans.position = new Vector3(hit2.point.x,0, hit2.point.z);
                retunSize = currentSize;
            }
          //  yield break;
            yield return null;
        }

    }

    Vector3 lastVector;
   // Touch dragTouch;
  /*  public void DragScreen_Android(bool draged = false)
    {
        if (Input.touchCount > 0 && inOtherAction == false || draged)
        {
            if (Input.touchCount == 1)
            {
                dragTouch = Input.GetTouch(0);
                if (dragTouch.phase == UnityEngine.TouchPhase.Began || manyFingers)
                {
                    GraphicRaycaster ggr = GameInstance.GameIns.uiManager.graphicRaycaster;
                    ped.position = dragTouch.position;
                    raycastResults.Clear();
                    // List<RaycastResult> raycastResults = new List<RaycastResult>();
                    ggr.Raycast(ped, raycastResults);
                    bool chck = false;
                    for (int i = 0; i < raycastResults.Count; i++)
                    {
                        if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                        {
                            chck = true;
                            break;
                        }
                    }

                    if (!chck)
                    {
                        manyFingers = false;
                        Ray ray = cachingCamera.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) || draged)
                        {
                            dragOrigin = hit.point;
                            isDragging = true;
                            deltaResult = hit;
                        }

                        if (dragTouch.phase == UnityEngine.TouchPhase.Began)
                        {
                            if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 11))
                            {
                                if (h.collider.TryGetComponent<Table>(out Table getTable))
                                {
                                    if (getTable.isDirty && !getTable.interacting)
                                    {
                                        clickedTable = getTable;
                                        clickedTable.interacting = true;
                                        // GameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(true);
                                        //        originSize = Camera.main.orthographicSize;
                                        clickedTable.CleanTableManually();
                                        targetVector = dragTouch.position;
                                        //  entireStart = true;
                                        //  StopAllCoroutines();
                                        //  StartCoroutine(ViewEntireScreen());
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                  *//*  if (clickedTable != null)
                    {
                        lastVector = dragTouch.position;
                        Ray ray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 5))
                        {
                            clickedTable.trashPlate.transform.position = h.point;
                        }
                    }*//*
                //    else
                    {


                        Ray dragray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(dragray, out hit, Mathf.Infinity))
                        {
                            Vector3 currentPoint = hit.point;
                            Vector3 moveDirection = dragOrigin - currentPoint;

                            dir = GameInstance.GetVector3(moveDirection.x, 0, moveDirection.z); //new Vector3(moveDirection.x, 0, moveDirection.z);

                            if (RayMove(dir, cameraSpeed))
                            {
                                dragTimer = (deltaResult.point - hit.point).magnitude;

                                diff = cameraSpeed / 2;
                                dragTimer = 1 < dragTimer ? 1 : dragTimer;
                                dragTimer = 0.5f > dragTimer ? 0.5f : dragTimer;
                                reduceSpeed = (cameraSpeed / 2) / dragTimer;
                            }
                            else
                            {
                                manyFingers = true;
                            }
                            deltaResult = hit;
                        }
                    }
                }

            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // 이전 프레임에서 각 터치의 위치 차이 계산
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // 이전 및 현재 터치 간의 거리 계산
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // 두 거리의 차이로 줌 비율 계산
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                cachingCamera.orthographicSize += deltaMagnitudeDiff * Time.deltaTime;

                if (cachingCamera.orthographicSize < 8) cachingCamera.orthographicSize = 8;
                else if (cachingCamera.orthographicSize > 50) cachingCamera.orthographicSize = 50;
            }

            if (Input.touchCount > 1) manyFingers = true;
        }
        else
        {
           *//* if (clickedTable != null && inOtherAction == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(lastVector);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 11))
                {
                    if (h.collider.GetComponentInParent<TrashCan>() != null)
                    {
                        clickedTable.trashPlate.transform.position = h.point;
                        //   targetVector = Input.mousePosition;
                        //GameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.CleanTableManually(h.point);
                        //   if (Physics.Raycast(ray, out RaycastHit hh, Mathf.Infinity, 1 << 5)) clickedTable.CleanTableManually(hh.point);
                    }
                    else
                    {
                       // GameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.trashPlate.transforms.position = clickedTable.plateLoc.position;
                    }

                }
                else
                {
                   // GameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                    clickedTable.trashPlate.transforms.position = clickedTable.plateLoc.position;
                }
                clickedTable.interacting = false;
                clickedTable = null;
            }*//*

            if (dragTimer > 0)
            {
                dragTimer -= Time.deltaTime;
                Vector3 currentPoint = hit.point;
                if (RayMove(dir, diff) == false) dragTimer = 0;
                diff -= Time.deltaTime * reduceSpeed;
                if(diff < 0) diff = 0;
            }
        }
    }*/


    void DoubleClick()
    {
       // Input.touchCount == 1
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            if(Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase != UnityEngine.TouchPhase.Began) return;
            }
            if (Time.time < lastClick + doubleClickTimer)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) targetVector = Input.mousePosition;
                if (Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
                bool check = false;
            
                Ray ray = Camera.main.ScreenPointToRay(targetVector);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    if (h.collider.TryGetComponent<Table>(out Table getTable))
                    {
                        if (getTable.isDirty)
                        {
                            clickedTable = getTable;
                            clickedTable.interacting = true;
                            //        originSize = Camera.main.orthographicSize;
                            if(Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
                            if(Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
                         //   entireStart = true;
                            StopAllCoroutines();
                            StartCoroutine(ViewEntireScreen());
                            check = true;
                        }
                    }
                }

                if(!check)
                {
                //    entireStart = false;
                    StopAllCoroutines();
                    StartCoroutine(BecomeToOrgin());
                }
            }
            else lastClick = Time.time;
        }
    }


  /*  void Unlock()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                Ray ray = cachingCamera.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
                {
                    if (h.collider.gameObject.TryGetComponent<UnlockableBuyer>(out UnlockableBuyer unlockableBuyer))
                    {
                        currentUnlockableBuyer = unlockableBuyer;
                        unlockableBuyer.MouseDown();
                    }
                }
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled)
            {
                if (currentUnlockableBuyer != null)
                {
                    currentUnlockableBuyer.MouseUp();
                    currentUnlockableBuyer = null;
                    isDragging = false;
                }
            }
          //  else if(touch.phase)
         *//*   else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
                {
                    if (h.collider.gameObject.TryGetComponent<UnlockableBuyer>(out UnlockableBuyer unlockableBuyer))
                    {
                        if (unlockableBuyer != currentUnlockableBuyer)
                        {
                            currentUnlockableBuyer.MouseUp();
                            currentUnlockableBuyer = null;
                            isDragging = false;
                        }
                    }
                }
                else
                {
                    if (currentUnlockableBuyer != null)
                    {
                        currentUnlockableBuyer.MouseUp();
                        currentUnlockableBuyer = null;
                        isDragging = false;
                    }
                }
            }*//*
        }
       *//* else
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }*//*
    }*/
/*    void Unlock_T()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray();
            if (Application.platform == RuntimePlatform.WindowsEditor) ray = cachingCamera.ScreenPointToRay(Input.mousePosition);
          
            if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
            {
                if (h.collider.gameObject.TryGetComponent<UnlockableBuyer>(out UnlockableBuyer unlockableBuyer))
                {
                    currentUnlockableBuyer = unlockableBuyer;
                    unlockableBuyer.MouseDown();
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }
        else
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }
    }*/


    public Vector3 lastLoc;
    /*private void ClickMachine()
    {
        float test = (preLoc - curLoc).magnitude;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);    
            if ((touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled) && test < 0.01f)
            {
                Ray ray = cachingCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 13))
                {
                    if (hit.collider.gameObject.GetComponentInParent<FoodMachine>() != null)
                    {
                        GameInstance.GameIns.applianceUIManager.ShowApplianceInfo(hit.collider.gameObject.GetComponentInParent<FoodMachine>());
                    }
                }
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Animal")))
                {
                    Animal animal = hit.collider.gameObject.GetComponentInParent<Animal>();
                    if (animal)
                    {
                        if (animal.GetComponentInChildren<Employee>() != null)
                        {
                            GameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(animal.GetComponentInChildren<Employee>());
                        }
                    }
                }
            }
        }
    }*/
    private void ClickMachine_T()
    {

        float test = (preLoc - curLoc).magnitude;

        if (Input.GetMouseButtonUp(0) && test < 0.01f)
        {
            Ray ray = cachingCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 13))
            {
                if (hit.collider.gameObject.GetComponentInParent<FoodMachine>() != null)
                {
                    GameInstance.GameIns.applianceUIManager.ShowApplianceInfo(hit.collider.gameObject.GetComponentInParent<FoodMachine>());
                }
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Animal")))
            {
                Animal animal = hit.collider.gameObject.GetComponentInParent<Animal>();
                if (animal)
                {
                    if (animal.GetComponentInChildren<Employee>() != null)
                    {
                        GameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(animal.GetComponentInChildren<Employee>());
                    }
                }
            }
        }
    }
  
}
