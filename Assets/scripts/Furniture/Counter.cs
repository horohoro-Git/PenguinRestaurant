
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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
    public HashSet<Employee> employees = new HashSet<Employee>();
    public HashSet<Customer> customers = new HashSet<Customer>();

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

            int[] angleRange = { 0, 15, -15, 30, -30, 45, -45, 60, -60, 75, -75, 90, -90, 120, -120, 150, -150, 180 };
            for(int i = 0; i < angleRange.Length; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(angleRange[i], offset.up) * offset.forward;
                if (Physics.Raycast(transforms.position + Vector3.up, dir, out RaycastHit hitWall, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hitWall.collider.gameObject;

                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);

                    GameInstance.GameIns.restaurantManager.doorPreview.transform.position = door.transform.position;
                    GameInstance.GameIns.restaurantManager.doorPreview.rotateOffset.transform.rotation = door.transform.rotation * Quaternion.Euler(0, 90, 0);
                    if (GameInstance.GameIns.restaurantManager.doorPreview.CheckDoorPlacement())
                    {
                        door.transform.SetParent(h.transform.parent);
                        door.removeWall = h;
                        MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, h.GetComponentInChildren<Collider>(), false);
                        h.SetActive(false);
                    }
                    break;
                }
            }
        }

    }

    private void OnEnable()
    {
        for (int i = 0; i < foodStacks.Count; i++)
        {
            foreach (var f in foodStacks[i].foodStack)
            {
                Vector3 pos = stackPoints[i].transform.position;
                f.transform.position = new Vector3(pos.x, f.transform.position.y, pos.z);
                f.gameObject.SetActive(true);
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        foreach (var v in foodStacks)
        {
            foreach (var f in v.foodStack)
            {
                if(f!=null)f.gameObject.SetActive(false);
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
