using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    [ReadOnly]
    public MechanicOwnable mechanicOwnable;
    void Awake(){
        mechanicOwnable = GetComponentInParent<MechanicOwner>(true);
        #if FISHNET
        if(mechanicOwnable == null){
            mechanicOwnable = GetComponentInParent<NetworkMechanicOwner>(true);
        }
        #endif
        mechanicOwnable.AddMechanic(GetType(), this);
    }
}
