using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    public MechanicOwner MechanicOwner;
    void Awake(){
        MechanicOwner = GetComponentInParent<MechanicOwner>(true);
        MechanicOwner.AddMechanic(GetType(), this);
    }
}
