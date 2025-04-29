//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class InputManger : MonoBehaviour
//{
//    GameInstance GameInstance = new GameInstance();
//    public Transform cameraTrans; // ī�޶��� Transform
//    public Transform cameraRange; // ī�޶� �̵� ����
//    public float maxCameraLocX; // ī�޶��� �ִ� X ��ġ
//    public float maxCameraLocZ; // ī�޶��� �ִ� Z ��ġ
//    public float minCameraLocX; // ī�޶��� �ּ� X ��ġ
//    public float minCameraLocZ; // ī�޶��� �ּ� Z ��ġ
//    public float cameraSpeed; // ī�޶� �̵� �ӵ�
//    int money; // ���� ��
//    AnimalManager animalManager; // ���� ����
//    RestaurantManager restaurantManager; // ������� ����
//    public UIManager UIManager; // UI ����
//    Vector2 cameraLoc = new Vector2(0, 0); // ī�޶� ��ġ
//    public int Money { get { return money; } set { money = value; UIManager.UpdateMoneyText(Money); } } // �� �Ӽ�

//    public GameObject go;
//    // Start�� ù ������ ���� ȣ��˴ϴ�.
//    void Start()
//    {

//        GameInstance.GameIns.uiManager = UIManager;
//        GameInstance.GameIns.inputManager = this;
//        Money = 10000; // ������ �� ���� 10000���� ����
//        restaurantManager = GetComponent<RestaurantManager>(); // RestaurantManager ������Ʈ ��������
//        animalManager = GetComponent<AnimalManager>(); // AnimalManager ������Ʈ ��������
//        animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // ���� ���� ����
//        animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // �� �ٸ� ���� ���� ����
//    }

//    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
//    void Update()
//    {
//        // if (animalManager.mode == AnimalManager.Mode.GameMode)
//        {
//            // ī�޶� �̵� ���̶�� �Է� ����
//            if (Zoom.isMoving)
//                return;
//            if (Input.touchCount > 0)
//            { 
//                Touch touch = Input.GetTouch(0);



//                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Ŭ���� ��ġ���� Ray ����
//                RaycastHit hit; // Ray�� �浹�� ������ ���� ����
//                if (Physics.Raycast(ray, out hit, 5000f)) // Raycast�� �浹 �˻�
//                {
//                    NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget ������Ʈ ��������
//                    if (nt)
//                    {
//                        // ���� ����ϸ� ������� ���� ��
//                        if (money >= nt.money)
//                        {
//                            Money -= nt.money; // �� ����
//                            restaurantManager.LevelUp(); // ���� �� ȣ��
//                        }
//                    }
//                }

//            }
//            //touch.position;
//           // Camera.main.ScreenPointToRay(touch.position);
//            // �����̽��ٸ� ������ �� �� ���� ����
//            if (Input.GetKeyDown(KeyCode.Space))
//            {

//                //  animalManager.SpawnAnimal(AnimalController.PlayType.Customer, new FoodsAnimalsWant());
//            }

//            // ���콺 ���� ��ư Ŭ�� ��
//            if (Input.GetMouseButtonDown(0))
//            {
//                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Ŭ���� ��ġ���� Ray ����
//                RaycastHit hit; // Ray�� �浹�� ������ ���� ����
//                if (Physics.Raycast(ray, out hit, 5000f)) // Raycast�� �浹 �˻�
//                {
//                    NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget ������Ʈ ��������
//                    if (nt)
//                    {
//                        // ���� ����ϸ� ������� ���� ��
//                        if (money >= nt.money)
//                        {
//                            Money -= nt.money; // �� ����
//                            restaurantManager.LevelUp(); // ���� �� ȣ��
//                        }
//                    }

//                    MoneyPile pile = hit.collider.gameObject.GetComponent<MoneyPile>();
//                    if (pile)
//                    {
//                        pile.RemoveAllChildren();
//                    }
//                }
//            }

//            // �Է¹��� ���� �� ���� �� ��
//            float h = Input.GetAxisRaw("Horizontal");
//            float v = Input.GetAxisRaw("Vertical");

//            if (Vector3.zero != new Vector3(h, 0, v))
//            {
//                // ī�޶� ��ġ ��������
//                Vector3 loc = cameraTrans.position;
//                // ī�޶� �̵�
//                cameraTrans.Translate(new Vector3(h, 0, v).normalized * cameraSpeed * Time.deltaTime, Space.Self);

