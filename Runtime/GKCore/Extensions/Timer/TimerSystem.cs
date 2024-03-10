using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSystem : SingletonMonoBehaviour<TimerSystem>
{
    public List<Timer> timers = new List<Timer>();
    public void RegisterTimer(Timer timer)
    {
        if (!timers.Contains(timer))
        {
            timers.Add(timer);
        }
    }
    public void DestroyTimer(Timer timer)
    {
        timers.Remove(timer);
    }
    private void Update()
    {
        for(int i = 0; i < timers.Count; i++)
        {
            Timer timer = timers[i];
            timer.Update();
        }
    }
}
