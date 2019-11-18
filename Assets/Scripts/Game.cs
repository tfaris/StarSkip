using System;
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
    public List<PredefinedStuff> predefinedStuff;
    public AsteroidController worldAmbientAsteroidSpawner;
    [HideInInspector]
    [NonSerialized]
    public float gridWidth, gridHeight;
    [HideInInspector]
    public int gameSeed;

    float lastAspectRatio;
    bool placedPlayer, firstGen = true;
    CameraController cameraController;

    System.Collections.Generic.Dictionary<int, GridState> gridStates;

    private static Game _instance;
    public static Game Instance
    {
        get {
            return _instance;
        }
    }

    void Start()
    {
        gameSeed = UnityEngine.Random.Range(0, int.MaxValue);
        gridStates = new Dictionary<int, GridState>();
        _instance = this;
        cameraController = FindObjectOfType<CameraController>();
    }

    void Update()
    {
        if (Camera.main.aspect != lastAspectRatio)
        {
            // Aspect ratio changed. Now we need to adjust our grids.
            lastAspectRatio = Camera.main.aspect;
            gridWidth = singleGridSize;
            gridHeight = gridWidth / lastAspectRatio;

            WorldBoundaries = new Rect(
                worldOrigin.x,
                worldOrigin.y,
                worldGridsWide * gridWidth,
                worldGridsHigh * gridHeight
            );

            // Calculate the required camera height to fit the grid size.
            // This part is voodoo.
            // credit: https://forum.unity.com/threads/fit-object-exactly-into-perspective-cameras-field-of-view-focus-the-object.496472/
            float objectSize = Mathf.Max(gridWidth, gridHeight);
            float cameraView = Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView);
            float camHeight = .5f * gridHeight / cameraView;
            //

            cameraController.CenterCameraOn(playerShip.transform);

            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x,
                camHeight,
                Camera.main.transform.position.z
            );

            if (firstGen)
            {
                SpawnPredefinedObjects();
                StartCoroutine(SpawnAmbientAsteroids());
            }

            if (!placedPlayer)
            {
                // Randomly place the player.
                playerShip.transform.position = new Vector3(
                    UnityEngine.Random.Range(WorldBoundaries.xMin + 1, WorldBoundaries.xMax - 1),
                    playerShip.transform.position.y,
                    UnityEngine.Random.Range(WorldBoundaries.yMin + 1, WorldBoundaries.yMax - 1)
                );
                placedPlayer = true;
            }
            
            firstGen = false;
        }
        
        // for (int x=0; x < worldGridsWide; x++)
        // {
        //     for (int z = 0; z < worldGridsHigh; z++)
        //     {
        //         float xMin = x * gridWidth,
        //             xMax = xMin + gridWidth,
        //             zMin = z * gridHeight,
        //             zMax = zMin + gridHeight;

        //         Debug.DrawLine(
        //             new Vector3(xMin, 0, zMin),
        //             new Vector3(xMin, 0, zMax),
        //             Color.yellow
        //         );
        //         Debug.DrawLine(
        //             new Vector3(xMin, 0, zMax),
        //             new Vector3(xMax, 0, zMax),
        //             Color.yellow
        //         );
        //         Debug.DrawLine(
        //             new Vector3(xMax, 0, zMax),
        //             new Vector3(xMax, 0, zMin),
        //             Color.yellow
        //         );
        //         Debug.DrawLine(
        //             new Vector3(xMax, 0, zMin),
        //             new Vector3(xMin, 0, zMin),
        //             Color.yellow
        //         );

        //         // GameObject tmp = new GameObject();
        //         // LineRenderer lr = tmp.AddComponent<LineRenderer>();
        //         // lr.material = new Material(Shader.Find("Diffuse"));
        //         // lr.startColor = Color.yellow;
        //         // lr.endColor = Color.yellow;
        //         // lr.startWidth = lr.endWidth = 1;
        //         // lr.positionCount = 5;

        //         // lr.SetPosition(0, new Vector3(xMin, 0, zMin));
        //         // lr.SetPosition(1, new Vector3(xMin, 0, zMax));
        //         // lr.SetPosition(2, new Vector3(xMax, 0, zMax));
        //         // lr.SetPosition(3, new Vector3(xMax, 0, zMin));
        //         // lr.SetPosition(4, new Vector3(xMin, 0, zMin));
        //     }
        // }
    }

    ///
    /// Create all of the predefined/prepositioned objects.
    ///
    private void SpawnPredefinedObjects()
    {
        GameObject predefinedContainer = new GameObject("SpecialObjects");
        foreach (PredefinedStuff ps in predefinedStuff)
        {
            int gridX, gridZ; 
            if (ps.spawnAtRandomPosition)
            {
                gridX = UnityEngine.Random.Range(0, worldGridsWide);
                gridZ = UnityEngine.Random.Range(0, worldGridsHigh);
            }
            else
            {
                gridX = ps.gridX;
                gridZ = ps.gridZ;
            }

            GridState gridState = GetGridState(gridX, gridZ);
            var worldPos = GetCenterPositionForGrid(gridX, gridZ);
            worldPos.x -= gridWidth / 2 - ps.xWithinGrid * gridWidth;
            worldPos.z -= gridHeight / 2 - ps.zWithinGrid * gridHeight;
            
            if (ps.objectToSpawn != null)
            {
                var obj = GameObject.Instantiate(
                    ps.objectToSpawn,
                    worldPos,
                    Quaternion.identity,
                    predefinedContainer.transform
                );

                // Setup various grid states here after creation.
                var asteroidSpawner = obj.GetComponent<AsteroidController>();
                if (asteroidSpawner != null)
                {
                    gridState.asteroidSpawner = asteroidSpawner;
                }
            }
        }
    }

    private IEnumerator SpawnAmbientAsteroids()
    {
        GameObject asteroidsContainer = new GameObject("ambient asteroids container");
        for (int z = 0; z < worldGridsHigh; z++)
        {
            for (int x=0; x < worldGridsWide; x++)
            {
                bool asteroidsHere = UnityEngine.Random.Range(0f, 1f) >= .8f;
                if (asteroidsHere)
                {
                    var worldPos = GetCenterPositionForGrid(x, z);
                    float adjustX = UnityEngine.Random.Range(0, 1),
                        adjustZ = UnityEngine.Random.Range(0, 1);
                    worldPos.x -= gridWidth / 2 - adjustX * gridWidth;
                    worldPos.z -= gridHeight / 2 - adjustZ * gridHeight;
                    GameObject.Instantiate(
                        worldAmbientAsteroidSpawner,
                        worldPos,
                        Quaternion.identity,
                        asteroidsContainer.transform
                    );
                }
                yield return null;
            }
        }
    }

    public int GetGrid(Vector3 position)
    {
        int col = (int)Mathf.Floor(position.x / gridWidth),
         row = (int)Mathf.Floor(position.z / gridHeight);
        return worldGridsWide * row + col;
    }

    public int GetCurrentGrid()
    {
        return GetGrid(playerShip.transform.position);
    }

    string ColumnToLetter(int column) 
    {
        int temp = 0;
        string letter = "";
        while (column > 0)
        {
            temp = (column - 1) % 26;
            letter = ((char)(temp + 65)).ToString() + letter;
            column = (column - temp - 1) / 26;
        }
        return letter;
    }

    public string GetSectorName(int gridPosition)
    {
        int col = gridPosition % worldGridsWide,
            row = gridPosition / worldGridsWide;
        return string.Format("{0}/{1}", ColumnToLetter(col + 1), row + 1);
    }

    ///
    /// Get the world center position of the specified grid position.
    ///
    public Vector3 GetCenterPositionForGrid(int gridPosition)
    {
        int gridX = gridPosition % worldGridsWide,
            gridZ = gridPosition / worldGridsWide;
        return GetCenterPositionForGrid(gridX, gridZ);
    }

    ///
    /// Get the world center position for the specified grid x and y.
    ///
    public Vector3 GetCenterPositionForGrid(int gridX, int gridZ)
    {
        // TODO: This is a problem with negative grid numbers. Probably should just set world boundaries.
        return new Vector3(
            gridX * gridWidth + gridWidth / 2f,
            0,
            gridZ * gridHeight + gridHeight / 2f
        );
    }

    ///
    /// Get the bounding area of the specified grid position.
    ///
    public Rect GetBoundsForGrid(int gridPosition)
    {
        var centerPos = GetCenterPositionForGrid(gridPosition);
        centerPos.x -= gridWidth / 2f;
        centerPos.z -= gridHeight / 2f;
        centerPos.y = centerPos.z;
        return new Rect(
            centerPos,
            new Vector2(gridWidth, gridHeight)
        );
    }

    ///
    /// Get a position that is outside of the world bounds.
    ///
    public Vector3 GetOutOfBoundsPosition()
    {
        return new Vector3(
            WorldBoundaries.x - 1000,
            0,
            WorldBoundaries.y - 1000
        );
    }

    public GridState GetGridState(int gridX, int gridZ)
    {
        return GetGridState(worldGridsWide * gridX + gridZ);
    }

    public GridState GetGridState(int gridPos)
    {
        if (!gridStates.TryGetValue(gridPos, out GridState gs))
        {
            gs = gridStates[gridPos] = new GridState();
        }
        return gs;
    }
}
