
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
//using TMPro.EditorUtilities;
public class Counter : Furniture, IObjectOffset
{

    public Employee employee;
    public Customer customer;
    public UIManager uiManager;
    public Customer Customer
    {
        get { return customer; }
        set { customer = value;}
    }

    [field: SerializeField]
    public Transform offset { get; set; }

    public Transform workingSpot;
    public WorkingSpot[] workingSpot_SmallTables;

    public Transform[] stackPoints;
    public Transform smallTable;
    public Transform smallTable2;
    public QueuePoint[] queuePoints;

    public FoodMachine[] foodMachines; 
    public List<FoodStack> foodStacks = new List<FoodStack>();
    public CounterType counterType;
    public MoneyPile moneyPile;
    public Transform transforms;
    // Start is called before the first frame update
    private void Awake()
    {
        transforms = transform;
    }
    public override void Start()
    {
        
        switch (counterType)
        {

            case CounterType.None:
                break;
            default:
                {
                    int identy = GetInstanceID();
                    if (counterType == CounterType.FastFood || counterType == CounterType.TakeOut)
                    {
                        FoodStack burgerStack = new FoodStack();
                        burgerStack.type = MachineType.BurgerMachine;
                        burgerStack.id = identy + 1;
                        foodStacks.Add(burgerStack);
                        FoodStack cokeStack = new FoodStack();
                        cokeStack.type = MachineType.CokeMachine;
                        foodStacks.Add(cokeStack);
                        cokeStack.id = identy + 2;
                        GameInstance.GameIns.restaurantManager.foodStacks.Add(burgerStack);
                        GameInstance.GameIns.restaurantManager.foodStacks.Add(cokeStack);
                    }

                    if (counterType == CounterType.Donuts || counterType == CounterType.TakeOut)
                    {
                        FoodStack CoffeeStack = new FoodStack();
                        CoffeeStack.type = MachineType.CoffeeMachine;
                        foodStacks.Add(CoffeeStack);
                        FoodStack DonutStack = new FoodStack();
                        DonutStack.type = MachineType.DonutMachine;
                        foodStacks.Add(DonutStack);
                        CoffeeStack.id = identy + 3;
                        DonutStack.id = identy + 4;
                        GameInstance.GameIns.restaurantManager.foodStacks.Add(CoffeeStack);
                        GameInstance.GameIns.restaurantManager.foodStacks.Add(DonutStack);
                    }
                    break;

                    
                }
        }

        GameInstance.GameIns.workSpaceManager.unlockCounter[(int)counterType - 1] = true;
        GameInstance.GameIns.workSpaceManager.counters.Add(this);

        base.Start();

        ShowSells(App.GlobalToken).Forget();

        Door door = GameInstance.GameIns.restaurantManager.door;

        if(!door.setup)
        {
            door.gameObject.SetActive(true);
            door.setup = true;
            door.transform.localScale = Vector3.zero;
            if (Physics.Raycast(transforms.position + Vector3.up, offset.forward, out RaycastHit hits, float.MaxValue, 1 << 16 | 1 << 19))
            {
                Debug.DrawLine(transforms.position + Vector3.up, transforms.position - transforms.forward * float.MaxValue, Color.red, 5);
              
                GameObject h = hits.collider.gameObject;
                door.removeWall = h;
                MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, h.GetComponentInChildren<Collider>(), false);
                h.SetActive(false);
                door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                door.transform.SetParent(h.transform.parent);
                StartCoroutine(door.OpenDoor());
                return;
            }
            float angle = 30f;
            Vector3 leftDir = Quaternion.AngleAxis(-angle, offset.up) * offset.forward;
            if (Physics.Raycast(transforms.position + Vector3.up, leftDir, out RaycastHit hit2, float.MaxValue, 1 << 16 | 1 << 19))
            {
                Debug.DrawLine(transforms.position + Vector3.up, leftDir * float.MaxValue, Color.red, 5);
              
                GameObject h = hit2.collider.gameObject;
                door.removeWall = h;
                MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, h.GetComponentInChildren<Collider>(), false);
                h.SetActive(false);
                door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                door.transform.SetParent(h.transform.parent);
                StartCoroutine(door.OpenDoor());
                return;
            }
            Vector3 rightDir = Quaternion.AngleAxis(angle, offset.up) * offset.forward;
            if (Physics.Raycast(transforms.position + Vector3.up, rightDir, out RaycastHit hit3, float.MaxValue, 1 << 16 | 1 << 19))
            {
                Debug.DrawLine(transforms.position + Vector3.up, rightDir * float.MaxValue, Color.red, 5);

                GameObject h = hit3.collider.gameObject;
                door.removeWall = h;
                MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, h.GetComponentInChildren<Collider>(), false);
                h.SetActive(false);
                door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                door.transform.SetParent(h.transform.parent);
                StartCoroutine(door.OpenDoor());
                return;
            }
        }

        
      

    }

    async UniTask ShowSells(CancellationToken cancellationToken = default)
    {
        await UniTask.Delay(500, cancellationToken: cancellationToken);

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++) spriteRenderers[i].enabled = true;
     
    }
}
