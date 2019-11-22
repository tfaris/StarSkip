using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    public delegate void OnTriggerEnterFunc(GameObject sourceObject, Collider c);

    [System.NonSerialized]
    [HideInInspector]
    public OnTriggerEnterFunc HandleOnTriggerEnter;

    void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(this.gameObject, other);
    }
}
