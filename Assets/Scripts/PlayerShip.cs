using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Ship
{
    Rigidbody body;

    public float moveSpeed = 150, rotateSpeed = 50, maxSpeed = 200;
    float vertForce, horzForce;
    
    protected override void Start()
    {
        base.Start();
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

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
        }
        
        var delta = Vector3.up * this.horzForce - body.angularVelocity;
        //body.AddRelativeTorque(delta);
        this.transform.Rotate(delta);
        
        // Prevent rotation on other axes. RigidBody constraints aren't doing the job.
        var rot = body.transform.rotation;
        rot.x = rot.z = 0;
        body.transform.rotation = rot;

        if(Input.GetAxis("Fire1") > 0)
        {
            FireWeapon();
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
    }

    public override void ApplyDamage(GameObject source, int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            var weapon = source.GetComponent<Weapon>();
            if (weapon != null)
            {
                Game.Instance.TrackingObject = weapon.FromShip.gameObject;
            }
            else
            {
                Game.Instance.TrackingObject = source;
            }
            GameObject.Destroy(this.gameObject);
        }
    }
}
