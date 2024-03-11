using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    [HideInInspector]
    public MechanicOwner mechanicOwner;
    public T M<T>() where T : MonoBehaviour{
        return mechanicOwner.M<T>();
    }
    void Awake(){
        mechanicOwner = GetComponentInParent<MechanicOwner>(true);
        mechanicOwner.AddMechanic(GetType(), this);
    }
}
