
using System.Collections;
using System.Collections.Generic;
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
    void Start()
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

        Door door = GameInstance.GameIns.restaurantManager.door;

        if(!door.setup)
        {
            RaycastHit[] hits = Physics.RaycastAll(transforms.position + Vector3.up, offset.forward, float.MaxValue, 1 << 16);
            Debug.DrawLine(transforms.position + Vector3.up, transforms.position - transforms.forward * float.MaxValue, Color.red, 5);
            for (int i = 0; i < hits.Length; i++)
            {

                door.gameObject.SetActive(true);
                door.setup = true;
                GameObject h = hits[i].collider.gameObject;
                door.transform.localScale = Vector3.zero;
                door.removeWall = h;
                h.SetActive(false);
                door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                door.transform.rotation = h.transform.rotation * Quaternion.Euler(0,90,0);
                StartCoroutine(door.OpenDoor());
                return;
            }
        }


        GameInstance.GameIns.workSpaceManager.unlockCounter[(int)counterType - 1] = true;
    }
}
