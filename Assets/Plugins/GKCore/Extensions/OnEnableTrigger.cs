using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableTrigger : MonoBehaviour
{
    public UnityEvent unityEvent;
    public event System.Action<GameObject> OnEnableEvent;
    void OnEnable()
    {
        OnEnableEvent?.Invoke(gameObject);
        unityEvent?.Invoke();
    }
}
