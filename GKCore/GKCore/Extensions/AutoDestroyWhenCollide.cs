using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyWhenCollide : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
