using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// A standard projectile that travels in a straight line.
///
public class StandardShot : Weapon
{
    [System.NonSerialized]
    public SpreadShot spreadshotUpgrade;
    [System.NonSerialized]
    public RearShot rearShotUpgrade;

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startingPoint)
    {
        if (spreadshotUpgrade)
        {
            spreadshotUpgrade.FireWeapon(FromShip);
        }
        if (rearShotUpgrade)
        {
            rearShotUpgrade.FireWeapon(FromShip);
        }

        var direction = FromShip.transform.forward;
        while (projectileObj != null)
        {
            projectileObj.transform.position += direction * speed * Time.deltaTime;
            yield return base.ProjectileUpdate(projectileObj, startingPoint);
        }
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        CooldownTimer -= .15f;
    }
    
    public override void AddToShip(Ship ship)
    {
        ship.weapons.Add(this);
    }
}