//                bool isWall = Physics.CheckSphere(cameraRange.position, 1, LayerMask.GetMask("wall"));
//                bool isDoor = Physics.CheckSphere(cameraRange.position, 1, LayerMask.GetMask("door"));
//                if (isWall || isDoor)
//                {
//                    cameraTrans.position = loc; // ���� ��ġ�� ����
//                }
//            }
//        }
//    }
//}


using CryingSnow.FastFoodRush;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
//using UnityEditor.Experimental.GraphView;
//using static DG.Tweening.DOTweenModuleUtils;

public class InputManger : MonoBehaviour
{
    public Transform cameraTrans; // ī�޶��� Transform
    public Transform cameraRange; // ī�޶� �̵� ����
    public float maxCameraLocX; // ī�޶��� �ִ� X ��ġ
    public float maxCameraLocZ; // ī�޶��� �ִ� Z ��ġ
    public float minCameraLocX; // ī�޶��� �ּ� X ��ġ
    public float minCameraLocZ; // ī�޶��� �ּ� Z ��ġ
    public float cameraSpeed; // ī�޶� �̵� �ӵ�
    //float money; // ���� ��
    AnimalManager animalManager; // ���� ����
    RestaurantManager restaurantManager; // ������� ����
    public UIManager UIManager; // UI ����
    Vector2 cameraLoc = new Vector2(0, 0); // ī�޶� ��ġ
    //public float Money { get { return money; } set { money = value; UIManager.UpdateMoneyText(Money); } } // �� �Ӽ�
    private Vector3 nowPos, prePos;
    private Vector3 movePos;
    public GameObject go;

    public GameObject testTrash;

    public bool inputDisAble;
    public bool bCleaningMode; //Ź�ڸ� ġ�� �� �ִ°�
 //   public bool a;
//    Vector3 lastPoint = new Vector3();

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
    private void Awake()
    {
        es = GetComponent<EventSystem>();
        Application.targetFrameRate = 60;//(int)Screen.currentResolution.refreshRateRatio.numerator;
      //  GameInstance.GameIns.uiManager = UIManager;
        GameInstance.GameIns.inputManager = this;
        ped = new PointerEventData(es);
    }
    
   
    // Start�� ù ������ ���� ȣ��˴ϴ�.
    void Start()
    {
      
        //Money = 100000; // ������ �� ���� 10000���� ����

        restaurantManager = GetComponent<RestaurantManager>(); // RestaurantManager ������Ʈ ��������
        animalManager = GetComponent<AnimalManager>(); // AnimalManager ������Ʈ ��������
     //   animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // ���� ���� ����
        //animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // �� �ٸ� ���� ���� ����
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // �� �ٸ� ���� ���� ����
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // �� �ٸ� ���� ���� ����
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // �� �ٸ� ���� ���� ����
        zOffset = cameraTrans.position.z - Camera.main.transform.position.z;


        originSize = Camera.main.orthographicSize;
    }
    public float dragSpeed = 2.0f;
    public Transform centerObject;       // ȭ�� �߾� ���� ������Ʈ (���̸� ��� ����)
    public LayerMask navigationLayer;    // �̵� ������ ������ ���̾�

