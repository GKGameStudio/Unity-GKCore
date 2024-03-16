#if FISHNET_V4
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using GKCore.Observers;
using HarmonyLib;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
public enum NetworkMethodType{
    Both,
    ServerOnly,
    ClientOnly,
    ServerRpc,
    ClientRpc,
    TargetRpc
}
public class NetworkMethodInfo{
    public MethodInfo methodInfo;
    public string methodSignatureText;
    public NetworkMethodType NetworkMethodType;
    public NetworkMethodInfo(MethodInfo methodInfo, NetworkMethodType NetworkMethodType){
        this.methodInfo = methodInfo;
        this.NetworkMethodType = NetworkMethodType;
        methodSignatureText = GKUtils.GetMethodSignatureText(methodInfo);
    }
    public override string ToString(){
        return methodSignatureText;
    }
}
public class NetworkSubMechanic<T> : NetworkMechanic where T : Mechanic
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
    public List<NetworkMethodInfo> networkMethodInfos = new List<NetworkMethodInfo>();
    public static HashSet<Type> patchedSet = new HashSet<Type>();

    public static Dictionary<Mechanic, NetworkMechanic> integratedNetworkSubMechanic = new Dictionary<Mechanic, NetworkMechanic>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        LoadAndPatchAllNetworkMethods();
    }
    [Button]
    public void LogAllMasterMethods(){
        networkMethodInfos.ForEach((networkMethodInfo)=>{
            if(networkMethodInfo.methodInfo == null){
                Debug.LogError("Method info is null");
            }else{
                Debug.Log(networkMethodInfo.methodInfo.Name);
            }
        });
    }
    public void LoadAndPatchAllNetworkMethods(){
        LoadAllMasterMethods();
        TryPatchAllNetworkMethods();
    }
    private void LoadAllMasterMethods(){
        integratedNetworkSubMechanic.Add(master, this);
        networkMethodInfos.Clear();
        AccessTools.GetDeclaredMethods(master.GetType()).ForEach((methodInfo)=>{
            // If exist, continue
            if(networkMethodInfos.Any(x=>x.methodInfo == methodInfo)){
                return;
            }

            NetworkMethodType networkMethodType = NetworkMethodType.Both;

            // If setter, default to server only
            bool isReturningSomething = methodInfo.ReturnType != typeof(void);
            if(!isReturningSomething){
                networkMethodType = NetworkMethodType.ServerOnly;
            }

            // If is Unity Registered, like Awake, Start, Update, etc, default to Both
            if(methodInfo.Name == "Awake" || methodInfo.Name == "Start" || methodInfo.Name == "Update" || methodInfo.Name == "FixedUpdate" || methodInfo.Name == "LateUpdate" || methodInfo.Name.StartsWith("On")){
                networkMethodType = NetworkMethodType.Both;
            }

            // If found the same method info with same name & arguments in this.GetType(), 
            // follow this methodInfo attribute
            Type[] types = methodInfo.GetParameters().Select(x=>x.ParameterType).ToArray();
            MethodInfo foundMethodInfo = GetType().GetMethod(methodInfo.Name, types);
            if(foundMethodInfo != null){
                ServerAttribute serverAttribute = foundMethodInfo.GetCustomAttribute<ServerAttribute>();
                if(serverAttribute != null){
                    networkMethodType = NetworkMethodType.ServerOnly;
                }
                ClientAttribute clientAttribute = foundMethodInfo.GetCustomAttribute<ClientAttribute>();
                if(clientAttribute != null){
                    networkMethodType = NetworkMethodType.ClientOnly;
                }
                ServerRpcAttribute serverRpcAttribute = foundMethodInfo.GetCustomAttribute<ServerRpcAttribute>();
                if(serverRpcAttribute != null){
                    networkMethodType = NetworkMethodType.ServerRpc;
                }
                ObserversRpcAttribute clientRpcAttribute = foundMethodInfo.GetCustomAttribute<ObserversRpcAttribute>();
                if(clientRpcAttribute != null){
                    networkMethodType = NetworkMethodType.ClientRpc;
                }
                TargetRpcAttribute targetRpcAttribute = foundMethodInfo.GetCustomAttribute<TargetRpcAttribute>();
                if(targetRpcAttribute != null){
                    networkMethodType = NetworkMethodType.TargetRpc;
                }
            }
            if(networkMethodType != NetworkMethodType.Both){
                networkMethodInfos.Add(new NetworkMethodInfo(methodInfo, networkMethodType));
            }
        });
    }
    private void TryPatchAllNetworkMethods(){
        if(patchedSet.Contains(GetType())){
            // Already patched
            return;
        }
        var harmony = new Harmony($"com.gkcore.rpc.{GetType().Name}");
        networkMethodInfos.ForEach((networkMethodInfo)=>{
            PatchNetworkMethod(harmony, networkMethodInfo.methodInfo, networkMethodInfo.NetworkMethodType);
        });
        patchedSet.Add(GetType());
    }
    private void PatchNetworkMethod(Harmony harmony, MethodInfo methodInfo, NetworkMethodType networkMethodType){
        

        var mPrefix = SymbolExtensions.GetMethodInfo(GetSyncMethodPrefix(networkMethodType));
        // in general, add null checks here (new HarmonyMethod() does it for you too)
        
        // Debug.Log($"mOriginal: {methodInfo}, NetworkMethodType: {networkMethodType}, mPrefix: {mPrefix}");
        harmony.Patch(methodInfo, new HarmonyMethod(mPrefix));
    }
    private static System.Linq.Expressions.Expression<Action> GetSyncMethodPrefix(NetworkMethodType networkMethodType)
    {
        switch(networkMethodType){
            case NetworkMethodType.ServerOnly:
                return ()=>ServerOnlyPrefix(null, null, null);
            case NetworkMethodType.ClientOnly:
                return ()=>ClientOnlyPrefix(null, null, null);
            case NetworkMethodType.ServerRpc:
                return ()=>ServerRpcPrefix(null, null, null);
            case NetworkMethodType.ClientRpc:
                return ()=>ClientRpcPrefix(null, null, null);
            case NetworkMethodType.TargetRpc:
                return ()=>TargetRpcPrefix(null, null, null);
        };
        return ()=>Debug.Log("No prefix found");
    }
    private static void ServerOnlyPrefix(Mechanic __instance, MethodBase __originalMethod, object[] __args)
    {
        NetworkMechanic networkMechanic = integratedNetworkSubMechanic[__instance];
        // Debug.Log("ServerOnlyPrefix Triggered");
        // Debug.Log("networkMechanic.NetworkManager.IsServerStarted: " + networkMechanic.NetworkManager.IsServerStarted);

        //If is client, replace the method by redirecting it to the NetworkSubMechanic's Server Rpc
        if(!networkMechanic.NetworkManager.IsServerStarted){
            Debug.Log($"The method {__originalMethod.DeclaringType.Name}.{__originalMethod.Name} is Server Only, but the Server is not initialized");
            return;
        }
    }
    private static void ClientOnlyPrefix(Mechanic __instance, MethodBase __originalMethod, object[] __args)
    {
        NetworkMechanic networkMechanic = integratedNetworkSubMechanic[__instance];
        //If is client, replace the method by redirecting it to the NetworkSubMechanic's Server Rpc
        if(!networkMechanic.NetworkManager.IsClientOnlyStarted){
            Debug.Log($"The method {__originalMethod.DeclaringType.Name}.{__originalMethod.Name} is Client Only, but the Client is not initialized");
            return;
        }
    }
    private static void ServerRpcPrefix(Mechanic __instance, MethodBase __originalMethod, object[] __args)
    {
        NetworkMechanic networkMechanic = integratedNetworkSubMechanic[__instance];
        // Debug.Log("ServerRpcPrefix Triggered");
        // Debug.Log("networkMechanic.NetworkManager.IsServerStarted: " + networkMechanic.NetworkManager.IsServerStarted);

        //If is client, replace the method by redirecting it to the NetworkSubMechanic's Server Rpc
        if(networkMechanic.NetworkManager.IsClientOnlyStarted){
            MethodInfo targetMethod = networkMechanic.GetType().GetMethod(__originalMethod.Name, __originalMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            if(targetMethod == null){
                Debug.LogError($"Failed to find the server rpc method: {__originalMethod.Name} in {networkMechanic.GetType()}");
                return;
            }
            targetMethod.Invoke(networkMechanic, __args);
            Debug.Log($"Successfully redirected the method {__originalMethod.Name} to the server rpc in {networkMechanic.GetType()}");
            return;
        }

        // Continue to execute the method
    }

    private static void ClientRpcPrefix(Mechanic __instance, MethodBase __originalMethod, object[] __args)
    {
        NetworkMechanic networkMechanic = integratedNetworkSubMechanic[__instance];
        //If is client, replace the method by redirecting it to the NetworkSubMechanic's Server Rpc
        if(networkMechanic.NetworkManager.IsServerStarted){
            MethodInfo targetMethod = networkMechanic.GetType().GetMethod(__originalMethod.Name, __originalMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            if(targetMethod == null){
                Debug.LogError($"Failed to find the server rpc method: {__originalMethod.Name} in {networkMechanic.GetType()}");
                return;
            }
            targetMethod.Invoke(networkMechanic, __args);
            Debug.Log($"Successfully redirected the method {__originalMethod.Name} to the client rpc in {networkMechanic.GetType()}");
            return;
        }

        // Continue to execute the method
    }
    private static void TargetRpcPrefix(Mechanic __instance, MethodBase __originalMethod, object[] __args)
    {
        NetworkMechanic networkMechanic = integratedNetworkSubMechanic[__instance];
        //If is client, replace the method by redirecting it to the NetworkSubMechanic's Server Rpc
        if(networkMechanic.IsClientOnlyInitialized){
            MethodInfo targetMethod = networkMechanic.GetType().GetMethod(__originalMethod.Name, __originalMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            if(targetMethod == null){
                Debug.LogError($"Failed to find the server rpc method: {__originalMethod.Name} in {networkMechanic.GetType()}");
                return;
            }
            // Add the owner as the first argument
            object[] newArr = new object[__args.Length + 1];
            newArr[0] = networkMechanic.Owner;
            for(int i = 0 ; i < __args.Length ; i++){
                newArr[i + 1] = __args[i];
            }
            // Invoke the method
            targetMethod.Invoke(networkMechanic, __args);
            return;
        }

        // Continue to execute the method
    }
}

#endif