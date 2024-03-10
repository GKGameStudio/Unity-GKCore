#if FISHNET
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMechanic : NetworkBehaviour
{
    public MechanicOwner MechanicOwner;
    public bool clientAuthoritative;
    public bool isSender => (IsClient && clientAuthoritative) || (IsServer && !clientAuthoritative);
    void Awake(){
        MechanicOwner = GetComponentInParent<MechanicOwner>();
        MechanicOwner.AddMechanic(GetType(), this);
    }
}
#endif