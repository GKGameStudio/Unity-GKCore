using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    public MechanicOwner mechanicOwner;
    void Awake(){
        mechanicOwner = GetComponentInParent<MechanicOwner>(true);
        mechanicOwner.AddMechanic(GetType(), this);
    }
}
