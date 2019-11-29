using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpJuicePickup : MonoBehaviour, IAddToShip
{
    public int warpPoints = 1;

    public void AddToShip(Ship ship)
    {
        if (ship is PlayerShip playerShip)
        {
            playerShip.warpPoints += warpPoints;
        }
    }
    
}
