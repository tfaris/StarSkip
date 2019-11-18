﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    Rigidbody body;

    public float moveSpeed = 150, rotateSpeed = 50, maxSpeed = 200;
    float vertForce, horzForce;
    
    // Start is called before the first frame update
    void Start()
    {
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Game.Instance.GetGridState(Game.Instance.GetCurrentGrid()).explored = true;

        this.vertForce = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        this.horzForce = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

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
            this.vertForce = 0;
            body.velocity = Vector3.zero;

            var rot = body.transform.rotation;
            rot.x = rot.z = 0;
            body.transform.rotation = rot;
            Debug.Log("BOUNDS");
        }
    }

    void FixedUpdate()
    {
        Vector3 movementForce = transform.forward * this.vertForce;
        body.drag = moveSpeed / maxSpeed;
        body.AddForce(
            movementForce,
            ForceMode.Acceleration
        );
        
        var delta = Vector3.up * this.horzForce - body.angularVelocity;
        body.AddRelativeTorque(delta);
    }
}
