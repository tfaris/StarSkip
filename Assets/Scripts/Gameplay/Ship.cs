using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable, IAmAttacking
{
    public int health;
    public int maxHealth=100;
    public List<Weapon> weaponPrefabs;
    int _currentWeaponIndex;
    [System.NonSerialized]
    public List<Weapon> weapons;
    [System.NonSerialized]
    public HomingMissile missileWeaponInstance;
    [System.NonSerialized]
    public Mines minesWeaponInstance;
    [System.NonSerialized]
    public SuperLaser superLaserInstance;
    public Pickup drop;
    public float dropChance = .5f;
    AttackPlayer _attack;

    public GameObject destructionEffect;
    public List<AudioClip> destructionSounds, takeDamageSounds;

    public virtual GameObject AttackingThis { get => GetCurrentTarget(); }

    public bool IsActivatingSuperLaser { get; protected set; }

    protected virtual void Start()
    {
        // Any weapons that are in weaponPrefabs need to have instances
        // created.
        weapons = new List<Weapon>();
        foreach (var wpf in weaponPrefabs)
        {
            AddWeapon(wpf);
        }
        _attack = GetComponent<AttackPlayer>();
    }

    public void AddWeapon(Weapon weaponPrefab)
    {
        var inst = GameObject.Instantiate(weaponPrefab, this.transform);
        inst.AddToShip(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Game.Instance.CheckWorldWrap(this.transform);
    }

    public virtual GameObject GetCurrentTarget()
    {
        if (_attack != null && _attack.IsAttacking)
        {
            return _attack.attackThis;
        }
        return null;
    }

    ///
    /// Fire the current weapon.
    ///
    public void FireWeapon()
    {
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < weapons.Count)
        {
            // weapons is a list of prefabs
            weapons[_currentWeaponIndex].FireWeapon(this);
        }
    }

    public void CycleMainWeapon()
    {
        _currentWeaponIndex++;
        if (_currentWeaponIndex >= weapons.Count)
        {
            _currentWeaponIndex = 0;
        }
    }

    ///
    /// Get the current weapon.
    ///
    public Weapon GetCurrentWeapon()
    {
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < weapons.Count)
        {
            return weapons[_currentWeaponIndex];
        }
        return null;
    }

    [ContextMenu("Upgrade all weapons by one")]
    void BumpWeaponUpgradeLevel()
    {
        foreach (var weapon in weapons)
        {
            weapon.UpgradeLevel++;
            if (weapon is StandardShot ss)
            {
                if (ss.spreadshotUpgrade != null)
                {
                    ss.spreadshotUpgrade.UpgradeLevel++;
                }
                if (ss.rearShotUpgrade != null)
                {
                    ss.rearShotUpgrade.UpgradeLevel++;
                }
            }
        }
        if (minesWeaponInstance)
        {
            minesWeaponInstance.UpgradeLevel++;
        }
        if (missileWeaponInstance)
        {
            missileWeaponInstance.UpgradeLevel++;
        }
        if (superLaserInstance)
        {
            superLaserInstance.UpgradeLevel++;
        }
    }

    ///
    /// Apply damage to this ship.
    ///
    public virtual bool ApplyDamage(GameObject source, Collider collider, int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (!(this is PlayerShip))
            {
                // Drop on destroy
                if (drop && Random.Range(0f, 1f) < dropChance)
                {
                    GameObject.Instantiate(drop, this.transform.position, Quaternion.identity);
                }
            }
            if (Game.Instance.IsInPlayerGrid(this.gameObject))
            {
                if (destructionSounds != null && destructionSounds.Count > 0)
                {
                    Game.Instance.effectsAudioSource.PlayOneShot(destructionSounds[Random.Range(0, destructionSounds.Count)]);
                }
                if (destructionEffect != null)
                {
                    GameObject.Instantiate(destructionEffect, this.transform.position, Quaternion.identity);
                }
            }
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if (Game.Instance.IsInPlayerGrid(this.gameObject))
            {
                if (takeDamageSounds != null && takeDamageSounds.Count > 0)
                {
                    Game.Instance.effectsAudioSource.PlayOneShot(takeDamageSounds[Random.Range(0, takeDamageSounds.Count)]);
                }
            }
        }
        return true;
    }
}
