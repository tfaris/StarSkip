using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingShot : Weapon
{
    bool _piercing = true;

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startPosition)
    {
        var direction = FromShip.transform.forward;
        projectileObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        while (projectileObj != null)
        {
            projectileObj.transform.position += direction * speed * Time.deltaTime;
            yield return base.ProjectileUpdate(projectileObj, startPosition);
        }
    }

    protected override void OnDamageableCollision(GameObject sourceObject, IDamageable damageable)
    {
        damageable.ApplyDamage(this && this.gameObject != null ? this.gameObject : null , this.damage);
        if (!_piercing)
        {
            GameObject.Destroy(sourceObject);
        }
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            CooldownTimer *= .85f;
        }
        else if (newLevel == 3)
        {
            CooldownTimer *= .75f;
        }
        else if (newLevel == 4)
        {
            CooldownTimer *= .5f;
        }
        else if (newLevel == 5)
        {
            damage *= 2;
        }
    }
    public override void AddToShip(Ship ship)
    {
        ship.weapons.Add(this);
        
        if (ship is PlayerShip)
        {
            Game.Instance.ShowMessage(UItext.MessageType.WeaponAcquired, "Piercing Shot", "select with B or Left Ctrl\n. Fire with RT, Spacebar, or left click");
        }
    }
}
