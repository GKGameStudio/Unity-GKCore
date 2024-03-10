using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will store the colliding objects in a list.
public class GKCollidable : MonoBehaviour
{
    public List<Collider> collidingObjects = new List<Collider>();
    public List<Collider> CollidingObjects { get { return collidingObjects; } }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (collidingObjects.Contains(other)) return;
        collidingObjects.Add(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!collidingObjects.Contains(other)) return;
        collidingObjects.Remove(other);
    }
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (collidingObjects.Contains(other.collider)) return;
        collidingObjects.Add(other.collider);
    }
    protected virtual void OnCollisionExit(Collision other)
    {
        if (!collidingObjects.Contains(other.collider)) return;
        collidingObjects.Remove(other.collider);
    }
}
