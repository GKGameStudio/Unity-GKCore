using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EventState{
    public bool cancelled = false;
    public void SetCancelled(bool cancelled){
        this.cancelled = cancelled;
    }
}
