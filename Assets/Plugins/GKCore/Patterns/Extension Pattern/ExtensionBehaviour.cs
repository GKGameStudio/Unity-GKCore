
using UnityEngine;

public class ExtensionBehaviour<T> : MonoBehaviour
{
    public T master{
        get{
            return GetComponent<T>();
        }
    }
}