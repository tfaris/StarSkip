using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable, IAmAttacking
{
    public int health;
    public List<Weapon> weaponPrefabs;
    public HomingMissile missileWeaponPrefab;
    public Mines minesWeaponPrefab;
    int _currentWeaponIndex;
    List<Weapon> _weapons;
    protected HomingMissile missileWeaponInstance;
    protected Mines minesWeaponInstance;
    AttackPlayer _attack;

    public virtual GameObject AttackingThis { get => GetCurrentTarget(); }

    protected virtual void Start()
    {
        // Any weapons that are in weaponPrefabs need to have instances
        // created.
        _weapons = new List<Weapon>();
        foreach (var wpf in weaponPrefabs)
        {
            _weapons.Add(GameObject.Instantiate(wpf, this.transform));
        }
        if (missileWeaponPrefab)
        {
            missileWeaponInstance = GameObject.Instantiate(missileWeaponPrefab, this.transform);
        }
        if (minesWeaponPrefab)
        {
            minesWeaponInstance = GameObject.Instantiate(minesWeaponPrefab, this.transform);
        }
        _attack = GetComponent<AttackPlayer>();
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
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < _weapons.Count)
        {
            // weapons is a list of prefabs
            _weapons[_currentWeaponIndex].FireWeapon(this);
        }
    }

    ///
    /// Get the current weapon.
    ///
    public Weapon GetCurrentWeapon()
    {
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < _weapons.Count)
        {
            return _weapons[_currentWeaponIndex];
        }
        return null;
    }

    [ContextMenu("Upgrade all weapons by one")]
    void BumpWeaponUpgradeLevel()
    {
        foreach (var weapon in _weapons)
        {
            weapon.UpgradeLevel++;
        }
        if (minesWeaponInstance)
        {
            minesWeaponInstance.UpgradeLevel++;
        }
        if (missileWeaponInstance)
        {
            missileWeaponInstance.UpgradeLevel++;
        }
    }

    ///
    /// Apply damage to this ship.
    ///
    public virtual void ApplyDamage(GameObject source, int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
