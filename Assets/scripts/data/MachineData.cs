using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class MachineData
{
    public int level;
    public int sale_proceeds;
    public int upgrade_cost;
    public float food_production_speed;
    public int food_production_max_height;

    public MachineData(int level, int sale_proceeds, int upgrade_cost, float food_production_speed, int food_production_max_height)
    {
        this.level = level;
        this.sale_proceeds = sale_proceeds;
        this.upgrade_cost = upgrade_cost;
        this.food_production_speed = food_production_speed;
        this.food_production_max_height = food_production_max_height;
    }
}
