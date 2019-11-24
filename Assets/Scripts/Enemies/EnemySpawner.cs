using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnTimer;
    public int numEnemiesAtOnce;
    float _spawnTimerCounter;
    int _index, _enemyCounter;
    List<GameObject> _spawned = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        _spawnTimerCounter += Time.deltaTime;
        if (_spawnTimerCounter >= spawnTimer && (numEnemiesAtOnce == 0 || _spawned.Count < numEnemiesAtOnce))
        {
            if (_index >= enemyPrefabs.Count)
            {
                _index = 0;
            }

            _spawnTimerCounter = 0;

            GameObject enemyObj = GameObject.Instantiate(enemyPrefabs[_index++], this.transform.position, Quaternion.identity);
            _spawned.Add(enemyObj);

            AttackPlayer ap = enemyObj.GetComponent<AttackPlayer>();
            FollowPlayer fp = enemyObj.GetComponent<FollowPlayer>();

            if (Game.Instance.playerShip != null)
            {
                if (fp != null)
                {
                    fp.trackThisObject = Game.Instance.playerShip.gameObject;
                }
                if (ap != null)
                {
                    ap.attackThis = Game.Instance.playerShip.gameObject;
                }
            }
        }

        for (int i=0; i < _spawned.Count; i++)
        {
            if (_spawned[i] == null)
            {
                _spawned.RemoveAt(i);
                i--;
            }
        }
    }
}
