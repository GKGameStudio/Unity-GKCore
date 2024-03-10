#if FISHNET
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMechanic : NetworkBehaviour
{
    public MechanicOwner mechanicOwner;
    public bool clientAuthoritative;
    public bool isSender => (IsClient && clientAuthoritative) || (IsServer && !clientAuthoritative);
    void Awake(){
        mechanicOwner = GetComponentInParent<MechanicOwner>();
        mechanicOwner.AddMechanic(GetType(), this);
    }
}
#endif