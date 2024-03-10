using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class IgnoreParentRotation : MonoBehaviour
{
    public Quaternion customRotation = Quaternion.identity;
    void Update()
    {
        transform.rotation = customRotation; //transforming the rotation each update to that rotation
    }
    void LateUpdate()
    {
        transform.rotation = customRotation; //transforming the rotation each update to that rotation
    }
}
