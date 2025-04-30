using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization.Formatters;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static UnityEngine.GraphicsBuffer;
/*
public class Coord
{
    public int r, c;
    public bool blocked;
    public int startPoint;
    public int endPoint;
    public int dPoint;
    public int point;
 
    public Coord parent = null;
    public Coord(int r, int c, int startPoint = 0, int endPoint = 0, bool blocked = false)
    {
        this.r = r;
        this.c = c;
        this.blocked = blocked;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        dPoint = 0;
        point = 0;
    }

    public void Setup()
    {
        point = startPoint + endPoint;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Food food;
    public Customer customer;
    public Animal player;
    public WorkSpace[] workSpaces;
    public GameObject target;
    public GameObject block;
    public Vector2[] blockVector;
    public GameObject go;
    public int sizeX;
    public int sizeY;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float distanceSize;
    public float height;
    int[] moveX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] moveY = { 0, 0, 1, -1, 1, -1, 1, -1 };
    Coord[,] coords;
    bool startRoutine = false;
    // Start is called before the first frame update
    void Start()
    {
       
        player.GetComponent<Animal>().routineAction = (targets) => {

            if (player.state != Animal.AnimalState.Counter)
            {
                if (player.state != Animal.AnimalState.Walking)
                {
                    CalculatorScale calculatorScale = new CalculatorScale();
                    {
                        calculatorScale.minX = minX; calculatorScale.maxX = maxX; calculatorScale.minY = minY;
                        calculatorScale.maxY = maxY; calculatorScale.distanceSize = distanceSize; calculatorScale.height = height;
                    }
                    MoveCalculator calculator = new MoveCalculator();
                    calculator.Init();

                    player.state = Animal.AnimalState.Walking;
                    target = targets.gameObject;

         //           Coord returnCoord = calculator.AStarAlgorithm(player.transform.position, target.transform.position);
           //         if (returnCoord != null) StartCoroutine(player.GetComponent<Animal>().Move(returnCoord, minY, minX, distanceSize, targets));
                }
            }
            else
            {
                CalculatorScale calculatorScale = new CalculatorScale();
                {
                    calculatorScale.minX = minX; calculatorScale.maxX = maxX; calculatorScale.minY = minY;
                    calculatorScale.maxY = maxY; calculatorScale.distanceSize = distanceSize; calculatorScale.height = height;
                }
                MoveCalculator calculator = new MoveCalculator();
                calculator.Init();
               
            //    Coord returnCoord = calculator.AStarAlgorithm(player.transform.position, workSpaces[0].counterTrans.position);
            //    if (returnCoord != null) StartCoroutine(player.GetComponent<Animal>().Move(returnCoord, minY, minX, distanceSize, targets));
            }
        };
        customer.customerAction = ((targets) =>
        {
            if (customer.state != Customer.CustomerState.Wait)
            {
              //  customer.state
                CalculatorScale calculatorScale = new CalculatorScale();
                {
                    calculatorScale.minX = minX; calculatorScale.maxX = maxX; calculatorScale.minY = minY;
                    calculatorScale.maxY = maxY; calculatorScale.distanceSize = distanceSize; calculatorScale.height = height;
                }
                MoveCalculator calculator = new MoveCalculator();
                calculator.Init();
                customer.state = Customer.CustomerState.WalkToCounter;
                GameObject t = targets.foodCustomerTrans.gameObject;
             //   Coord returnCoord = calculator.AStarAlgorithm(customer.transform.position, t.transform.position);
          //      if (returnCoord != null) StartCoroutine(customer.Move(returnCoord, minY, minX, distanceSize, targets));
            }
        });

       
    }

  

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startRoutine = true;
        }


        if (startRoutine)
        {

            if (player.state == Animal.AnimalState.Counter)
            {
                if (!(workSpaces[0].foodStack.Count > 0 && workSpaces[0].customer))
                {
                    if (workSpaces[1].foodStack.Count > 0)
                    {
                        player.state = Animal.AnimalState.Standing;
                        player.routineAction(workSpaces[1]);
                    }
                }
            }
            else
            {
                if (workSpaces[0].foodStack.Count > 0 && workSpaces[0].customer)
                {
                    if (workSpaces[0].foodStack.Count > 0 && workSpaces[0].customer)
                    {
                     //   Debug.Log("AA");
                        player.state = Animal.AnimalState.Counter;
                    }
                }
                else
                {
                    if (player.state == Animal.AnimalState.Standing)
                    {
                        if (workSpaces[1].foodStack.Count > 0)
                        {
                            player.routineAction(workSpaces[1]);
                        }

                    }
                    else if (player.state == Animal.AnimalState.Taking)
                    {

                        player.routineAction(workSpaces[0]);
                    }
                }
            }
            if(customer.state == Customer.CustomerState.Wait)
            {
                customer.state = Customer.CustomerState.WalkToCounter;
                customer.customerAction(workSpaces[0]);
            }
            if(customer.state == Customer.CustomerState.WalkToTable)
            {
                customer.customerAction(workSpaces[2]);
            }
        }
    }

    Coord AStarAlgorithm(Coord start, Coord end)
    {

        List<Coord> openList = new List<Coord>();
        List<Coord> closedList = new List<Coord>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            //Debug.Log("A");
            Coord coord = openList[0];

            closedList.Add(coord);

            openList.RemoveAt(0);

            for (int i = 0; i < 8; i++)
            {

                int r = coord.r + moveY[i];
                int c = coord.c + moveX[i];

                if (ValidCheck(r, c))
                {
                    if (closedList.Contains(coords[r, c]) == false && coords[r, c].blocked == false)
                    {

                        if (openList.Contains(coords[r, c]))
                        {
                            int startPoint = coord.startPoint + ReturnDiff(Mathf.Abs(moveY[i]), Mathf.Abs(moveX[i]));
                            int endPoint = ReturnDiff(Mathf.Abs(r - end.r), Mathf.Abs(c - end.c));
                            if (coords[r, c].point > startPoint + endPoint)
                            {
                                coords[r, c].startPoint = startPoint;
                                coords[r, c].endPoint = endPoint;
                                coords[r, c].Setup();
                                coords[r, c].parent = coord;
                            }
                        }
                        else
                        {
                            int startPoint = coord.startPoint + ReturnDiff(Mathf.Abs(moveY[i]), Mathf.Abs(moveX[i]));
                            int endPoint = ReturnDiff(Mathf.Abs(r - end.r), Mathf.Abs(c - end.c));
                            coords[r, c].startPoint = startPoint;
                            coords[r, c].endPoint = endPoint;
                            coords[r, c].Setup();
                            coords[r, c].parent = coord;
                            openList.Add(coords[r, c]);
                        }
                    }

                    if (r == end.r && c == end.c)
                    {
                        // Debug.Log("A");

                        return end;

                    }
                }

            }
        }

      //  Debug.Log("이동 불가");

        return null;
    }
    int ReturnDiff(int diffX, int diffY)
    {
        if (diffX == diffY) return 14 * diffX;

        int s = diffY > diffX ? diffX : diffY;
        int b = diffY > diffX ? diffY : diffX;

        int p = s * 14;
        p += (b - s) * 10;


        return p;
    }

    bool ValidCheck(int r, int c)
    {
        if (r >= 0 && r < sizeX && c >= 0 && c < sizeY)
        {
            return true;
        }

        return false;
    }

    IEnumerator Move(Coord target)
    {
        Coord cor = target;
        Stack<Coord> stack = new Stack<Coord>();
        stack.Push(cor);
        while (cor.parent != null)
        {
            cor = cor.parent;
            stack.Push(cor);
        }
        stack.Pop();
        while (stack.Count > 0)
        {
            Coord node = stack.Pop();

            float r = minY + node.r * distanceSize;
            float c = minX + node.c * distanceSize;
            Vector3 targetNode = new Vector3(c, height, r);
            Vector3 dir = targetNode - player.transform.position;

            while (true)
            {
                float magnitude = (targetNode - player.transform.position).magnitude;
                if (magnitude <= 0.01f)
                {
                    player.transform.position = targetNode;
                    break;
                }
                player.transform.Translate(dir.normalized * Time.deltaTime * 3f, Space.World);
        

                yield return null;
            }
        }

    }
}

*/