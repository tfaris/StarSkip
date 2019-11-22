using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Ship FromShip {get;set;}
    [Tooltip("Despawn when this far away from the player.")]
    public float despawnDistance = 500;
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
    public float CooldownTimer { get => _coolDownTimerTmp; set => _coolDownTimerTmp = value; }

    ///
    /// The amount of time this weapon has been in cooldown.
    ///
    public float CooldownCounter {get;set;}

    [System.NonSerialized]
    public int UpgradeLevel = 1;

    int _previousUpgradeLevel = 1;
    bool _isInCooldown;
    float _coolDownTimerTmp;
    public bool Launched { get; set; }
    
    // Start is called before the first frame update
    public void Start()
    {
        _coolDownTimerTmp = cooldownTimer;
    }

    void Update()
    {
        if (IsInCooldown)
        {
            CooldownCounter += Time.deltaTime;
            if (CooldownCounter >= CooldownTimer)
            {
                IsInCooldown = false;
            }
        }
        if (_previousUpgradeLevel != UpgradeLevel)
        {
            OnUpgraded(_previousUpgradeLevel, UpgradeLevel);
            _previousUpgradeLevel = UpgradeLevel;
        }
    }

    ///
    /// Fire the weapon.
    ///
    public virtual void FireWeapon(Ship fromShip)
    {
        if (!IsInCooldown)
        {
            FromShip = fromShip;
            this.IsInCooldown = true;
            var projectileObj = GameObject.Instantiate(this.projectilePrefab, fromShip.transform.position, Quaternion.identity);
            var cb = projectileObj.AddComponent<ColliderBridge>();
            cb.HandleOnTriggerEnter = ProjectileOnTriggerEnter;

            // Use game instance to start, because once the ship that's using this weapon is destroyed,
            // its coroutines will stop.
            Game.Instance.StartCoroutine(ProjectileUpdate(projectileObj, fromShip.transform.position));
            Launched = true;
        }
    }

    ///
    /// Called every frame after a projectile is created when this weapon is fired.
    ///
    public virtual IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startingPoint)
    {
        CheckDestroy(projectileObj, startingPoint);
        yield return null;
    }

    ///
    /// Check if a fired projectile should be destroyed.
    ///
    public virtual void CheckDestroy(GameObject projectileObj, Vector3 startingPoint)
    {
        if (Launched)
        {
            if (Mathf.Abs((startingPoint - projectileObj.transform.position).magnitude) >= despawnDistance)
            {
                GameObject.Destroy(projectileObj);
            }
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
        damageable.ApplyDamage(this.gameObject, this.damage);
        GameObject.Destroy(sourceObject);
    }
}
