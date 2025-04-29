using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글


[System.Serializable]
public class PlayData
{
    public int id;
    public int type;
    public bool unlock;
/*
    int id2;
    int lvl;


    int l;
    int upgrade_cost;
    int sale_proceeds;
*/


//    public int level;
  //  public int money;

    public PlayData(int id, int type, bool unlock)
    {
        this.id = id;
        this.type = type;
        this.unlock = unlock;
      //  this.level = level;
      //  this.money = money; 
    }
}
