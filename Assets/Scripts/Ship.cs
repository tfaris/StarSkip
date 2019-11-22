using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    public int health;
    public List<Weapon> weapons;
    int _currentWeaponIndex;

    // Update is called once per frame
    protected virtual void Update()
    {
        foreach (Weapon weapon in weapons)
        {
            if (!weapon.IsStarted)
            {
                weapon.Start();
            }
            
            weapon.Update();
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

    public void FireWeapon()
    {
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < weapons.Count)
        {
            // weapons is a list of prefabs
            weapons[_currentWeaponIndex].FireWeapon(this);
        }
    }

    [ContextMenu("Upgrade current weapon by one")]
    void BumpWeaponUpgradeLevel()
    {
        weapons[_currentWeaponIndex].UpgradeLevel++;
        Debug.Log("Upgrade level of " + weapons[_currentWeaponIndex] + " is now " + weapons[_currentWeaponIndex].UpgradeLevel);
    }

    public virtual void ApplyDamage(GameObject source, int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
