using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetWaitTimer
{
    static GetWaitTimer waitTimer = new GetWaitTimer();
    Dictionary<int, WaitForSeconds> waitForSeconds = new Dictionary<int, WaitForSeconds>();
    public static GetWaitTimer WaitTimer { get { return waitTimer; }}
    public GetWaitTimer()
    {
       /* for (int i = 100; i <= 5000; i += 100)
        {
            waitForSeconds[i] = new WaitForSeconds(i / 1000f);
        }*/
    }


    public WaitForSeconds GetTimer(int time)
    {
        if (!waitForSeconds.TryGetValue(time, out var waitForSecondsObj))
        {
            // 예외를 던지지 않고, 필요한 경우 생성하여 반환
            waitForSecondsObj = new WaitForSeconds(time / 1000f);
        }
        return waitForSeconds[time];
    }
}
