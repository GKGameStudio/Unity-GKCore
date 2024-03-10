#if FISHNET
using System;
using System.Collections.Generic;
using System.Reflection;
using FishNet.Connection;
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

    #region Synchronizing propertys for non-network object
    public Dictionary<string, object> syncProperties = new Dictionary<string, object>();
    public double numberSyncThreshold = 0.0001;
    public void RegisterSyncProperty(string propertyName){
        syncProperties[propertyName] = GetMasterPropertyValue(propertyName);
    }
    void Update(){
        Dictionary<string, object> temp = new Dictionary<string, object>(syncProperties);
        foreach(string propertyName in temp.Keys){
            TrySyncPropertyValue(propertyName);
        }
    }
    /// <summary>
    /// Synchronize property with the same name for the non-network object
    /// </summary>
    private void TrySyncPropertyValue(string propertyName){
        // If not sender, return
        if(!isSender) return;

        // Get property info
        PropertyInfo propertyInfo = master.GetType().GetProperty(propertyName);
        if(propertyInfo == null) return;

        // Get value by property
        object currentValue = GetMasterPropertyValue(propertyName);

        // If value is not changed, return
        if(syncProperties.ContainsKey(propertyName) && syncProperties[propertyName].Equals(currentValue)){
            return;
        }

        // Check sync threshold
        if(propertyInfo.PropertyType.Name == "Single" || propertyInfo.PropertyType.Name == "Double"){
            if(Math.Abs((double)syncProperties[propertyName] - (double)currentValue) < numberSyncThreshold){
                return;
            }
        }

        // Set value to syncProperties
        syncProperties[propertyName] = currentValue;

        // Send to receiver
        if(IsServer){
            switch(propertyInfo.PropertyType.Name){
                case "Single":
                    SetMasterPropertyInClient(propertyName, (float)currentValue);
                    break;
                case "Double":
                    SetMasterPropertyInClient(propertyName, (double)currentValue);
                    break;
                case "Int32":
                    SetMasterPropertyInClient(propertyName, (int)currentValue);
                    break;
                case "Int64":
                    SetMasterPropertyInClient(propertyName, (long)currentValue);
                    break;
                case "String":
                    SetMasterPropertyInClient(propertyName, (string)currentValue);
                    break;
                case "Boolean":
                    SetMasterPropertyInClient(propertyName, (bool)currentValue);
                    break;
                default:
                    Debug.LogError("Type " + propertyInfo.PropertyType.Name + " not supported");
                    break;
            }
        }else{
            switch(propertyInfo.PropertyType.Name){
                case "Single":
                    SetMasterPropertyInServer(propertyName, (float)currentValue);
                    break;
                case "Double":
                    SetMasterPropertyInServer(propertyName, (double)currentValue);
                    break;
                case "Int32":
                    SetMasterPropertyInServer(propertyName, (int)currentValue);
                    break;
                case "Int64":
                    SetMasterPropertyInServer(propertyName, (long)currentValue);
                    break;
                case "String":
                    SetMasterPropertyInServer(propertyName, (string)currentValue);
                    break;
                case "Boolean":
                    SetMasterPropertyInServer(propertyName, (bool)currentValue);
                    break;
                default:
                    Debug.LogError("Type " + propertyInfo.PropertyType.Name + " not supported");
                    break;
            }
        }
    }
    private object GetMasterPropertyValue(string propertyName){
        PropertyInfo property = master.GetType().GetProperty(propertyName);
        if(property == null){
            Debug.LogError("Property " + propertyName + " not found in " + GetType().Name);
            return null;
        }
        return property.GetValue(master);
    }

    ///--------------------------------------------------------
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, float value){
        _SetMasterProperty(propertyName, value);
    }
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, double value){
        _SetMasterProperty(propertyName, value);
    }
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, int value){
        _SetMasterProperty(propertyName, value);
    }
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, long value){
        _SetMasterProperty(propertyName, value);
    }
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, string value){
        _SetMasterProperty(propertyName, value);
    }
    [ObserversRpc]
    private void SetMasterPropertyInClient(string propertyName, bool value){
        _SetMasterProperty(propertyName, value);
    }
    ///--------------------------------------------------------
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, object value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, float value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, double value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, int value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, long value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, string value){
        _SetMasterProperty(propertyName, value);
    }
    [ServerRpc]
    private void SetMasterPropertyInServer(string propertyName, bool value){
        _SetMasterProperty(propertyName, value);
    }
    ///--------------------------------------------------------
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, object value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, float value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, double value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, int value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, long value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, string value){
        _SetMasterProperty(propertyName, value);
    }
    [TargetRpc]
    private void SetMasterPropertyInOwner(NetworkConnection conn, string propertyName, bool value){
        _SetMasterProperty(propertyName, value);
    }



    private void _SetMasterProperty(string propertyName, object value){
        // If sending to self, return
        if(isSender){
            return;
        }
        PropertyInfo property = master.GetType().GetProperty(propertyName);
        if(property == null){
            Debug.LogError("Property " + propertyName + " not found in " + GetType().Name);
            return;
        }
        Debug.Log($"Set property {propertyName} (type: {property.PropertyType}) to {value} (type: {value.GetType()})");
        property.SetValue(master, value);
    }
    #endregion
}

#endif