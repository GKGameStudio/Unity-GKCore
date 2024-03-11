#if Obsolete
using System;
using System.Collections.Generic;
using System.Reflection;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GKCore.Observers;
using Unity.VisualScripting;
using UnityEngine;
public class NetworkSubMechanicOld<T> : NetworkMechanic
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

    #region Synchronizing fields for non-network object
    public Dictionary<string, object> syncProperties = new Dictionary<string, object>();
    public Dictionary<string, object> observableSyncProperties = new Dictionary<string, object>();
    public double numberSyncThreshold = 0.0001;
    #region Getter
    private FieldInfo GetMasterFieldInfo(string fieldName){
        FieldInfo fieldInfo = master.GetType().GetField(fieldName);
        return fieldInfo;
    }
    private object GetMasterFieldValue(string fieldName){
        FieldInfo fieldInfo = GetMasterFieldInfo(fieldName);
        return fieldInfo?.GetValue(master);
    }
    #endregion
    #region Register
    public void RegisterSyncVar(string fieldName){
        FieldInfo fieldInfo = GetMasterFieldInfo(fieldName);
        Debug.Log("RegisterSyncVar 1 " + fieldName + " " + fieldInfo.FieldType.Name);
        if(fieldInfo == null) return;
        Debug.Log("RegisterSyncVar 2 " + fieldName + " " + fieldInfo.FieldType.Name);
        switch(fieldInfo.FieldType.Name){
            case "Single":
            case "Double":
            case "Int32":
            case "Int64":
            case "String":
            case "Boolean":
                syncProperties[fieldName] = GetMasterFieldValue(fieldName);
                Debug.Log("RegisterSyncVar " + fieldName + " " + fieldInfo.FieldType.Name);
                break;
            default:
                Debug.LogError("Type " + fieldInfo.FieldType.Name + " not supported");
                return;
        }
    }
    public void RegisterObservableSyncVar<V>(string fieldName){
        FieldInfo field = master.GetType().GetField(fieldName);
        ObservableValue<V> observer = (ObservableValue<V>)field.GetValue(master);
        if(observer == null){
            Debug.LogError("Observable " + fieldName + " not found in " + master.GetType().Name);
            return;
        }
        observer.AddListener((oldValue, newValue) => {
            TrySyncObservableField(fieldName, newValue);
        });
    }
    public void RegisterObservableSyncList<V>(ObservableList<V> observableList, SyncList<V> syncList){
        if(isSender){
            observableList.OnChange += (ObservableListOperation op, int index, V oldItem, V newItem) => {
                Debug.Log("strListObserver.OnChange: " + op + " " + index + " " + oldItem + " " + newItem);
                switch(op){
                    case ObservableListOperation.Add:
                        syncList.Add(newItem);
                        break;
                    case ObservableListOperation.Clear:
                        syncList.Clear();
                        break;
                    case ObservableListOperation.Insert:
                        syncList.Insert(index, newItem);
                        break;
                    case ObservableListOperation.RemoveAt:
                        syncList.RemoveAt(index);
                        break;
                    case ObservableListOperation.Set:
                        if(syncList.Count == index)
                            syncList.Add(newItem);
                        else
                            syncList[index] = newItem;
                        break;
                }
            };
        }else{
            syncList.OnChange += (SyncListOperation op, int index, V oldItem, V newItem, bool asServer) => {
                Debug.Log("strSyncList.OnChange");
                switch(op){
                    case SyncListOperation.Add:
                        observableList.Add(newItem);
                        break;
                    case SyncListOperation.Clear:
                        observableList.Clear();
                        break;
                    case SyncListOperation.Insert:
                        observableList.Insert(index, newItem);
                        break;
                    case SyncListOperation.RemoveAt:
                        observableList.RemoveAt(index);
                        break;
                    case SyncListOperation.Set:
                        if(observableList.Count == index)
                            observableList.Add(newItem);
                        else
                            observableList[index] = newItem;
                        break;
                }
            };
        }
    }
    #endregion
    void Update(){
        Dictionary<string, object> temp = new Dictionary<string, object>(syncProperties);
        foreach(string fieldName in temp.Keys){
            TrySyncField(fieldName);
        }
    }
    private void TrySyncField(string fieldName){
        // If not sender, return
        if(!isSender) return;

        // Get fieldInfo info
        FieldInfo fieldInfo = master.GetType().GetField(fieldName);
        if(fieldInfo == null) return;

        // Get value by fieldInfo
        object currentValue = GetMasterFieldValue(fieldName);

        // If value is not changed, return
        if(syncProperties.ContainsKey(fieldName) && syncProperties[fieldName].Equals(currentValue)){
            return;
        }

        // Check sync threshold
        if(fieldInfo.FieldType.Name == "Single" || fieldInfo.FieldType.Name == "Double"){
            if(Math.Abs((double)syncProperties[fieldName] - (double)currentValue) < numberSyncThreshold){
                return;
            }
        }

        // Set value to syncProperties
        syncProperties[fieldName] = currentValue;

        // Send to receiver
        if(IsServer){
            switch(fieldInfo.FieldType.Name){
                case "Single":
                    SetMasterFieldInClient(fieldName, false, (float)currentValue);
                    break;
                case "Double":
                    SetMasterFieldInClient(fieldName, false, (double)currentValue);
                    break;
                case "Int32":
                    SetMasterFieldInClient(fieldName, false, (int)currentValue);
                    break;
                case "Int64":
                    SetMasterFieldInClient(fieldName, false, (long)currentValue);
                    break;
                case "String":
                    SetMasterFieldInClient(fieldName, false, (string)currentValue);
                    break;
                case "Boolean":
                    SetMasterFieldInClient(fieldName, false, (bool)currentValue);
                    break;
                default:
                    Debug.LogError("Type " + fieldInfo.FieldType.Name + " not supported");
                    break;
            }
        }else{
            switch(fieldInfo.FieldType.Name){
                case "Single":
                    SetMasterFieldInServer(fieldName, false, (float)currentValue);
                    break;
                case "Double":
                    SetMasterFieldInServer(fieldName, false, (double)currentValue);
                    break;
                case "Int32":
                    SetMasterFieldInServer(fieldName, false, (int)currentValue);
                    break;
                case "Int64":
                    SetMasterFieldInServer(fieldName, false, (long)currentValue);
                    break;
                case "String":
                    SetMasterFieldInServer(fieldName, false, (string)currentValue);
                    break;
                case "Boolean":
                    SetMasterFieldInServer(fieldName, false, (bool)currentValue);
                    break;
                default:
                    Debug.LogError("Type " + fieldInfo.FieldType.Name + " not supported");
                    break;
            }
        }
    }
    private void TrySyncObservableField(string fieldName, object observedNewValue){
        if(!isSender) return;
        if(IsServer){
            switch(observedNewValue){
                case float floatValue:
                    SetMasterFieldInClient(fieldName, true, floatValue);
                    break;
                case double doubleValue:
                    SetMasterFieldInClient(fieldName, true, doubleValue);
                    break;
                case int intValue:
                    SetMasterFieldInClient(fieldName, true, intValue);
                    break;
                case long longValue:
                    SetMasterFieldInClient(fieldName, true, longValue);
                    break;
                case string stringValue:
                    SetMasterFieldInClient(fieldName, true, stringValue);
                    break;
                case bool boolValue:
                    SetMasterFieldInClient(fieldName, true, boolValue);
                    break;
                default:
                    Debug.LogError("Type " + observedNewValue.GetType().Name + " not supported");
                    break;
            }
        }else{
            switch(observedNewValue){
                case float floatValue:
                    SetMasterFieldInServer(fieldName, true, floatValue);
                    break;
                case double doubleValue:
                    SetMasterFieldInServer(fieldName, true, doubleValue);
                    break;
                case int intValue:
                    SetMasterFieldInServer(fieldName, true, intValue);
                    break;
                case long longValue:
                    SetMasterFieldInServer(fieldName, true, longValue);
                    break;
                case string stringValue:
                    SetMasterFieldInServer(fieldName, true, stringValue);
                    break;
                case bool boolValue:
                    SetMasterFieldInServer(fieldName, true, boolValue);
                    break;
                default:
                    Debug.LogError("Type " + observedNewValue.GetType().Name + " not supported");
                    break;
            }
        }
        
    }
    #region RPCs
    ///--------------------------------------------------------
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, float value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, double value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, int value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, long value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, string value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ObserversRpc]
    private void SetMasterFieldInClient(string fieldName, bool isObserver, bool value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    ///--------------------------------------------------------
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, object value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, float value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, double value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, int value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, long value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, string value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [ServerRpc]
    private void SetMasterFieldInServer(string fieldName, bool isObserver, bool value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    ///--------------------------------------------------------
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  object value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  float value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  double value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  int value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  long value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  string value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    [TargetRpc]
    private void SetMasterFieldInOwner(NetworkConnection conn, string fieldName, bool isObserver,  bool value){
        _SetMasterSyncVar(fieldName, isObserver, value);
    }
    #endregion
    private void _SetMasterSyncVar(string varName, bool isObserver, object value){
        // If sending to self, return
        // if(isSender){
        //     return;
        // }
        FieldInfo fieldInfo = master.GetType().GetField(varName);
        if(fieldInfo == null) return;
        if(isObserver){
            var observer = fieldInfo.GetValue(master);
            observer.GetType().GetMethod("Set").Invoke(observer, new object[]{value, true});
            Debug.Log($"Set observer {varName} (type: {fieldInfo.FieldType}) to {value} (type: {value.GetType()})");
        }else{
            fieldInfo.SetValue(master, value);
            Debug.Log($"Set fieldInfo {varName} (type: {fieldInfo.FieldType}) to {value} (type: {value.GetType()})");
            return;
        }
    }
    #endregion
}

#endif