using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// A standard projectile that travels in a straight line.
///
public class StandardShot : Weapon
{    
    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startingPoint)
    {
        var direction = FromShip.transform.forward;
        while (projectileObj != null)
        {
            projectileObj.transform.position += direction * speed * Time.deltaTime;
            yield return base.ProjectileUpdate(projectileObj, startingPoint);
        }
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        CooldownTimer -= .5f;
    }
}