    private Vector3 dragOrigin;          // �巡�� ���� ��ġ�� ���� ��ǥ
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
    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
           // for (int i = 0; i < 1; i++)
            {
            //    SaveLoadTest.WriteData();
              //  SaveLoadTest.ReadData();
            }
            Camera.main.orthographicSize -= 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.P))
        {
            Camera.main.orthographicSize += 5f * Time.deltaTime;
          //  SaveLoadTest.ReadData();
            // Camera.main.orthographicSize += 5f * Time.deltaTime;
        }
        /*  if (Input.GetKeyDown(KeyCode.Escape)) {
              Vector3 vbv = Camera.main.ScreenToWorldPoint(GameInstance.GameIns.uiManager.moneyText.rectTransform.position);
              testTrash.transform.position = new Vector3(vbv.x, vbv.y -1, vbv.z);
              //Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(testTrash.transform.position.x, testTrash.transform.position.y, Camera.main.nearClipPlane));
          }*/
        preLoc = curLoc;
        curLoc = cameraRange.position;
        if (!inputDisAble)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                DragScreen_WindowEditor();
                Unlock_T();
                ClickMachine_T();
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                DragScreen_Android();
                Unlock();
                ClickMachine();
            }

           
            //   DoubleClick();

            /*   if (clickedTable != null)
               {
                   Vector2 viewLocation;

                   if (Application.platform == RuntimePlatform.WindowsEditor) viewLocation = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                   else if (Input.touchCount > 0)
                   {
                       Touch t = Input.GetTouch(0);
                       viewLocation = Camera.main.ScreenToViewportPoint(t.position);
                   }
                   else viewLocation = new Vector2(0.5f, 0.5f);

                   if (viewLocation.x < 0.01f)
                   {
                       Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(-1, 0, 0);
                       RayMove(dir, 30);
                   }
                   if (viewLocation.x > 0.99f)
                   {
                       Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(1, 0, 0);
                       RayMove(dir, 30);
                   }
                   if (viewLocation.y > 0.99f)
                   {
                       Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(0, 0, 1);
                       RayMove(dir, 30);
                   }
                   if (viewLocation.y < 0.01f)
                   {
                       Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(0, 0, -1);
                       RayMove(dir, 30);
                   }

               }*/

        }  
        else
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    dragOrigin = hit.point;
                }
                isDragging = false;
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                if(Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    {
                        dragOrigin = hit.point;
                    }
                }
                isDragging = false;
            }
        }
    }

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

        //���� �����Ӱ� ���� ������ ������ ���̷� �Ÿ��� ���
        // ����ĳ�������� ���� ���� �������� �̵�
        if (hitPoint != Vector3.zero)
        {
            //�ִ� �Ÿ� �̵� ������ ���� ���� ���� ��
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
            //���� ������ ����
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

        if (isDragging)
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
    Vector3 tempCameraLoc;
    public IEnumerator BecomeToOrgin()
    {
        float retunSize = Camera.main.orthographicSize;

        double r = Camera.main.orthographicSize;
        float currentSize = (float)r;
        double t = 0;

        Ray ray2 = Camera.main.ScreenPointToRay(targetVector);
        if (Physics.Raycast(ray2, out RaycastHit hitres2, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hitres2.point;
        }
            //    StartCoroutine(MoveCamera());//cameraTrans.DOMove(tempCameraLoc, 5f);
        while (t < 1)
        {
            t += 5 *Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, originSize, (float)t);
            Camera.main.orthographicSize = currentSize;
            Vector3 newLoc;
             Ray ray = Camera.main.ScreenPointToRay(targetVector);
            if (Physics.Raycast(ray, out RaycastHit hitres, Mathf.Infinity, LayerMask.GetMask("Default")))
            { 
                newLoc = hitres.point;
                float s = (target - newLoc).magnitude;
                Vector3 dir = (target - newLoc).normalized;
                if (RayMove(dir, s / Time.deltaTime) == false)
                {
                    Camera.main.orthographicSize = retunSize;
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
        cameraTrans.DOMove(tempCameraLoc, 0.5f).SetEase(Ease.InBack).OnComplete(() => { t = false; });
        while (t)
        {
            yield return null;
        }
    }

    IEnumerator ViewEntireScreen()
    {
        float retunSize = Camera.main.orthographicSize;

        if(Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
        if(Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
        Ray ray = Camera.main.ScreenPointToRay(targetVector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hit.point;
        }
        double r = Camera.main.orthographicSize;
        float currentSize = (float)r;
        double t = 0;
        while (t < 1)
        {
          //      retunSize = currentSize;
            t += 1 * Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, 50, (float)t);

            //     Camera.main.WorldToScreenPoint(targetVector);
          
            Camera.main.orthographicSize = currentSize;
            Vector3 cV = cameraTrans.position;
            Ray ray2 = Camera.main.ScreenPointToRay(targetVector);

            if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                Vector3 dir = target - hit2.point;
                float s = dir.magnitude;
                Vector3 d = GameInstance.GetVector3(dir.x, 0, dir.z).normalized; //new Vector3(dir.x, 0, dir.z).normalized;
                //  cameraTrans.Translate(d * Time.deltaTime * 50);
               if (RayMove(d, s/ Time.deltaTime) == false)
                {
                    tempCameraLoc = cameraTrans.position;
                    Camera.main.orthographicSize = retunSize;
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
    Touch dragTouch;
    public void DragScreen_Android(bool draged = false)
    {
        if (Input.touchCount > 0 && inOtherAction == false || draged)
        {
            if (Input.touchCount == 1)
            {
                dragTouch = Input.GetTouch(0);
                if (dragTouch.phase == TouchPhase.Began || manyFingers)
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
                        Ray ray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) || draged)
                        {
                            dragOrigin = hit.point;
                            isDragging = true;
                            deltaResult = hit;
                        }

                        if (dragTouch.phase == TouchPhase.Began)
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
                  /*  if (clickedTable != null)
                    {
                        lastVector = dragTouch.position;
                        Ray ray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 5))
                        {
                            clickedTable.trashPlate.transform.position = h.point;
                        }
                    }*/
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

                // ���� �����ӿ��� �� ��ġ�� ��ġ ���� ���
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // ���� �� ���� ��ġ ���� �Ÿ� ���
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // �� �Ÿ��� ���̷� �� ���� ���
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                Camera.main.orthographicSize += deltaMagnitudeDiff * Time.deltaTime;

                if (Camera.main.orthographicSize < 8) Camera.main.orthographicSize = 8;
                else if (Camera.main.orthographicSize > 50) Camera.main.orthographicSize = 50;
            }

            if (Input.touchCount > 1) manyFingers = true;
        }
        else
        {
           /* if (clickedTable != null && inOtherAction == false)
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
            }*/

            if (dragTimer > 0)
            {
                dragTimer -= Time.deltaTime;
                Vector3 currentPoint = hit.point;
                if (RayMove(dir, diff) == false) dragTimer = 0;
                diff -= Time.deltaTime * reduceSpeed;
                if(diff < 0) diff = 0;
            }
        }
    }


    void DoubleClick()
    {
       // Input.touchCount == 1
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            if(Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase != TouchPhase.Began) return;
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

    UnlockableBuyer currentUnlockableBuyer;

    void Unlock()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
                {
                    if (h.collider.gameObject.TryGetComponent<UnlockableBuyer>(out UnlockableBuyer unlockableBuyer))
                    {
                        currentUnlockableBuyer = unlockableBuyer;
                        unlockableBuyer.MouseDown();
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (currentUnlockableBuyer != null)
                {
                    currentUnlockableBuyer.MouseUp();
                    currentUnlockableBuyer = null;
                    isDragging = false;
                }
            }
          //  else if(touch.phase)
         /*   else
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
            }*/
        }
       /* else
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }*/
    }
    void Unlock_T()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray();
            if (Application.platform == RuntimePlatform.WindowsEditor) ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          
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
    }

    // private void OnDrawGizmos()
    // {
    // Gizmos.color = Color.green;
    // Gizmos.DrawWireSphere(cameraRange.position, Camera.main.orthographicSize / 2.5f); //new Vector3(Camera.main.orthographicSize / 2.5f * 2, 0, Camera.main.orthographicSize / 2.5f * 2));
    //  }

    public Vector3 lastLoc;
    private void ClickMachine()
    {
        float test = (preLoc - curLoc).magnitude;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);    
            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && test < 0.01f)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
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
    private void ClickMachine_T()
    {

        float test = (preLoc - curLoc).magnitude;

        if (Input.GetMouseButtonUp(0) && test < 0.01f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    // bool dragging;
    //// Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    //  void Update()
    //  {

    //     //  ī�޶� �̵� ���̶�� �Է� ����
    //      if (Zoom.isMoving)
    //          return;
    //      HandleDragging();
    //    //   ��ġ �Է��� ó��

    //      if (Application.platform == RuntimePlatform.WindowsEditor)
    //      {
    //    //       ���콺 ���� ��ư Ŭ�� ��
    //          if (Input.GetMouseButtonDown(0))
    //          {
    //              Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Ŭ���� ��ġ���� Ray ����
    //              RaycastHit hit; // Ray�� �浹�� ������ ���� ����
    //              if (Physics.Raycast(ray, out hit, 5000f)) // Raycast�� �浹 �˻�
    //              {
    //                  NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget ������Ʈ ��������
    //                  if (nt)
    //                  {
    //          //             ���� ����ϸ� ������� ���� ��
    //                      if (money >= nt.money)
    //                      {
    //                          Money -= nt.money; // �� ����
    //                          restaurantManager.LevelUp(); // ���� �� ȣ��
    //                      }
    //                  }

    //                  MoneyPile pile = hit.collider.gameObject.GetComponent<MoneyPile>();
    //                  if (pile)
    //                  {
    //                      pile.RemoveAllChildren();
    //                  }
    //              }
    //          }


    //          if (inOtherAction == false)
    //          {
    //              if (a)
    //              {
    //                  a = false;
    //                  prePos = Input.mousePosition;//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    //              }
    //              else
    //              {
    //                  if (Input.GetMouseButtonDown(0))
    //                  {
    //                      RaycastHit hit;
    //                      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, LayerMask.GetMask("Default")))
    //                      {
    //                          inLine = true;
    //                          audioSource.Play();
    //                          particleSystem.Play();
    //                          prePos = Input.mousePosition; //Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y));
    //                          Vector3 vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //                          particleSystem.gameObject.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
    //                          dragging = true;
    //                      }
    //                      else { inLine = false; }
    //                  }

    //                  if (dragging && inLine)
    //                  {

    //                      nowPos = Input.mousePosition;//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y));
    //                      movePos = (prePos - nowPos).normalized;

    //                      Vector3 dir =  (Quaternion.Euler(0,45,0) * new Vector3(movePos.x, 0, movePos.y)).normalized;

    //                      float mag = (prePos - nowPos).magnitude * cameraSpeed * Time.deltaTime;
    //                      CheckRay(mag, dir);
    //                      prePos = nowPos;
    //                      RaycastHit hit;
    //                      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, LayerMask.GetMask("Default")))
    //                      {
    //                           particleSystem.gameObject.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
    //                          particleSystem.Pause();
    //                      }
    //                      else
    //                      {
    //                          particleSystem.Clear();
    //                          particleSystem.Stop();
    //                      }
    //                  }
    //                  if (Input.GetMouseButtonUp(0))
    //                  {
    //                      dragging = false;
    //                      particleSystem.Clear();
    //                      particleSystem.Stop();
    //                  }
    //              }

    //          }
    //      }

    //      if (Application.platform == RuntimePlatform.Android)
    //      {
    //          if (Input.touchCount > 0 && inOtherAction == false)
    //          {
    //        //       ī�޶� �̵�: �� �հ��� �巡�׷� ����
    //              if (Input.touchCount == 1)
    //              {
    //                  Touch touch = Input.GetTouch(0);

    //                  if (a)
    //                  {
    //                      prePos = touch.position - touch.deltaPosition;
    //                      a = false;
    //                  }
    //                  else
    //                  {
    //                      if (touch.phase == TouchPhase.Began)
    //                      {
    //                          if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), 100f))
    //                          {
    //                              inLine = true;
    //                              audioSource.Play();
    //                              prePos = touch.position - touch.deltaPosition;
    //                          }
    //                          else
    //                          {
    //                              inLine = false;
    //                          }
    //                      }
    //                      else if (touch.phase == TouchPhase.Moved && inLine)
    //                      {

    //                          nowPos = touch.position - touch.deltaPosition;
    //                          movePos = (Vector3)(prePos - nowPos);

    //                          if (movePos != Vector3.zero)
    //                          {
    //                              Vector3 dir = Quaternion.Euler(0, 45, 0) * movePos.normalized;
    //                              float mag = (prePos - nowPos).magnitude;// * Camera.main.orthographicSize / (size + Mathf.Abs(dir.x) * (Screen.height / Screen.width)) * cameraSpeed;
    //                               mag = mag > 50f ? 50f : mag;
    //                              CheckRay(mag, dir);
    //                          }
    //                          prePos = nowPos;
    //                      }
    //                  }
    //              }

    //           //    ��ġ �� ����: �� �հ������� Ȯ��/���
    //              if (Input.touchCount == 2)
    //              {
    //                  a = true;
    //                  Touch touchZero = Input.GetTouch(0);
    //                  Touch touchOne = Input.GetTouch(1);


    //                //   ���� �����ӿ��� �� ��ġ�� ��ġ ���� ���
    //                  Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //                  Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    //              //     ���� �� ���� ��ġ ���� �Ÿ� ���
    //                  float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //                  float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

    //               //    �� �Ÿ��� ���̷� �� ���� ���
    //                  float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;



    //                  Camera.main.orthographicSize += deltaMagnitudeDiff * Time.deltaTime;



    //                  if (Camera.main.orthographicSize < 8) Camera.main.orthographicSize = 8;
    //                  else if (Camera.main.orthographicSize > 50) Camera.main.orthographicSize = 50;

    //                  nowPos = prePos;
    //              }
    //          }
    //      }
    //  }
    //  void HandleDragging()
    //  {
    //      if (Input.GetMouseButtonDown(1))
    //      {
    //      //     �巡�� ���� ��ġ�� ���� ��ǥ�� ��ȯ�ؼ� ���� (���ұ׷���)
    //           // movePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));

    //         dragOrigin = Input.mousePosition;
    //         movePos = Input.mousePosition;
    //      }

    //      if (Input.GetMouseButton(1))
    //      {
    //         // �巡�� ���� ���� ���콺 ��ġ ���� ��ǥ�� ���� (���ұ׷���)
    //         Vector3 currentPoint = Input.mousePosition; //Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    //          Vector3 difference = dragOrigin - currentPoint; // ���� ��ġ�� ���� ��ġ ���� ���

    //          Vector3 move = new Vector3(difference.x, difference.y, difference.z);
    //          CheckRay(difference.magnitude * Time.deltaTime * cameraSpeed, move);

    //         dragOrigin = currentPoint;
    //      }
    //  }


    //  void CheckRay(float speed , Vector3 dir)
    //  {
    //      float currentMagnitude = 0;
    //      float divdedSpeed = speed / 1000f;
    //      float f = 0.001f;

    //      Vector3 dirs = new Vector3();
    //      while (f <= 1f) {
    //          dirs = (Quaternion.Euler(0, 45, 0) * new Vector3(dir.x , dir.z, dir.y)).normalized;

    //  //        dirs = cameraTrans.transform.TransformDirection(movePos.x, 0, movePos.y*1.15f);
    //          RaycastHit hit;
    //          RaycastHit hit2;
    //          float lerps =  Mathf.Lerp(divdedSpeed, speed , f);

    //    //       Debug.DrawLine(cameraRange.position, cameraRange.position + dirs * cameraSpeed * Time.deltaTime, Color.red, 0.5f);
    //          bool isWall = Physics.CheckSphere(cameraRange.position + dirs * lerps, 1f, LayerMask.GetMask("wall"));
    //          bool isDoor = Physics.CheckSphere(cameraRange.position + dirs * lerps, 1f, LayerMask.GetMask("door"));
    //          //bool isWall = Physics.Raycast(cameraRange.position, dir, out hit, lerps , LayerMask.GetMask("wall"));
    //          // bool isDoor = Physics.Raycast(cameraRange.position, dir, out hit2, lerps , LayerMask.GetMask("door"));
    //          if (!isWall && !isDoor)
    //          {
    //              currentMagnitude = lerps;
    //           //   cameraTrans.Translate(dir * currentMagnitude * Time.deltaTime, Space.World);
    //          }
    //          else
    //          {
    //          //    Debug.DrawLine(cameraRange.position, cameraRange.position + dirs * lerps , Color.red);
    //              break;
    //          }
    //          f += 0.001f;
    //      }
    //      if (dirs != Vector3.zero)
    //      {
    //          Vector3 origin = cameraRange.position;
    //          Vector3 target = cameraRange.position + dirs * currentMagnitude * Time.deltaTime;
    //          //target -= new Vector3(0, dirs.y * currentMagnitude * Time.deltaTime, 0);

    //          Vector3 pos = cameraTrans.position + dirs * currentMagnitude * Time.deltaTime;
    //           //cameraTrans.position += move;
    //          cameraTrans.Translate(dirs * cameraSpeed * Time.deltaTime, Space.World);
    //       //   cameraRange.position -= new Vector3(0, dir.y * cameraSpeed * Time.deltaTime, 0);

    //          float player = (new Vector3(cameraRange.position.x, 0, cameraRange.position.z) - new Vector3(origin.x, 0, origin.z)).magnitude;
    //          float t = (new Vector3(target.x,0, target.z) - new Vector3(origin.x,0, origin.z)).magnitude;
    //       //   cameraTrans.Translate(new Vector3(dirs.x, 0, dirs.z)* cameraSpeed * Camera.main.orthographicSize / size * speed * Time.deltaTime, Space.World);
    //         // if ((cameraRange.position - origin).magnitude > (target - origin).magnitude)
    //          {
    //        //      cameraTrans.position = pos;
    //      //        cameraRange.position -= new Vector3(0, dirs.y * currentMagnitude * Time.deltaTime, 0);
    //          }
    //      }
    //  }
}
