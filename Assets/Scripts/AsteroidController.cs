using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{

    public List<GameObject> asteroidPool = new List<GameObject>();

    public Game game;

    GameObject asteroidObject;
    Asteroid asteroid;

    public float spawnRate;
    public float averageSpd;

    public bool grey;
    public bool larger;

    public float xBoundary;
    public float zBoundary;

    public float weightedTwdsMid;

    float timer;

    Vector3 t;

    void Start()
    {
        t = transform.position;
    }

    void Update()
    {
        xBoundary = game.gridWidth;
        zBoundary = game.gridHeight;

        timer += .2f * Time.deltaTime;

        if (timer > spawnRate)
        {
            int dirDie = Random.Range(0,4);

            if (dirDie == 0)
            {
                asteroidObject = Instantiate(asteroidPool[0], new Vector3(Random.Range(t.x, t.z + xBoundary), t.y, t.z), Random.rotation);
                asteroid = asteroidObject.GetComponent<Asteroid>();
                asteroid.goDirection = Direction.goUp;
            }
            else if (dirDie == 1)
            {
                asteroidObject = Instantiate(asteroidPool[0], new Vector3(Random.Range(t.x, t.x + xBoundary), t.y, t.z + zBoundary), Random.rotation);
                asteroid = asteroidObject.GetComponent<Asteroid>();
                asteroid.goDirection = Direction.goDown;
            }
            else if (dirDie == 2)
            {
                asteroidObject = Instantiate(asteroidPool[0], new Vector3(t.x + xBoundary, t.y, Random.Range(t.z, t.z + xBoundary)), Random.rotation);
                asteroid = asteroidObject.GetComponent<Asteroid>();
                asteroid.goDirection = Direction.goLeft;
            }
            else
            {
                asteroidObject = Instantiate(asteroidPool[0], new Vector3(t.x, t.y, Random.Range(t.z, t.z + xBoundary)), Random.rotation);
                asteroid = asteroidObject.GetComponent<Asteroid>();
                asteroid.goDirection = Direction.goRight;
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
    }
}
