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

    WeaponLauncher _launcher;
    int _previousUpgradeLevel = 1;
    bool _launched, _isInCooldown;
    float _coolDownTimerTmp;
    [System.NonSerialized]
    bool _isStarted;

    public bool IsStarted {get=>_isStarted;set => _isStarted = value;}
    
    // Start is called before the first frame update
    public void Start()
    {
        _launcher = GetComponent<WeaponLauncher>();
        _coolDownTimerTmp = cooldownTimer;
        IsStarted = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        CheckDestroy();

        if (_previousUpgradeLevel != UpgradeLevel)
        {
            OnUpgraded(_previousUpgradeLevel, UpgradeLevel);
            _previousUpgradeLevel = UpgradeLevel;
        }
    }

    public virtual void CheckDestroy()
    {
        if (_launched)
        {
            if (FromShip != null)
            {
                if (Mathf.Abs((FromShip.transform.position - this.transform.position).magnitude) >= despawnDistance)
                {
                    GameObject.Destroy(this.gameObject);
                }
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }

    public virtual void FireWeapon(Ship fromShip)
    {
        if (!IsInCooldown)
        {
            this.IsInCooldown = true;
            var weapon = GameObject.Instantiate(this, fromShip.transform.position, Quaternion.identity);
            var launcher = weapon.GetComponent<WeaponLauncher>();
            weapon.FromShip = fromShip;
            launcher.LaunchWeapon(weapon, fromShip);
            weapon._launched = true;
        }
    }

    protected virtual void OnCooldown()
    {
        //
    }

    protected virtual void OnUpgraded(int oldLevel, int currentLevel)
    {
        //
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        var dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            OnDamageableCollision(dmg);
        }
    }

    protected virtual void OnDamageableCollision(IDamageable damageable)
    {
        damageable.ApplyDamage(this.gameObject, this.damage);
        GameObject.Destroy(this.gameObject);
    }
}
