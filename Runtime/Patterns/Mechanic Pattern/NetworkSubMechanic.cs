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

    // public void RegisterObservableSyncVar<V>(ObservableValue<V> observableVar, SyncVar<V> syncVar){
    //     if(isSender){
    //         observableVar.AddListener((V oldValue, V newValue) => {
    //             Debug.Log("strObserver.OnChange: " + oldValue + " " + newValue);
    //             syncVar.Value = newValue;
    //         });
    //     }else{
    //         syncVar.OnChange += (V oldValue, V newValue, bool asServer) => {
    //             Debug.Log("strSyncVar.OnChange: " + oldValue + " " + newValue);
    //             observableVar.Value = newValue;
    //         };
    //     }
    // }
    // public void Test<V>(SyncVar<V> syncVar, V value){
    //     syncVar.Value = value;
    // }
    #if FISHNET_V3
    #region Register
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
    public void RegisterObservableSyncDictionary<K, V>(ObservableDictionary<K, V> observableDict, SyncDictionary<K, V> syncDict){
        if(isSender){
            observableDict.OnChange += (ObservableDictionaryOperation op, K key, V oldValue, V newValue) => {
                Debug.Log("strDictObserver.OnChange: " + op + " " + key + " " + oldValue + " " + newValue);
                switch(op){
                    case ObservableDictionaryOperation.Add:
                        syncDict.Add(key, newValue);
                        break;
                    case ObservableDictionaryOperation.Clear:
                        syncDict.Clear();
                        break;
                    case ObservableDictionaryOperation.Remove:
                        syncDict.Remove(key);
                        break;
                    case ObservableDictionaryOperation.Set:
                        syncDict[key] = newValue;
                        break;
                }
            };
        }else{
            syncDict.OnChange += (SyncDictionaryOperation op, K key, V value, bool asServer) => {
                Debug.Log("strSyncDict.OnChange: " + op + " " + key + " " + value);
                switch(op){
                    case SyncDictionaryOperation.Add:
                        observableDict.Add(key, value);
                        break;
                    case SyncDictionaryOperation.Clear:
                        observableDict.Clear();
                        break;
                    case SyncDictionaryOperation.Remove:
                        observableDict.Remove(key);
                        break;
                    case SyncDictionaryOperation.Set:
                        observableDict[key] = value;
                        break;
                }
            };
        }
    }
    #endregion
    #endif
}

#endif