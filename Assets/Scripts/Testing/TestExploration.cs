using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// Auto explore the whole map.
///  
public class TestExploration : MonoBehaviour
{
    public float sleepTime = .01f;

    void Awake()
    {
        StartCoroutine(TestExplorationbyMovingShip());
    }
    
    private IEnumerator TestExplorationbyMovingShip()
    {
        while (!Game.Instance || !Game.Instance.IsSetup)
        {
            yield return null;
        }

        var collider = Game.Instance.playerShip.GetComponentInChildren<Collider>();
        collider.enabled = false;

        for (int i=0; i < Game.Instance.worldGridsWide * Game.Instance.worldGridsHigh; i++)
        {
            Game.Instance.playerShip.transform.position = Game.Instance.GetCenterPositionForGrid(i);
            yield return new WaitForSeconds(this.sleepTime);
        }

        collider.enabled = true;
    }
}
