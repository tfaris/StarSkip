using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UItext : MonoBehaviour
{

    public Dictionary<MessageType, string> textForPlayer = new Dictionary<MessageType, string>();

    public List<string> upgradeNames = new List<string>();

    public enum MessageType : int
    {
        Tut1,
        Tut2,
        Tut3,
        PirateDetected,
        WarpTut1,
        WarpTut2,
        RankUp,
        
        WeaponAcquired,
        StandarShotPassiveUpgrade,

        NewArea,
        BigBad,

        GameOverLoss,
        GameOverWin,
    }


    void Start()
    {
        //On spawn:
        textForPlayer.Add(MessageType.Tut1, "Fire with RT, Left Click, or Spacebar.");
        //Moving to second grid space
        textForPlayer.Add(MessageType.Tut2, "Explore to receive upgrades");
        //Moving to third grid space
        textForPlayer.Add(MessageType.Tut3, "Use d-left and d-right, or Z and C, to choose which part of your ship will receive upgrades as you explore");
        //Moving to fourth grid space/whenever a pirate spawns
        textForPlayer.Add(MessageType.PirateDetected, "Pirate detected. Check map (Select, or M)");

        //Picking up first warp token
        textForPlayer.Add(MessageType.WarpTut1, "Warp tokens can sometimes be collected from asteroids or enemies. The more you collect, the further you can jump.");
        textForPlayer.Add(MessageType.WarpTut2, "Press Y (controller) or J (keyboard) to warp jump.");
        //After upgrading
        textForPlayer.Add(MessageType.RankUp, "Rank Up! {0} explored");
        //Weapon picked up
        textForPlayer.Add(MessageType.WeaponAcquired, "Acquired: {0}\nUse: {1}");
        textForPlayer.Add(MessageType.StandarShotPassiveUpgrade, "{0} added to primary weapon.\nUse: Select primary weapon with {1}\nFire with {2}");
        //Finding new enemy area
        textForPlayer.Add(MessageType.NewArea, "New area detected");
        //After completing all three areas
        textForPlayer.Add(MessageType.BigBad, "Massive energy surge detected! Check map");

        //Game Over
        textForPlayer.Add(MessageType.GameOverLoss, "Game Over!\nRank: {0}");
        //Victory
        textForPlayer.Add(MessageType.GameOverWin, "Thanks for playing!\nRank: {0}");

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

    public string GetMessage(MessageType messageType, params string[] formatArgs)
    {
        string msg = this.textForPlayer[messageType];
        if (formatArgs.Length > 0)
        {
            msg = string.Format(msg, formatArgs);
        }
        return msg;
    }

}
