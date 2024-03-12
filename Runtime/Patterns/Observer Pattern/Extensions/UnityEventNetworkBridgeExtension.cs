#if FISHNET_V4
using System.Reflection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GKCore.Observers;
using UnityEngine;
using UnityEngine.Events;
public static class UnityEventNetworkBridgeExtension{
    public static void LinkSyncEvent<T0>(this UnityEvent<T0> unityEvent, NetworkMechanic nm, SyncEvent<T0> syncEvent, SyncDirection syncDirection = SyncDirection.ServerToAllClient){
        Debug.Log("Linking SyncEvent, IsSender(syncDirection): " + nm.IsSender(syncDirection));
        if(nm.IsSender(syncDirection)){
            unityEvent.AddListener((t0) => {
                Debug.Log("Server Invoking syncEvent 1");
                syncEvent.Invoke(t0);
                Debug.Log("Server Invoking syncEvent 1");
            });
        }else if(nm.IsReceiver(syncDirection)){
            Debug.Log("Registering Listener for SyncEvent");
            syncEvent.AddListener((t0)=>{
                Debug.Log("Client Received Invoking UnityAction AAAAAAAAA");
                unityEvent?.Invoke(t0);
            });
        }
    }
    // public static void LinkSyncEvent<T0, T1>(this UnityAction<T0, T1> unityAction, NetworkMechanic nm, SyncEvent<T0, T1> syncEvent){
    //     if(nm.IsSender(syncDirection)){
    //         unityAction += (t0, t1) => {
    //             syncEvent.Invoke(t0, t1);
    //         };
    //     }else if(nm.isReceiver){
    //         syncEvent.AddListener((t0, t1)=>{
    //             unityAction?.Invoke(t0, t1);
    //         });
    //     }
    // }
    // public static void LinkSyncEvent<T0, T1, T2>(this UnityAction<T0, T1, T2> unityAction, NetworkMechanic nm, SyncEvent<T0, T1, T2> syncEvent){
    //     if(nm.IsSender(syncDirection)){
    //         unityAction += (t0, t1, t2) => {
    //             syncEvent.Invoke(t0, t1, t2);
    //         };
    //     }else if(nm.isReceiver){
    //         syncEvent.AddListener((t0, t1, t2)=>{
    //             unityAction?.Invoke(t0, t1, t2);
    //         });
    //     }
    // }
    // public static void LinkSyncEvent<T0, T1, T2, T3>(this UnityAction<T0, T1, T2, T3> unityAction, NetworkMechanic nm, SyncEvent<T0, T1, T2, T3> syncEvent){
    //     if(nm.IsSender(syncDirection)){
    //         unityAction += (t0, t1, t2, t3) => {
    //             syncEvent.Invoke(t0, t1, t2, t3);
    //         };
    //     }else if(nm.isReceiver){
    //         syncEvent.AddListener((t0, t1, t2, t3)=>{
    //             unityAction?.Invoke(t0, t1, t2, t3);
    //         });
    //     }
    // }

}

#endif