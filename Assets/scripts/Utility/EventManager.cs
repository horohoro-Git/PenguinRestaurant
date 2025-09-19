using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EventManager
{
    public static Dictionary<int, Delegate> touchEvents = new Dictionary<int, Delegate>();
    public static Dictionary<TutorialEventKey, Delegate> tutorialEvents = new();

    public static void AddTouchEvent(int id, Delegate action)
    {
        if(!touchEvents.ContainsKey(id)) touchEvents[id] = action;

    }
    public static void RemoveTouchEvent(int id)
    {
        if(touchEvents.ContainsKey(id)) touchEvents.Remove(id);
    }
    public static void AddTutorialEvent(TutorialEventKey id, Delegate action)
    {
        if(!tutorialEvents.ContainsKey(id)) tutorialEvents[id] = action;
       // if (!tutorialEvents.ContainsKey(id)) tutorialEvents[id] = action;
    }
    public static void RemoveTutorialEvent(TutorialEventKey id)
    {
        if (tutorialEvents.ContainsKey(id)) tutorialEvents.Remove(id);
        // if (!tutorialEvents.ContainsKey(id)) tutorialEvents[id] = action;
    }
    public static Delegate Publish(TutorialEventKey tutorialEventKey)
    {
        if(tutorialEvents.ContainsKey(tutorialEventKey)) return tutorialEvents[tutorialEventKey];
        return null;
    }
    public static Delegate Publish(int id, bool tutorial = false)
    {
        if (!tutorial)
        {
            if (touchEvents.ContainsKey(id)) return touchEvents[id];
        }
        else
        {
         //   return tutorialEvent;
            //tutorialEvent = action;
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
