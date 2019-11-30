using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnTimer;
    public int numEnemiesAtOnce;
    public float returnToSpawnerTime = 10f;
    float _spawnTimerCounter;
    int _index, _enemyCounter;
    List<GameObject> _spawned = new List<GameObject>();
    List<FollowPlayer> _spawnedFollow = new List<FollowPlayer>();
    List<float> _timeGivenUp = new List<float>();

    public event System.EventHandler<GameObject> enemySpawned;

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
            SpawnEnemy();
        }

        for (int i=0; i < _spawned.Count; i++)
        {
            if (_spawned[i] == null)
            {
                _spawned.RemoveAt(i);
                _spawnedFollow.RemoveAt(i);
                _timeGivenUp.RemoveAt(i);
                i--;
            }
            else
            {
                // Bring back to the spawner after a certain amount of time.
                if (_spawnedFollow[i].HasGivenUp || _spawnedFollow[i].IsIdle)
                {
                    if (Mathf.Abs(MathUtil.ToroidalDistance(this.transform.position, _spawned[i].transform.position).magnitude) > 50)
                    {
                        _timeGivenUp[i] += Time.deltaTime;

                        if (_timeGivenUp[i] >= returnToSpawnerTime)
                        {
                            _spawned[i].transform.position = this.transform.position;
                            _timeGivenUp[i] = 0;
                        }
                    }
                }
                else
                {
                    _timeGivenUp[i] = 0;
                }
            }
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemyObj = GameObject.Instantiate(enemyPrefabs[_index++], this.transform.position, Quaternion.identity);
        _spawned.Add(enemyObj);
        _timeGivenUp.Add(0);

        AttackPlayer ap = enemyObj.GetComponent<AttackPlayer>();
        FollowPlayer fp = enemyObj.GetComponent<FollowPlayer>();

        _spawnedFollow.Add(fp);

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

        enemySpawned?.Invoke(this, enemyObj);
    }
}
