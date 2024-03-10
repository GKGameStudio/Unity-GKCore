using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotation : MonoBehaviour
{
    public bool useRigidbody = true;
    public bool rotationWithRespectToCurrentAxis = false;
    [ShowIf("rotationWithRespectToCurrentAxis")]
    public float rotationWithRespectToCurrentAxisSpeed = 100;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    public void Start()
    {
        if (useRigidbody)
        {
            GetComponent<Rigidbody>().angularVelocity = rotationSpeed;
        }
    }
    void Update()
    {
        if (!useRigidbody)
        {
            if(rotationWithRespectToCurrentAxis){
                transform.RotateAround(transform.position, transform.up, rotationWithRespectToCurrentAxisSpeed * Time.deltaTime);
            }
            else{
                transform.Rotate(rotationSpeed * Time.deltaTime);
            }
        }
        else {
            if(rotationWithRespectToCurrentAxis){
                GetComponent<Rigidbody>().angularVelocity = transform.up * rotationWithRespectToCurrentAxisSpeed;
            }
            else{
                GetComponent<Rigidbody>().angularVelocity = rotationSpeed;
            }
        }
    }
}
