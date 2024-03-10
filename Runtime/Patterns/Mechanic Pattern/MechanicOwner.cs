
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicOwner : MonoBehaviour
{
    public new GameObject gameObject => this.gameObject;
    public Dictionary<Type, MonoBehaviour> mechanics = new Dictionary<Type, MonoBehaviour>();

    public void AddMechanic(Type type, MonoBehaviour mechanic)
    {
        if(mechanics.ContainsKey(type)){
            Debug.Log("Mechanic already exists");
            return;
        }
        mechanics.Add(type, mechanic);
    }

    public T M<T>() where T : MonoBehaviour{
        return (T)mechanics[typeof(T)];
    }
}