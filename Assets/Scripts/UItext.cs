using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UItext : MonoBehaviour
{

    public List<string> textForPlayer = new List<string>();

    public List<string> upgradeNames = new List<string>();


    void Start()
    {

        //On spawn:
        textForPlayer.Add("Fire with R");
        //Moving to second grid space
        textForPlayer.Add("Explore to receive upgrades");
        //Moving to third grid space
        textForPlayer.Add("Use d-left and d-right to choose which part of your ship will receive upgrades");
        //Moving to fifth grid space/whenever a pirate spawns
        textForPlayer.Add("New pirate detected. Check map (select)");

        //Picking up first warp token
        textForPlayer.Add("Press Y to warp jump");
        //After upgrading
        textForPlayer.Add("Rank Up! 50/100/150/... explored");
        //Weapon picked up
        textForPlayer.Add("Use X to fire Super Laser/Pierce Shot/Missile/...");
        //Finding new enemy area
        textForPlayer.Add("New area detected");
        //After completing all three areas
        textForPlayer.Add("Massive energy surge detected. Check map");

        //Game Over
        textForPlayer.Add("Game Over");
        //Victory
        textForPlayer.Add("Thanks for playing! Your rank: X");

        //

        upgradeNames.Add("Standard Shot");

        upgradeNames.Add("Spread Shot");

        upgradeNames.Add("Peirce");

        upgradeNames.Add("Missile");

        upgradeNames.Add("Mine");

        upgradeNames.Add("Super Laser");

        upgradeNames.Add("Rear Shot");

        upgradeNames.Add("Armor");

        upgradeNames.Add("Speed");

    }

}
