using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MechanicOwnable
{
    public T M<T>() where T : MonoBehaviour;
    public void AddMechanic(Type type, MonoBehaviour mechanic);
}
