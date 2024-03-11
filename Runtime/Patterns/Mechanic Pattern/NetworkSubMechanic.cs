#if FISHNET_V4
using System;
using System.Collections.Generic;
using System.Reflection;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using GKCore.Observers;
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