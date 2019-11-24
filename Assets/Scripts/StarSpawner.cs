using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab;
    public Vector2 starDistanceRange = new Vector2(-100, -10);
    int gameSeed;

    Dictionary<int, List<GameObject>> gridStars;
    List<GameObject> lastSpawnedStars;
    int lastGenGrid;

    void Start()
    {
        gridStars = new Dictionary<int, List<GameObject>>();
        lastSpawnedStars = new List<GameObject>();
    }

    void Update()
    {
        int currentGrid = Game.Instance.GetCurrentGrid();
        if (lastGenGrid != currentGrid)
        {
            foreach (GameObject oldStar in lastSpawnedStars)
            {
                GameObject.Destroy(oldStar);
            }

            lastGenGrid = currentGrid;

            System.Random rGen = new System.Random(currentGrid ^ Game.Instance.gameSeed);
            int starCount = rGen.Next(30, 60);
            Vector3 gridCenter = Game.Instance.GetCenterPositionForGrid(currentGrid);

            for (int i=0; i < starCount; i++)
            {
                float min = gridCenter.x - Game.Instance.gridWidth,
                      max = gridCenter.x + Game.Instance.gridWidth,
                      zMin = gridCenter.z - Game.Instance.gridHeight,
                      zMax = gridCenter.z + Game.Instance.gridHeight;
                float xOff = (float) rGen.NextDouble() * (max - min) + min,
                  yOff = (float) (float) rGen.NextDouble() * (starDistanceRange.y - (starDistanceRange.x)) + starDistanceRange.x,
                  zOff = (float) rGen.NextDouble() * (zMax - zMin) + zMin ;

                GameObject starObj = GameObject.Instantiate(
                    starPrefab,
                    new Vector3(
                        xOff,
                        yOff,
                        zOff
                    ),
                    Quaternion.identity,
                    this.transform
                );
                starObj.transform.position = new Vector3(xOff, yOff, zOff);
                lastSpawnedStars.Add(starObj);
            }
        }
    }
}
