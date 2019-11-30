using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int worldGridsWide = 10, worldGridsHigh = 10;
    public float singleGridSize = 20;
    public bool spawnAmbientAsteroids = true;
    public PlayerShip playerShip;
    [Tooltip("Number of enemies per zone that should be killed to reach next goal.")]
    public int goalEnemiesPerZone = 15;

    Vector2 worldOrigin = new Vector2(0, 0);
    public Rect WorldBoundaries { get; private set; }
    public List<PredefinedStuff> predefinedStuff;
    public AsteroidController worldAmbientAsteroidSpawner;
    [Tooltip("Upgrade the ship whenever this many grids have been explored.")]
    public int upgradeOnGridsExplored;
    [HideInInspector]
    [NonSerialized]
    public float gridWidth, gridHeight;
    [HideInInspector]
    public int gameSeed;
    public UImanager uIManager;
    public Boss bossPrefab;

    public AudioSource effectsAudioSource;

    float lastAspectRatio;
    bool placedPlayer, firstGen = true;
    CameraController cameraController;
    int _explorationCount;
    List<UItext.MessageType> _messagesShownOnce = new List<UItext.MessageType>();

    System.Collections.Generic.Dictionary<int, GridState> gridStates;
    List<Zone> _zones = new List<Zone>();
    bool _enoughEnemiesKilled;

    private static Game _instance;
    public static Game Instance
    {
        get {
            return _instance;
        }
    }

    public List<Zone> Zones
    {
        get => _zones;
    }

    ///
    /// The object the game is currently tracking. Will normally
    /// be the players ship.
    ///
    public GameObject TrackingObject
    {
        get => cameraController.trackThis;
        set => cameraController.trackThis = value;
    }

    public bool IsSetup
    {
        get;
        private set;
    }

    ///
    /// Total number of explored grid spaces.
    ///
    public int ExplorationCount
    {
        get => _explorationCount;
        set
        {
            if (value != _explorationCount)
            {
                _explorationCount = value;

                if (_explorationCount % this.upgradeOnGridsExplored == 0)
                {
                    playerShip?.Upgrade(playerShip?.GetUpgradeable());
                }
            }
        }
    }

    void Start()
    {
        gameSeed = UnityEngine.Random.Range(0, int.MaxValue);
        gridStates = new Dictionary<int, GridState>();
        _instance = this;
        cameraController = FindObjectOfType<CameraController>();
        TrackingObject = playerShip.gameObject;
    }

    void Update()
    {
        if (!IsSetup && Camera.main.aspect != lastAspectRatio)
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

            cameraController.CenterCameraOn(TrackingObject.transform);

            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x,
                camHeight,
                Camera.main.transform.position.z
            );

            if (firstGen)
            {
                SpawnPredefinedObjects();
                if (spawnAmbientAsteroids)
                {
                    StartCoroutine(SpawnAmbientAsteroids());
                }
                _zones.AddRange(GameObject.FindObjectsOfType<Zone>());
            }

            if (!placedPlayer)
            {
                // Randomly place the player.
                var worldPos = new Vector3(
                    UnityEngine.Random.Range(WorldBoundaries.xMin + 1, WorldBoundaries.xMax - 1),
                    playerShip.transform.position.y,
                    UnityEngine.Random.Range(WorldBoundaries.yMin + 1, WorldBoundaries.yMax - 1)
                );
                playerShip.transform.position = GetCenterPositionForGrid(GetGrid(worldPos));
                placedPlayer = true;
            }
            
            firstGen = false;
            IsSetup = true;
            
            StartCoroutine(ShowTutorialMessages());
        }

        if (IsSetup)
        {
            if (playerShip)
            {
                if (playerShip.warpPoints > 0)
                {
                    ShowMessageOnce(UItext.MessageType.WarpTut2);
                }
            }
            else
            {
                // no playership? loss....
                Game.Instance.ShowMessageOnce(UItext.MessageType.GameOverLoss, Game.Instance.GetRank().ToString());
            }

            if (!_enoughEnemiesKilled)
            {
                int zonesCompletedCount = 0;
                foreach (Zone z in _zones)
                {
                    if (z.DestroyedEnemiesCount > goalEnemiesPerZone)
                    {
                        zonesCompletedCount++;
                    }
                }

                if (zonesCompletedCount == _zones.Count)
                {
                    _enoughEnemiesKilled = true;
                    SpawnBoss();
                }
            }
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

    IEnumerator ShowTutorialMessages()
    {
        yield return new WaitForSeconds(2f);
        ShowMessageOnce(UItext.MessageType.Tut1);
        ShowMessageOnce(UItext.MessageType.Tut2);
        ShowMessageOnce(UItext.MessageType.Tut3);
        ShowMessageOnce(UItext.MessageType.WarpTut1);
    }

    void SpawnBoss()
    {
        var spawnLoc = new Vector3(
            WorldBoundaries.xMin + (WorldBoundaries.width / 2f),
            0,
            WorldBoundaries.yMin + (WorldBoundaries.height / 2f) 
        );
        GameObject.Instantiate(
            bossPrefab,
            spawnLoc,
            Quaternion.identity
        );
        ShowMessageOnce(UItext.MessageType.BigBad);

        GridState gs = GetGridState(GetGrid(spawnLoc));
        gs.isBossLocation = true;
    }

    ///
    /// Show the specified message.
    ///
    public void ShowMessage(UItext.MessageType messageType, params string[] formatArgs)
    {
        if (uIManager)
        {
            uIManager.ShowMessage(messageType, formatArgs);
        }
    }

    ///
    /// Show the specified message only once.
    ///
    public bool ShowMessageOnce(UItext.MessageType messageType, params string[] formatArgs)
    {
        if (!_messagesShownOnce.Contains(messageType))
        {
            _messagesShownOnce.Add(messageType);
            ShowMessage(messageType, formatArgs);
            return true;
        }
        return false;
    }

    public int GetRank()
    {
        return ExplorationCount;
    }

    ///
    /// Returns true if the specified object is in the same world
    /// grid as the player.
    ///
    public bool IsInPlayerGrid(GameObject obj)
    {
        int playerGrid = Game.Instance.GetCurrentGrid();
        int objGrid = Game.Instance.GetGrid(obj.transform.position);
        return playerGrid == objGrid;
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

    ///
    /// Create random, ambient asteroid spawners around the map.
    ///
    private IEnumerator SpawnAmbientAsteroids()
    {
        GameObject asteroidsContainer = new GameObject("ambient asteroids container");
        for (int z = 0; z < worldGridsHigh; z++)
        {
            for (int x=0; x < worldGridsWide; x++)
            {
                GridState gs = GetGridState(x, z);
                if (!gs.isEnemyArea)
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
                }
            }
            yield return null;
        }
    }
    
    public int GetGrid(Vector3 worldPosition)
    {
        int col = (int)Mathf.Floor(worldPosition.x / gridWidth),
         row = (int)Mathf.Floor(worldPosition.z / gridHeight);
        return worldGridsWide * row + col;
    }

    ///
    /// Get the grid X and Z for the grid at the specified
    /// world coordinates.
    ///
    public Vector3 GetGridXZ(Vector3 worldPosition)
    {
        return GetGridXZ(GetGrid(worldPosition));
    }

    ///
    /// Get the grid X and Z for the specified grid position.
    ///
    public Vector3 GetGridXZ(int gridPosition)
    {
        return new Vector3(gridPosition % worldGridsWide, 0, gridPosition / worldGridsWide);
    }

    public int GetCurrentGrid()
    {
        if (TrackingObject)
        {
            return GetGrid(TrackingObject.transform.position);
        }
        else
        {
            return 0;
        }
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

    ///
    /// Return true if the specified world position is outside
    /// of the grid.
    ///
    public bool GetIsOutOfWorldBounds(Vector3 worldPosition)
    {
        var gridPos = GetGridXZ(worldPosition);        
        return gridPos.x < 0 || gridPos.z < 0;
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

    ///
    /// Check whether the specified transform is outside of world bounds.
    /// If so, moves the transform to the other side of the world and
    /// returns true. Otherwise returns false.
    ///
    public bool CheckWorldWrap(Transform transform)
    {
        bool changed = false;
        var pos = transform.position;
        if (pos.x < WorldBoundaries.xMin)
        {
            changed = true;
            pos.x = WorldBoundaries.xMax - 1;
        }
        else if (pos.x > WorldBoundaries.xMax)
        {
            changed = true;
            pos.x = WorldBoundaries.xMin + 1;
        }
        if (pos.z < WorldBoundaries.yMin)
        {
            changed = true;
            pos.z = WorldBoundaries.yMax - 1;
        }
        else if (pos.z > WorldBoundaries.yMax)
        {
            changed = true;
            pos.z = WorldBoundaries.yMin + 1;
        }
        if (changed)
        {
            transform.position = pos;
        }
        return changed;
    }
}
