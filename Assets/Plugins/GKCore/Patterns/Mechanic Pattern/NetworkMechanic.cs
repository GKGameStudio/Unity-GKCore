using System;
using System.Collections.Generic;
using FishNet.Object;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMechanic : NetworkBehaviour
{
    [ReadOnly]
    public MechanicOwnable mechanicOwnable;
    void Awake(){
        mechanicOwnable = GetComponentInParent<MechanicOwner>();
        if(mechanicOwnable == null){
            mechanicOwnable = GetComponentInParent<NetworkMechanicOwner>();
        }
        mechanicOwnable.AddMechanic(GetType(), this);
    }
}
