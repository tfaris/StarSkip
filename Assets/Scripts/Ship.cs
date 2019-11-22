using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    public int health;
    public List<Weapon> weaponPrefabs;
    int _currentWeaponIndex;
    List<Weapon> _weapons;

    protected virtual void Start()
    {
        // Any weapons that are in weaponPrefabs need to have instances
        // created.
        _weapons = new List<Weapon>();
        foreach (var wpf in weaponPrefabs)
        {
            _weapons.Add(GameObject.Instantiate(wpf, this.transform));
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        foreach (Weapon weapon in _weapons)
        {
            if (weapon.IsInCooldown)
            {
                weapon.CooldownCounter += Time.deltaTime;
                if (weapon.CooldownCounter >= weapon.CooldownTimer)
                {
                    weapon.IsInCooldown = false;
                }
            }
        }
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
