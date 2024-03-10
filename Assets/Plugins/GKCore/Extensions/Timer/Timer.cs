using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Timer
{
    public Timer(float seconds){
        this.seconds = seconds;
    }
    //Customizable in editor
    public float seconds = 100000;

    //Customizable in script
    private Action onTimerFinished;

    //Non-changable
    private bool finished = false;
    private float currentSeconds = 10000;
    public bool started = false;
    private void Register()
    {
        TimerSystem.instance.RegisterTimer(this);
        currentSeconds = seconds;
    }
    public Timer OnFinished(Action func)
    {
        onTimerFinished = func;
        Register();
        return this;
    }
    public void Reset()
    {
        currentSeconds = seconds;
        finished = false;
        started = false;
    }
    public void Restart()
    {
        Reset();
        Start();
    }
    public Timer Start()
    {
        started = true;
        return this;
    }
    // Update is called once per frame
    public void Update()
    {
        if (!started) return;
        if (currentSeconds <= 0)
        {
            if (!finished)
            {
                finished = true;
                try
                {
                    onTimerFinished();
                }catch(Exception e)
                {
                    Debug.Log(e);
                };
            }
        }
        else
        {
            currentSeconds -= Time.deltaTime;
        }
    }
    public void Destroy()
    {
        TimerSystem.instance.DestroyTimer(this);
    }
}
