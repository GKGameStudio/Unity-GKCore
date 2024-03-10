using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform destiny;
    //On Trigger enter, teleport it to destiny position
    private void OnTriggerEnter(Collider other) {
        other.transform.position = destiny.position;
        if(other.GetComponent<Rigidbody>() != null){
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
