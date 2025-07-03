
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AnimalController;
using static AnimalSpawner;
using static Customer;

[CustomEditor(typeof(AnimalManager))]
public class AnimalManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        AnimalManager manager = (AnimalManager)target;

        if (manager.mode == AnimalManager.Mode.DebugMode)
        {
            if (GUILayout.Button("Spawn Customer"))
            {
                //   bool burger = false;
                // bool coke = false;

                // if (manager.mode == AnimalManager.Mode.DebugMode)
                {
                    FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                    foodsAnimalsWant.burger = true;
                    foodsAnimalsWant.coke = true;
                    foodsAnimalsWant.spawnerType = SpawnerType.FastFood;
                    manager.SpawnCustomer(foodsAnimalsWant);
                }
            }
            GUILayout.Space(3);

            if (GUILayout.Button("Customer Walk To Counter And Place An Order"))
            {
                for (int i = 0; i < manager.customerControllers.Count; i++)
                {
                    Customer customer = manager.customerControllers[i];
                    if (customer.busy == false)
                    {
                        //customer.busy = true;
                        if (customer.customerState == CustomerState.Walk)
                        {
                           
                            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
                            customer.customerState = CustomerState.Counter;
                            customer.CustomerPlayAction(workSpaceManager.counters[(int)customer.foodsAnimalsWant.spawnerType].queuePoints, workSpaceManager.counters[(int)customer.foodsAnimalsWant.spawnerType]);
                            return;
                        }
                    }
                }
            }
            GUILayout.Space(3);

            if (GUILayout.Button("Employee Take Food"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.employeeControllers.Count; f++)
                {
                    Employee em = manager.employeeControllers[f];
                    if (em.busy == false)
                    {
                        Vector3 target;
                        List<FoodMachine> finalMachineList = new List<FoodMachine>();
                        List<FoodStack> foodStacks = new List<FoodStack>();
                        for (int i = 0; i < workSpaceManager.counters.Count; i++)
                        {
                            for (int j = 0; j < workSpaceManager.counters[i].foodStacks.Count; j++) foodStacks.Add(workSpaceManager.counters[i].foodStacks[j]);
                        }

                        for (int i = 0; i < workSpaceManager.counters.Count; i++)
                        {
                            if (workSpaceManager.counters[i].customer != null)
                            {
                                AnimalController customer = workSpaceManager.counters[i].customer;
                                Customer cs = customer.GetComponent<Customer>();    
                                //if(customer.food1 > customer.foodList.Count)
                                for (int j = 0; j < cs.foodStacks.Count; j++)
                                {
                                    for (int k = 0; k < workSpaceManager.foodMachines.Count; k++)
                                    {
                                        if (cs.foodStacks[j].type == workSpaceManager.foodMachines[k].machineType && workSpaceManager.foodMachines[k].foodStack.foodStack.Count + cs.foodStacks[j].foodStack.Count >= cs.foodStacks[j].needFoodNum && workSpaceManager.foodMachines[k].employee == null && cs.foodStacks[j].needFoodNum > cs.foodStacks[j].foodStack.Count)
                                        {
                                            finalMachineList.Add(workSpaceManager.foodMachines[k]);
                                        }
                                    }

                                    if (finalMachineList.Count > 0)
                                    {
                                        finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return (a.gameObject.transform.position - em.transform.position).magnitude.CompareTo((a.gameObject.transform.position - em.transform.position).magnitude); });
                                        finalMachineList[0].employee = em;
                                        em.employeeState = EmployeeState.FoodMachine;
                                        target = finalMachineList[0].workingSpot.position;
                                   //     em.Work(target, finalMachineList[0]);
                                        return;
                                    }
                                }
                            }
                        }

                        foodStacks.Sort(delegate (FoodStack a, FoodStack b) { return (a.foodStack.Count.CompareTo(b.foodStack.Count)); });

                        List<FoodMachine> foodMachineList = new List<FoodMachine>();
                        for (int i = 0; i < workSpaceManager.foodMachines.Count; i++) foodMachineList.Add(workSpaceManager.foodMachines[i]);

                        finalMachineList.Clear();


                        for (int i = 0; i < foodStacks.Count; i++)
                        {
                            for (int j = 0; j < foodMachineList.Count; j++)
                            {
                                if (foodMachineList[j].machineType == foodStacks[i].type && foodMachineList[j].foodStack.foodStack.Count > 0 && foodMachineList[j].employee == null)
                                {

                                    finalMachineList.Add(foodMachineList[j]);
                                }
                            }
                            if (finalMachineList.Count > 0) break;
                        }
                        //Debug.Log(finalMachineList.Count);
                        finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return a.foodStack.foodStack.Count.CompareTo((b.foodStack.foodStack.Count)); });
                        finalMachineList.Reverse();
                        foreach (FoodMachine machine in finalMachineList)
                        {
                            if (machine.foodStack.foodStack.Count > 0 && machine.employee == null)
                            {
                                machine.employee = em;
                                em.employeeState = EmployeeState.FoodMachine;
                                target = machine.workingSpot.position;
                                //em.Work(target, machine);
                                return;
                            }
                        }
                    }
                }
            }
            GUILayout.Space(3);
            if (GUILayout.Button("Employee Puts Food On The Counter"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.employeeControllers.Count; f++)
                {
                    Employee em = manager.employeeControllers[f];
                    if (em.busy == false)
                    {
                        Vector3 target;
                        List<FoodStack> stacks = new List<FoodStack>();
                        List<Counter> counterList = new List<Counter>();
                        for (int i = 0; i < workSpaceManager.counters.Count; i++)
                        {
                            //for (int j = 0; j < em.foodStacks.Count; j++)
                            {
                                for (int k = 0; k < workSpaceManager.counters[i].foodStacks.Count; k++)
                                {
                                    if (em.foodStacks[workSpaceManager.counters[i].foodStacks[k].type].foodStack.Count > 0)
                                    {
                                        counterList.Add(workSpaceManager.counters[i]);
                                    }
                                }
                            }
                        }
                        counterList.Sort(delegate (Counter a, Counter b) {

                            int aa = 0;
                            int bb = 0;
                            for (int i = 0; i < a.foodStacks.Count; i++)
                            {
                              //  for (int k = 0; k < em.foodStacks.Count; k++)
                                {
                                    if (a.foodStacks[i].type == em.foodStacks[a.foodStacks[i].type].type)
                                    {
                                        aa = i;

                                    }
                                }

                            }

                            for (int i = 0; i < b.foodStacks.Count; i++)
                            {
                         //       for (int k = 0; k < em.foodStacks.Count; k++)
                                {
                                    if (b.foodStacks[i].type == em.foodStacks[em.foodStacks[a.foodStacks[i].type].type].type)
                                    {
                                        bb = i;

                                    }
                                }

                            }
                            return (a.foodStacks[aa].foodStack.Count.CompareTo(b.foodStacks[bb].foodStack.Count));
                        });
                        foreach (Counter counter in counterList)
                        {
                            em.employeeState = EmployeeState.Counter;
                            target = counter.workingSpot_SmallTables[1].transform.position;
                            em.Work(counter);
                            return;
                        }
                    }
                }
            }
            GUILayout.Space(3);

            if (GUILayout.Button("Employee Takes Orders And Serves Food"))
            {   
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.employeeControllers.Count; f++)
                {
                    Employee em = manager.employeeControllers[f];
                    if (em.busy == false)
                    {
                        Vector3 target;

                        List<Counter> counterList = new List<Counter>();
                        for (int i = 0; i < workSpaceManager.counters.Count; i++) counterList.Add(workSpaceManager.counters[i]);
                        counterList.Sort(delegate (Counter a, Counter b) { return (a.gameObject.transform.position - em.transform.position).magnitude.CompareTo((b.gameObject.transform.position - em.transform.position).magnitude); });

                        foreach (Counter counter in counterList)
                        {
                            if (counter.customer && counter.employee == null)
                            {


                                for (int i = 0; i < counter.customer.foodStacks.Count; i++)
                                {
                                    //if (counter.customer.foodStacks[i].type == counter.foodStacks[0].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[0].foodStack.Count > 0)
                                    //{
                                    //    counter.employee = em;
                                    //    em.employeeState = EmployeeState.Serving;
                                    //    target = counter.workingSpot.position;

                                    //    em.Work(target, counter, true);
                                    //    return;
                                    //}
                                    for (int j = 0; j < counter.foodStacks.Count; j++)
                                    {
                                        if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                                        {
                                            counter.employee = em;
                                            em.employeeState = EmployeeState.Serving;
                                            target = counter.workingSpot.position;

                                            em.Work(counter);
                                            return;
                                        }

                                       
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.Space(3);

            if (GUILayout.Button("Customer Walk To Table And Eat Food"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.customerControllers.Count; f++)
                {
               
                    Customer customer = manager.customerControllers[f];
                    Vector3 target;
                    if (customer.busy == false)
                    {
                       // customer.busy = true;
                        List<Table> tableList = new List<Table>();
                        for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                        tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - customer.transform.position).magnitude.CompareTo((b.gameObject.transform.position - customer.transform.position).magnitude); });
                        foreach (Table t in tableList)
                        {
                            if (t.isDirty == false)
                            {
                                for (int i = 0; i < t.seats.Length; i++)
                                {
                                    if (t.seats[i].animal == null)
                                    {
                                        customer.busy = true;
                                        customer.customerState = CustomerState.Table;
                                        t.seats[i].animal = customer;
                                        target = t.seats[i].transform.position;
                                        customer.CustomerPlayAction(t, i);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.Space(3);


            if (GUILayout.Button("After Eating All The Food, Customer Go Home"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.customerControllers.Count; f++)
                {

                    Customer customer = manager.customerControllers[f];
          //          Vector3 target;
                    if (customer.busy == false)
                    {
                        List<EndPoint> endList = new List<EndPoint>();
                        for (int i = 0; i < workSpaceManager.endPoints.Count; i++) endList.Add(workSpaceManager.endPoints[i]);
                        endList.Sort(delegate (EndPoint a, EndPoint b) { return (a.transform.position - customer.transform.position).magnitude.CompareTo((b.transform.position - customer.transform.position).magnitude); });
                        customer.CustomerPlayAction(endList[0].transform.position);
                    }
                }
            }


            GUILayout.Space(3);
            if (GUILayout.Button("Employee Puts Trash"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.employeeControllers.Count; f++)
                {
                    Employee em = manager.employeeControllers[f];
                    if (em.busy == false)
                    {
                        Vector3 target;
                        List<Table> tableList = new List<Table>();
                        for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                        tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - em.transform.position).magnitude.CompareTo((b.gameObject.transform.position - em.transform.position).magnitude); });


                        foreach (Table table in tableList)
                        {
                            if (table.isDirty && !table.interacting && table.employeeContoller == null)
                            {
                                table.employeeContoller = em;
                                em.employeeState = EmployeeState.Table;
                                target = table.cleanSeat.position;
                            //    em.Work(target, table);
                                return;
                            }
                        }
                    }
                }
            }
            GUILayout.Space(3);
            if (GUILayout.Button("Employee Throw Trash In The Trash Can"))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

                for (int f = 0; f < manager.employeeControllers.Count; f++)
                {
                    Employee em = manager.employeeControllers[f];
                    if (em.busy == false)
                    {
               //         Vector3 target;
                        List<TrashCan> trashCanList = new List<TrashCan>();
                        for (int i = 0; i < workSpaceManager.trashCans.Count; i++) trashCanList.Add(workSpaceManager.trashCans[i]);
                        trashCanList.Sort(delegate (TrashCan a, TrashCan b) { return (a.gameObject.transform.position - em.transform.position).magnitude.CompareTo((b.gameObject.transform.position - em.transform.position).magnitude); });

                        foreach (TrashCan c in trashCanList)
                        {

                            em.Work(c);
                            return;
                        }
                    }
                }
            }
        }
    }
}
