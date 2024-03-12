#if FISHNET_V4
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMechanic : NetworkBehaviour
{
    [HideInInspector]
    public MechanicOwner mechanicOwner;
    // public bool clientAuthoritative;
    public bool IsSender(SyncDirection syncDirection){
        switch(syncDirection){
            case SyncDirection.ServerToAllClient:
                return IsServerInitialized;
            case SyncDirection.ServerToOwner:
                return IsServerInitialized;
            default:
                return false;
        }
    }
    public bool IsReceiver(SyncDirection syncDirection){
        switch(syncDirection){
            case SyncDirection.ServerToAllClient:
                return IsClientInitialized;
            case SyncDirection.ServerToOwner:
                return Owner.IsLocalClient;
            default:
                return false;
        }
    }
    
}
#endif