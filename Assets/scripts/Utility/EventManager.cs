using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager
{
    public static Dictionary<int, Delegate> touchEvents = new Dictionary<int, Delegate>();


    public static void AddTouchEvent(int id, Delegate action)
    {
        if(!touchEvents.ContainsKey(id)) touchEvents[id] = action;
    }

    public static Delegate Publish(int id)
    {
        if(touchEvents.ContainsKey(id))
        {
            return touchEvents[id];
        }
        return null;
    }
    public static Delegate PublishWithDestory(int id)
    {
        if (touchEvents.ContainsKey(id))
        {
            Delegate d = touchEvents[id];
            touchEvents.Remove(id);
            return d;
        }
        return null;
    }
}
