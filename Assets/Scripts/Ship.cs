using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable, IAmAttacking
{
    public int health;
    public List<Weapon> weaponPrefabs;
    public HomingMissile missileWeaponPrefab;
    int _currentWeaponIndex;
    List<Weapon> _weapons;
    protected HomingMissile missileWeaponInstance;
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

    [ContextMenu("Upgrade current weapon by one")]
    void BumpWeaponUpgradeLevel()
    {
        _weapons[_currentWeaponIndex].UpgradeLevel++;
        Debug.Log("Upgrade level of " + _weapons[_currentWeaponIndex] + " is now " + _weapons[_currentWeaponIndex].UpgradeLevel);
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
