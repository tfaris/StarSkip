using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{

    public List<GameObject> asteroidPool = new List<GameObject>();

    public Game game;

    public float spawnRate;
    public float averageSpd;

    public bool grey;
    public bool larger;

    public float xBoundary;
    public float zBoundary;

    public float weightedTwdsMid;
    public float distanceToDespawn;

    float timer;

    Vector3 t;

    List<GameObject> _spawnedAsteroids = new List<GameObject>();

    void Start()
    {
        t = transform.position;
    }

    void Update()
    {
        xBoundary = Game.Instance.gridWidth;
        zBoundary = Game.Instance.gridHeight;

        timer += .2f * Time.deltaTime;

        if (timer > spawnRate)
        {
            // Don't spawn if player isn't close
            float distToPlayer = Mathf.Abs((Game.Instance.playerShip.transform.position - transform.position).magnitude);
            if (distToPlayer <= distanceToDespawn * 1.5f)
            {
                int dirDie = Random.Range(0,4);

                GameObject asteroidObject;
                Asteroid asteroid;
                if (dirDie == 0)
                {
                    asteroidObject = Instantiate(asteroidPool[0], new Vector3(Random.Range(t.x, t.z + xBoundary), t.y, t.z), Random.rotation, this.transform);
                    asteroid = asteroidObject.GetComponent<Asteroid>();
                    asteroid.goDirection = Direction.goUp;
                }
                else if (dirDie == 1)
                {
                    asteroidObject = Instantiate(asteroidPool[0], new Vector3(Random.Range(t.x, t.x + xBoundary), t.y, t.z + zBoundary), Random.rotation, this.transform);
                    asteroid = asteroidObject.GetComponent<Asteroid>();
                    asteroid.goDirection = Direction.goDown;
                }
                else if (dirDie == 2)
                {
                    asteroidObject = Instantiate(asteroidPool[0], new Vector3(t.x + xBoundary, t.y, Random.Range(t.z, t.z + xBoundary)), Random.rotation, this.transform);
                    asteroid = asteroidObject.GetComponent<Asteroid>();
                    asteroid.goDirection = Direction.goLeft;
                }
                else
                {
                    asteroidObject = Instantiate(asteroidPool[0], new Vector3(t.x, t.y, Random.Range(t.z, t.z + xBoundary)), Random.rotation, this.transform);
                    asteroid = asteroidObject.GetComponent<Asteroid>();
                    asteroid.goDirection = Direction.goRight;
                }
                asteroid.spawner = this;
            }

            timer = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(t, new Vector3(t.x + xBoundary, t.y, t.z));
        Gizmos.DrawLine(t, new Vector3(t.x, t.y, t.z + zBoundary));
        Gizmos.DrawLine(new Vector3 (t.x, t.y, t.z + zBoundary), new Vector3(t.x + xBoundary, t.y, t.z + zBoundary));
        Gizmos.DrawLine(new Vector3 (t.x + xBoundary, t.y, t.z), new Vector3(t.x + xBoundary, t.y, t.z + zBoundary));

        Gizmos.DrawLine(Game.Instance.playerShip.transform.position, transform.position);

        Gizmos.DrawWireSphere(this.transform.position, distanceToDespawn);
    }
}
