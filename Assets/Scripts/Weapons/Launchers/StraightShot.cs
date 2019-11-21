using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// Launches weapons in a straight line, with specific speed and direction.
///
public class StraightShot : WeaponLauncher
{
    public float speed;
    [HideInInspector]
    public Vector3 direction;

    bool _hasLaunched;

    protected override void Update()
    {
        if (_hasLaunched)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public override void LaunchWeapon(Weapon weapon, Ship fromShip)
    {
        direction = fromShip.transform.forward;
        _hasLaunched = true;
    }
}
