using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : Ship
{   
    public float moveSpeed = 150, rotateSpeed = 50, maxSpeed = 200;
    public int warpPoints;
    float vertForce, horzForce;
    
    Rigidbody body;
    WarpJump _jump;
    
    protected override void Start()
    {
        base.Start();
        body = this.GetComponent<Rigidbody>();
        _jump = GetComponent<WarpJump>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Game.Instance.GetGridState(Game.Instance.GetCurrentGrid()).explored = true;

        if (!_jump.IsJumping)
        {
            this.vertForce = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            this.horzForce = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
            
            var delta = Vector3.up * this.horzForce - body.angularVelocity;
            this.transform.Rotate(delta);
            
            // Prevent rotation on other axes. RigidBody constraints aren't doing the job.
            var rot = body.transform.rotation;
            rot.x = rot.z = 0;
            body.transform.rotation = rot;

            if(Input.GetAxis("Fire Main Weapon") > 0)
            {
                FireWeapon();
            }
            if (Input.GetAxis("Fire Missile") > 0)
            {
                if (missileWeaponInstance)
                {
                    missileWeaponInstance.FireWeapon(this);
                }
            }
            if (Input.GetAxis("Lay Mine") > 0)
            {
                if (minesWeaponInstance)
                {
                    minesWeaponInstance.FireWeapon(this);
                }
            }
            if (Input.GetButtonUp("Swap Main Weapon"))
            {
                CycleMainWeapon();
            }
            if (Input.GetAxis("Fire Laser") > 0)
            {
                IsActivatingSuperLaser = true;
                if (superLaserInstance)
                {
                    superLaserInstance.FireWeapon(this);
                }
            }
            else
            {
                IsActivatingSuperLaser = false;
            }
            if (!_jump.IsJumping && Input.GetButtonUp("Warp Jump"))
            {
                _jump.jumpSpaces = this.warpPoints * _jump.spacesPerWarpPoint;
                if (_jump.jumpSpaces > 0)
                {
                    this.warpPoints = 0;
                    StartCoroutine(_jump.Warp());
                }
                else
                {
                    // bzzz....
                }
            }
        }
        else
        {
            body.velocity = Vector3.zero;
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
            if (source != null)
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
            }
            GameObject.Destroy(this.gameObject);
        }
    }
}
