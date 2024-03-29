using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingletonMonoBehaviour<T> : MonoBehaviour where T: SingletonMonoBehaviour<T>
{
    [SerializeField]
    private static T _instance;
    private static readonly object padlock = new object();
    public static T instance
    {
        get
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T), true) as T;
                    if (_instance == null)
                    {
                        return null;
                    }
                }
                return _instance;
            }
        }
        set
        {
            lock (padlock)
            {
                _instance = value;
            }
        }
    }
    public virtual void Init(){
        //Override this function to initialize the singleton
        Debug.Log("called original init..");
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
    }
}