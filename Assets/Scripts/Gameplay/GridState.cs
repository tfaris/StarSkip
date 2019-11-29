using System;
using System.Collections.Generic;
using UnityEngine;

public class GridState
{
    public bool explored;
    public bool hasQuestObjective;
    public bool isEnemyArea;
    public bool isBossLocation;
    public AsteroidController asteroidSpawner;

    internal void SetExplored(bool val)
    {
        if (val != this.explored)
        {
            if (val)
            {
                Game.Instance.ExplorationCount++;
            }
            else
            {
                // This shouldn't happen...
                Game.Instance.ExplorationCount--;
            }
            this.explored = val;
        }
    }
}