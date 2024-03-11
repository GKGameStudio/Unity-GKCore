#if FISHNET_V4
using System.Reflection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GKCore.Observers;
using UnityEngine.Events;
public static class ObserverNetworkBridgeExtension{
    public static void LinkSyncVar<T>(this ObservableVar<T> observableVar, SyncVar<T> syncVar, NetworkMechanic nm){
        if(nm.isSender){
            observableVar.AddListener((oldValue, newValue) => {
                syncVar.Value = newValue;
            });
        }else{
            syncVar.OnChange += (oldValue, newValue, asServer) => {
                observableVar.Value = newValue;
            };
        }
    }
    public static void LinkSyncList<T>(this ObservableList<T> observableList, SyncList<T> syncList, NetworkMechanic nm){
        if(nm.isSender){
            observableList.OnChange += (ObservableListOperation op, int index, T oldItem, T newItem) => {
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
                        syncList[index] = newItem;
                        break;
                }
            };
        }else{
            syncList.OnChange += (SyncListOperation op, int index, T oldItem, T newItem, bool asServer) => {
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
                        observableList[index] = newItem;
                        break;
                }
            };
        }
    }
    public static void LinkSyncDictionary<TKey, TValue>(this ObservableDictionary<TKey, TValue> observableDictionary, SyncDictionary<TKey, TValue> syncDictionary, NetworkMechanic nm){
        if(nm.isSender){
            observableDictionary.OnChange += (ObservableDictionaryOperation op, TKey key, TValue oldValue, TValue newValue) => {
                switch(op){
                    case ObservableDictionaryOperation.Add:
                        syncDictionary.Add(key, newValue);
                        break;
                    case ObservableDictionaryOperation.Clear:
                        syncDictionary.Clear();
                        break;
                    case ObservableDictionaryOperation.Remove:
                        syncDictionary.Remove(key);
                        break;
                    case ObservableDictionaryOperation.Set:
                        syncDictionary[key] = newValue;
                        break;
                }
            };
        }else{
            syncDictionary.OnChange += (SyncDictionaryOperation op, TKey key, TValue newValue, bool asServer) => {
                switch(op){
                    case SyncDictionaryOperation.Add:
                        observableDictionary.Add(key, newValue);
                        break;
                    case SyncDictionaryOperation.Clear:
                        observableDictionary.Clear();
                        break;
                    case SyncDictionaryOperation.Remove:
                        observableDictionary.Remove(key);
                        break;
                    case SyncDictionaryOperation.Set:
                        observableDictionary[key] = newValue;
                        break;
                }
            };
        }
    }
}

#endif