using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDisableTrigger : MonoBehaviour
{
    public UnityEvent unityEvent;
    public event System.Action<GameObject> OnDisableEvent;
    void OnDisable()
    {
        OnDisableEvent?.Invoke(gameObject);
        unityEvent?.Invoke();
    }
}
