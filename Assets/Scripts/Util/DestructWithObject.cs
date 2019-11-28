using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructWithObject : MonoBehaviour
{
    public GameObject other;

    void Update()
    {
        if (!other)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
