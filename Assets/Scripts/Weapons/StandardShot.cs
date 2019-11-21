using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardShot : Weapon
{
    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        CooldownTimer -= .5f;
    }
}
