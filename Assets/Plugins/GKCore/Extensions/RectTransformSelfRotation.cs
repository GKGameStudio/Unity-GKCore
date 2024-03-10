using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformSelfRotation : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float rotationOffset = 0f;
    public bool isClockwise = true;
    public void Update()
    {
        //Apply rotation for rect transform
        transform.Rotate(0, 0, rotationSpeed * (isClockwise ? 1 : -1) * Time.deltaTime, Space.Self);
    }
}
