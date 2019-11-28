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

    public enum UpgradeType : int
    {
        NoUpgrades = -1,
        Standard = 0,
        ScatterShot = 1,
        Pierce = 2,
        Missile = 3,
        Mine = 4,
        SuperLaser = 5,
        RearShot = 6,
        MaxHealth = 7,
        MaxSpeed = 8
    }

    public UpgradeType CurrentUpgradeType
    {
        get; set;
    }

    SpeedUpgrade _speedUpgrade = new SpeedUpgrade();
    HealthUpgradeable _healthUpgrade = new HealthUpgradeable();
    
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
        Game.Instance.GetGridState(Game.Instance.GetCurrentGrid()).SetExplored(true);

        if (this.warpPoints > 10)
        {
            this.warpPoints = 10;
        }

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

    class HealthUpgradeable : IUpgradeable
    {
        int _level;

        public bool CanUpgrade()
        {
            return _level < 10;
        }

        public void Upgrade()
        {
            _level++;
            Game.Instance.playerShip.maxHealth += 50;
        }

        public bool HasMaxUpgrade()
        {
            return _level == 10;
        }
    }

    class SpeedUpgrade : IUpgradeable
    {
        int _level = 1;

        public bool CanUpgrade()
        {
            return _level < 5;
        }

        public void Upgrade()
        {
            _level++;
            Game.Instance.playerShip.maxSpeed += 100;
        }

        public bool HasMaxUpgrade()
        {
            return _level == 5;
        }
    }

    public class NonUpgradeable : IUpgradeable
    {
        public bool CanUpgrade()
        {
            return false;
        }

        public bool HasMaxUpgrade()
        {
            return false;
        }

        public void Upgrade()
        {
            //
        }
    }



    ///
    /// Upgrade the ships current weapon/item.
    ///
    public IUpgradeable GetUpgradeable()
    {
        return GetUpgradeable(CurrentUpgradeType);
    }

    public IUpgradeable GetUpgradeable(UpgradeType ug)
    {
        Weapon wp = null;
        switch (ug)
        {
            case UpgradeType.NoUpgrades:
            {
                break;
            }
            case UpgradeType.Standard:
            {
                wp = this.weapons[0];
                break;
            }
            case UpgradeType.ScatterShot:
            {
                wp = (this.weapons[0] as StandardShot).spreadshotUpgrade;
                break;
            }
            case UpgradeType.Pierce:
            {
                if (this.weapons.Count > 1)
                {
                    wp = this.weapons[1];
                }
                break;
            }
            case UpgradeType.Missile:
            {
                wp = this.missileWeaponInstance;
                break;
            }
            case UpgradeType.Mine:
            {
                wp = this.minesWeaponInstance;
                break;
            }
            case UpgradeType.SuperLaser:
            {
                wp = this.superLaserInstance;
                break;
            }
            case UpgradeType.RearShot:
            {
                wp = (this.weapons[0] as StandardShot).rearShotUpgrade;
                break;
            }
            case UpgradeType.MaxHealth:
            {
                // max health upgrade
                return _healthUpgrade;
            }
            case UpgradeType.MaxSpeed:
            {
                // max speed upgrade
                return _speedUpgrade;
            }
            default:
            {
                throw new System.Exception("unknown upgrade type " + CurrentUpgradeType);
            }
        }

        if (wp != null)
        {
            return wp;
        }
        return new NonUpgradeable();
    }

    public void Upgrade(IUpgradeable upgradeable)
    {
        // full health recovery
        this.health = this.maxHealth;
        if (upgradeable.CanUpgrade())
        {
            upgradeable.Upgrade();
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
