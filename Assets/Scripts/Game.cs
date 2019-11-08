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
    [HideInInspector]
    public float gridWidth, gridHeight;

    float lastAspectRatio;
    bool placedPlayer;
    CameraController cameraController;

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

            if (!placedPlayer)
            {
                // Randomly place the player.
                playerShip.transform.position = new Vector3(
                    Random.Range(WorldBoundaries.xMin + 1, WorldBoundaries.xMax - 1),
                    playerShip.transform.position.y,
                    Random.Range(WorldBoundaries.yMin + 1, WorldBoundaries.yMax - 1)
                );
                placedPlayer = true;
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

    public int GetGrid(Vector3 position)
    {
        int col = (int)Mathf.Floor(position.x / gridWidth),
         row = (int)Mathf.Floor(position.z / gridHeight);
        return worldGridsWide * col + row;
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
        int col = gridPosition / worldGridsWide,
            row = gridPosition % worldGridsHigh;
        return string.Format("{0}/{1}", ColumnToLetter(col + 1), row + 1);
    }
}
