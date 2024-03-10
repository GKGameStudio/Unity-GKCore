using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    public LayerMask layerMask;
    public UnityAction onTriggerEnter;
    public UnityAction onTriggerExit;
    public UnityAction onTriggerStay;
    public UnityAction onCollisionEnter;
    public UnityAction onCollisionExit;
    public UnityAction onCollisionStay;
    private void OnTriggerEnter(Collider other)
    {
        //Return if other collider layer is not not in layer mask
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        
        onTriggerEnter.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        onTriggerExit.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        onTriggerStay.Invoke();
    }
        
    private void OnCollisionEnter(Collision other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        onCollisionEnter.Invoke();
    }
    private void OnCollisionExit(Collision other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        onCollisionExit.Invoke();
    }
    private void OnCollisionStay(Collision other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) == 0)
        {
            return;
        }
        onCollisionStay.Invoke();
    }


}
