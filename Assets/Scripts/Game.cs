using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int worldGridsWide = 10, worldGridsHigh = 10;
    public float singleGridSize = 20;
    public PlayerShip playerShip;

    Vector2 worldOrigin = new Vector2(0, 0);
    public Rect WorldBoundaries { get; private set; }

    private static Game _instance;
    public static Game Instance
    {
        get {
            return _instance;
        }
    }

    void Start()
    {
        _instance = this;
        WorldBoundaries = new Rect(
            worldOrigin.x,
            worldOrigin.y,
            worldGridsWide * singleGridSize,
            worldGridsHigh * singleGridSize
        );
        // Randomly place the player.
        playerShip.transform.position = new Vector3(
            Random.Range(WorldBoundaries.xMin + 1, WorldBoundaries.xMax - 1),
            playerShip.transform.position.y,
            Random.Range(WorldBoundaries.yMin + 1, WorldBoundaries.yMax - 1)
        );
    }

    void Update()
    {
        for (int x=0; x < worldGridsWide; x++)
        {
            for (int z = 0; z < worldGridsHigh; z++)
            {
                float xMin = x * singleGridSize,
                      xMax = xMin + singleGridSize,
                      zMin = z * singleGridSize,
                      zMax = zMin + singleGridSize;

                Debug.DrawLine(
                    new Vector3(xMin, 0, zMin),
                    new Vector3(xMin, 0, zMax),
                    Color.yellow
                );
                Debug.DrawLine(
                    new Vector3(xMin, 0, zMax),
                    new Vector3(xMax, 0, zMax),
                    Color.yellow
                );
                Debug.DrawLine(
                    new Vector3(xMax, 0, zMax),
                    new Vector3(xMax, 0, zMin),
                    Color.yellow
                );
                Debug.DrawLine(
                    new Vector3(xMax, 0, zMin),
                    new Vector3(xMin, 0, zMin),
                    Color.yellow
                );
            }
        }
    }

    public int GetGrid(Vector3 position)
    {
        int col = (int)Mathf.Floor(position.x / singleGridSize),
         row = (int)Mathf.Floor(position.z / singleGridSize);
        return worldGridsWide * col + row;
    }
}
