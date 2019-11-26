using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesMateyYarrr : MonoBehaviour
{
    public Ship piratePrefab;
    public List<Pickup> drops;

    Ship _pirateInstance;
    int _previousPirateGridPos = -999;
    Vector3 _previousPiratePos = Vector3.zero;

    List<int> _previousSpawnLocations = new List<int>();

    void Update()
    {
        if (Game.Instance.playerShip)
        {
            if (Game.Instance != null && Game.Instance.IsSetup && _pirateInstance == null)
            {
                if (_previousPiratePos != Vector3.zero)
                {
                    // A pirate died. Drop a thing.
                    Drop();
                }

                if (drops.Count > 0)
                {
                    _pirateInstance = GameObject.Instantiate(piratePrefab, GetSpawnLocationForPirate(), Quaternion.identity);

                    var attack = _pirateInstance.GetComponent<AttackPlayer>();
                    var follow = _pirateInstance.GetComponent<FollowPlayer>();

                    attack.attackThis = Game.Instance.playerShip.gameObject;
                    follow.trackThisObject = Game.Instance.playerShip.gameObject;
                }
                else
                {
                    // Nothing left for pirates to do. Off the plank...
                    GameObject.Destroy(this);
                }
            }
            else if (_pirateInstance != null)
            {
                // Update highlighted map grid position
                int pirateGridPos = Game.Instance.GetGrid(_pirateInstance.transform.position);
                if (pirateGridPos != _previousPirateGridPos)
                {
                    GridState newGs = Game.Instance.GetGridState(pirateGridPos);
                    newGs.hasQuestObjective = true;

                    GridState oldGs = Game.Instance.GetGridState(_previousPirateGridPos);
                    oldGs.hasQuestObjective = false;

                    _previousPirateGridPos = pirateGridPos;
                }
                _previousPiratePos = _pirateInstance.transform.position;
            }
        }
    }

    Vector3 GetSpawnLocationForPirate()
    {
        bool foundGoodPos = false;
        int pirateGridPos = 0;
        int attemptCounter = 0, totalGrids = Game.Instance.worldGridsHigh * Game.Instance.worldGridsWide;
        while (!foundGoodPos && attemptCounter < totalGrids)
        {
            pirateGridPos = Random.Range(0, totalGrids);
            // TODO: We should use grid state to check if this is an enemy area.
            bool isEnemyArea = false;
            GridState gs = Game.Instance.GetGridState(pirateGridPos);
            
            bool hasSpawnedHereAlready = _previousSpawnLocations.Contains(pirateGridPos);
            if (!isEnemyArea && !hasSpawnedHereAlready)
            {
                _previousSpawnLocations.Add(pirateGridPos);
                foundGoodPos = true;
            }
            attemptCounter++;
        }
        return Game.Instance.GetCenterPositionForGrid(pirateGridPos);
    }

    void Drop()
    {
        if (drops.Count > 0)
        {
            Pickup d = drops[0];
            var dropObj =  GameObject.Instantiate(d, _previousPiratePos, Quaternion.identity);
            drops.RemoveAt(0);
        }
    }
}
