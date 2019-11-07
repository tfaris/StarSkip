﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    Rigidbody body;

    public float moveSpeed = 150, maxSpeed = 200;
    float vertForce, horzForce;
    
    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        this.vertForce = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        this.horzForce = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        transform.Rotate(0, this.horzForce, 0, Space.World);

        Vector3 movementForce = transform.forward * this.vertForce;
        body.drag = moveSpeed / maxSpeed;
        body.AddForce(
            movementForce,
            ForceMode.Acceleration
        );

        Rect bound = Game.Instance.WorldBoundaries;
        var pos = transform.position;
        if (pos.x <= bound.xMin + 1)
        {
            pos.x = bound.xMin + 1;
        }
        else if (pos.x >= bound.xMax - 1)
        {
            pos.x = bound.xMax - 1;
        }
        
        if (pos.z <= bound.yMin + 1)
        {
            pos.z = bound.yMin + 1;
        }
        else if (pos.z >= bound.yMax - 1)
        {
            pos.z = bound.yMax - 1;
        }

        if (pos != transform.position)
        {
            transform.position = pos;
            Debug.Log("leaps and !!!__BOUNDS__!!!");
        }
    }
}
