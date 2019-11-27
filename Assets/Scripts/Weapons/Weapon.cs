using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IAddToShip
{
    public Ship FromShip {get;set;}
    [Tooltip("Despawn when this far away from the player.")]
    public float despawnDistance = 500;
    [Tooltip("Amount of time (seconds) before weapon despawns. Leave 0 for no timer.")]
    public float despawnTimer = 0;
    public float cooldownTimer;
    [Tooltip("The amount of damage this weapon does.")]
    public int damage;
    public float speed;
    public GameObject projectilePrefab;

    public bool IsInCooldown 
    {
        get => _isInCooldown; 
        set 
        {
            _isInCooldown = value;
            if (!value)
            {
                CooldownCounter = 0;
                OnCooldown();
            }
         }
    }

    ///
    /// Public float the amount of time it takes for this weapon
    /// to cooldown.
    ///
    public float CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }

    ///
    /// The amount of time this weapon has been in cooldown.
    ///
    public float CooldownCounter {get;set;}

    [System.NonSerialized]
    public int UpgradeLevel = 1;

    int _previousUpgradeLevel = 1;
    bool _isInCooldown;
    public bool Launched { get; set; }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        CheckCooldown();
        if (_previousUpgradeLevel != UpgradeLevel)
        {
            OnUpgraded(_previousUpgradeLevel, UpgradeLevel);
            _previousUpgradeLevel = UpgradeLevel;
        }
    }

    protected virtual void CheckCooldown()
    {
        if (IsInCooldown)
        {
            CooldownCounter += Time.deltaTime;
            if (CooldownCounter >= CooldownTimer)
            {
                IsInCooldown = false;
            }
        }
    }

    ///
    /// Fire the weapon.
    ///
    public virtual void FireWeapon(Ship fromShip)
    {
        if (GetCanFire())
        {
            FromShip = fromShip;
            this.IsInCooldown = true;

            GameObject projectileObj = CreateProjectile();
            // Use game instance to start, because once the ship that's using this weapon is destroyed,
            // its coroutines will stop.
            Game.Instance.StartCoroutine(ProjectileUpdate(projectileObj, fromShip.transform.position));
            Game.Instance.StartCoroutine(CheckDestroy(projectileObj, fromShip.transform.position));
            Launched = true;
        }
    }

    protected virtual GameObject CreateProjectile()
    {
        var projectileObj = GameObject.Instantiate(this.projectilePrefab, FromShip.transform.position, Quaternion.identity);
        var cb = projectileObj.AddComponent<ColliderBridge>();
        cb.HandleOnTriggerEnter = ProjectileOnTriggerEnter;
        return projectileObj;
    }

    ///
    /// Get whether the weapon can currently fire
    ///
    public virtual bool GetCanFire()
    {
        return !IsInCooldown;
    }

    ///
    /// Called every frame after a projectile is created when this weapon is fired.
    ///
    public virtual IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startingPoint)
    {
        Game.Instance.CheckWorldWrap(projectileObj.transform);
        yield return null;
    }

    ///
    /// Check if a fired projectile should be destroyed.
    ///
    public virtual IEnumerator CheckDestroy(GameObject projectileObj, Vector3 startingPoint)
    {
        float despawnTimerCounter = 0;
        while (projectileObj != null)
        {
            if (Launched)
            {
                if (Mathf.Abs(MathUtil.ToroidalDistance(startingPoint, projectileObj.transform.position).magnitude) >= despawnDistance)
                {
                    GameObject.Destroy(projectileObj);
                }

                if (despawnTimer != 0)
                {
                    despawnTimerCounter += Time.deltaTime;
                    if (despawnTimerCounter >= despawnTimer)
                    {
                        GameObject.Destroy(projectileObj);
                    }
                }
            }
            yield return null;
        }
    }

    ///
    /// Called when cooldown finishes.
    ///
    protected virtual void OnCooldown()
    {
        //
    }

    ///
    /// Called when the weapon upgrade level changes.
    ///
    protected virtual void OnUpgraded(int oldLevel, int currentLevel)
    {
        //
    }

    ///
    /// Called when a projectile fired from this weapon collides.
    ///
    protected virtual void ProjectileOnTriggerEnter(GameObject sourceObject, Collider other)
    {
        var dmg = other.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            // No damage to self...
            if (dmg.Equals(FromShip))
            {
                return;
            }
            OnDamageableCollision(sourceObject, dmg);
        }
    }

    ///
    /// Called when a projectile fired from this weapon collides with
    /// a damageable object.
    ///
    protected virtual void OnDamageableCollision(GameObject sourceObject, IDamageable damageable)
    {
        damageable.ApplyDamage(this && this.gameObject != null ? this.gameObject : null , this.damage);
        GameObject.Destroy(sourceObject);
    }

    public virtual void AddToShip(Ship ship)
    {
        //
    }
}
