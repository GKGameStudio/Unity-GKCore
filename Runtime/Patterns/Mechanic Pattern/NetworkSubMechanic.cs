#if FISHNET
using System;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkSubMechanic<T> : NetworkMechanic
{
    private T _master;
    public T master{
        get{
            if(_master == null){
                _master = GetComponent<T>();
            }
            return _master;
        }
        private set{
            _master = value;
        }
    }
}

#endif