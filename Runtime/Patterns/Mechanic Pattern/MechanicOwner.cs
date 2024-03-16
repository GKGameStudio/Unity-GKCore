
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicOwner : MonoBehaviour
{
    public Dictionary<Type, MonoBehaviour> mechanics = new Dictionary<Type, MonoBehaviour>();
    void Awake(){
        SearchMechanicsInChildren();
    }
    public void SearchMechanicsInChildren(){
        foreach (var item in GetComponentsInChildren<MonoBehaviour>(true))
        {
            Debug.Log("Found Mechanic: " + item.GetType());
            item.mechanicOwner = this;
            AddMechanic(item.GetType(), item);
        }
    }

    public void AddMechanic(Type type, MonoBehaviour mechanic)
    {
        if(mechanics.ContainsKey(type)){
            Debug.Log("Mechanic already exists");
            return;
        }
        mechanics.Add(type, mechanic);
    }
    public Mechanic GetMechanic(Type type)
    {
        if(mechanics.ContainsKey(type)){
            return (Mechanic)mechanics[type];
        }
        Debug.Log("Mechanic not found: "+type);
        return null;
    }

    public T M<T>() where T : MonoBehaviour{
        return (T)mechanics[typeof(T)];
    }
    public bool H<T>() where T : MonoBehaviour{
        return mechanics.ContainsKey(typeof(T));
    }
}
