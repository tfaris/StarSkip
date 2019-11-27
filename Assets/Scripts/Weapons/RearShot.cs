using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearShot : SpreadShot
{
    protected override void Start()
    {
        // Rear shot never has spread to start with.
        base.spreadDegrees = 0;
        // Rear starts with only one pellet
        base.numPellets = 1;
        base.delayBetweenPellets = .3f;
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            base.numPellets = 3;
        }
        else if (newLevel == 3)
        {
            scale = new Vector3(3, 3, 3);
            base.delayBetweenPellets = .5f;
        }
        else if (newLevel == 4)
        {
            base.delayBetweenPellets = 0;
            base.spreadDegrees = 90f;
        }
        else if (newLevel == 5)
        {
            CooldownTimer -= .5f;
            base.speed *= 1.5f;
        }
    }

    protected override Vector3 GetDirectionForPellet(int pelletIndex, GameObject pelletObj)
    {
        return -pelletObj.transform.forward;
    }

    public override void AddToShip(Ship ship)
    {
        foreach (var ss in ship.weapons.OfType<StandardShot>())
        {
            ss.rearShotUpgrade = this;
        }
    }
}
