﻿using System.Collections;
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

    List<int> _usedSpecialObjectForPos = new List<int>();

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
                    //if (Game.Instance.ExplorationCount > 1)
                    {
                        Game.Instance.ShowMessage(UItext.MessageType.PirateDetected);

                        _pirateInstance = GameObject.Instantiate(piratePrefab, GetSpawnLocationForPirate(), Quaternion.identity);

                        var attack = _pirateInstance.GetComponent<AttackPlayer>();
                        var follow = _pirateInstance.GetComponent<FollowPlayer>();

                        attack.attackThis = Game.Instance.playerShip.gameObject;
                        follow.trackThisObject = Game.Instance.playerShip.gameObject;
                    }
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
        // Try to spawn the pirate at a special object first. Maybe they're robbing a planet
        // or something, etc...
        int childCount = Game.Instance.SpecialObjectsContainer.transform.childCount;
        if (_usedSpecialObjectForPos.Count < childCount)
        {
            bool foundGoodPos = false;
            int attemptCounter = 0;
            while (!foundGoodPos && attemptCounter < childCount)
            {
                int specIndex = Random.Range(0, Game.Instance.SpecialObjectsContainer.transform.childCount);
                if (!_usedSpecialObjectForPos.Contains(specIndex))
                {
                    var tr = Game.Instance.SpecialObjectsContainer.transform.GetChild(specIndex);
                    // Don't spawn pirates at enemy zones
                    if (tr.GetComponent<Zone>() == null)
                    {
                        _usedSpecialObjectForPos.Add(specIndex);
                        Vector3 specPos = tr.position;
                        specPos.y = 0;
                        return specPos;
                    }
                }
            }
        }
        // Fall back to random if there's nothing left to spawn at.
        return GetRandomPiratePos();
    }

    Vector3 GetRandomPiratePos()
    {
        bool foundGoodPos = false;
        int pirateGridPos = 0;
        int attemptCounter = 0, totalGrids = Game.Instance.worldGridsHigh * Game.Instance.worldGridsWide;
        while (!foundGoodPos && attemptCounter < totalGrids)
        {
            pirateGridPos = Random.Range(0, totalGrids);
            GridState gs = Game.Instance.GetGridState(pirateGridPos);
            bool isEnemyArea = gs.isEnemyArea;
            
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
