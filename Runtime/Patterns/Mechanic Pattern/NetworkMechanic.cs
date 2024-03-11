#if FISHNET_V4
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkMechanic : NetworkBehaviour
{
    [HideInInspector]
    public MechanicOwner mechanicOwner;
    public bool clientAuthoritative;
    public bool isSender => (IsClientOnlyInitialized && clientAuthoritative) || (IsServerInitialized && !clientAuthoritative);
    
}
#endif