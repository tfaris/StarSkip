using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RawImage image;
    public Mesh tileMesh;
    
    public Material matUnexplored;
    public Material matExplored;
    public Material matCurrent;
    public Material matAsteroidField;
    public Material matQuestObjective;
    public Material matEnemyZone;
    public Camera mapCamera;

    bool _fullScreen;
    Vector2 _tmpOffsetMin, _tmpOffsetMax, _tmpSizeDelta;
    Vector3 _tmpPos;
    
    Mesh _mapMesh;

    MeshRenderer _mRenderer;
    MeshFilter _mFilter;
    List<SubmeshData> _submeshes;

    void Start()
    {
        _mapMesh = new Mesh();
        _mRenderer = gameObject.AddComponent<MeshRenderer>();
        _mFilter = gameObject.AddComponent<MeshFilter>();        

        _submeshes = new List<SubmeshData>()
        {
            new SubmeshData(0, matUnexplored),
            new SubmeshData(1, matExplored),
            new SubmeshData(2, matCurrent),
            new SubmeshData(3, matAsteroidField),
            new SubmeshData(4, matQuestObjective),
            new SubmeshData(5, matEnemyZone),
        };
        // Each submesh uses the material in the renderer at the same index.
        _mRenderer.materials = _submeshes
            .Select(sd => sd.Material)
            .ToArray();
    }

    void Update()
    {
        GenerateMap();

        if (Input.GetButtonUp("Map"))
        {
            if (!_fullScreen)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

                _tmpOffsetMin = image.rectTransform.offsetMin;
                _tmpOffsetMax = image.rectTransform.offsetMax;
                _tmpPos = image.rectTransform.position;
                _tmpSizeDelta = image.rectTransform.sizeDelta;

                image.rectTransform.anchorMin = new Vector2(0, 0);
                image.rectTransform.anchorMax = new Vector2(1, 1);
                image.rectTransform.offsetMax = new Vector2();
                image.rectTransform.offsetMin = new Vector2();
                
                _fullScreen = true;
            }
            else
            {
                // Slightly transparent when mini
                image.color = new Color(image.color.r, image.color.g, image.color.b, .5f);
                image.rectTransform.anchorMin = new Vector2(1, 0);
                image.rectTransform.anchorMax = new Vector2(1, 0);
                image.rectTransform.offsetMax = _tmpOffsetMin;
                image.rectTransform.offsetMin = _tmpOffsetMin;
                image.rectTransform.sizeDelta = _tmpSizeDelta;
                image.rectTransform.position = _tmpPos;
                _fullScreen = false;
            }
        }
    }

    public void GenerateMap()
    {
        _mapMesh.Clear();

        int total = Game.Instance.worldGridsWide * Game.Instance.worldGridsHigh;
        
        // Clear all of the submesh vert data
        foreach (var sd in _submeshes)
        {
            sd.triangles.Clear();
        }

        Vector3[] verts = new Vector3[4 * total];
        int[] triangles = new int[6 * total];
        int curVert = 0, curTri = 0;
        float spacing = 2f;

        for (int i=0; i < total; i++, curVert += 4, curTri += 6)
        {
            int gridPos = i;

            int gridX = gridPos % Game.Instance.worldGridsWide,
                gridZ = gridPos / Game.Instance.worldGridsWide;

            // bl
            verts[curVert] = new Vector3(gridX * spacing, 0, gridZ * spacing);
            // tl 
            verts[curVert + 1] = new Vector3(gridX * spacing, 0, spacing * gridZ + 1);
            // tr
            verts[curVert + 2] = new Vector3(spacing * gridX + 1, 0, spacing * gridZ + 1);
            // br
            verts[curVert + 3] = new Vector3(spacing * gridX + 1, 0, spacing * gridZ);

            triangles[curTri] = curVert;
            triangles[curTri + 1] = curVert + 1;
            triangles[curTri + 2] = curVert + 2;
            triangles[curTri + 3] = curVert + 0;
            triangles[curTri + 4] = curVert + 2;
            triangles[curTri + 5] = curVert + 3;

            // Pick the submesh by assigning the index of the material that
            // we want to use for it.
            int submesh;
            GridState gridState = Game.Instance.GetGridState(gridPos);
            if (gridPos == Game.Instance.GetCurrentGrid())
            {
                submesh = 2;
            }
            else
            {
                if (gridState.hasQuestObjective || gridState.isBossLocation)
                {
                    submesh = 4;
                }                
                else if (gridState.explored)
                {
                    if (gridState.asteroidSpawner != null)
                    {
                        submesh = 3;
                    }
                    else
                    {
                        submesh = 1;
                    }
                }
                else if (gridState.isEnemyArea)
                {
                    submesh = 5;
                }
                else
                {
                    submesh = 0;
                }
            }

            SubmeshData submeshData = _submeshes[submesh];
            submeshData.triangles.AddRange(
                new int[]{
                curVert,
                curVert + 1,
                curVert + 2,
                curVert + 0,
                curVert + 2,
                curVert + 3
                });
        }

        _mapMesh.vertices = verts;
        _mapMesh.triangles = triangles;
        _mapMesh.subMeshCount = _submeshes.Count;

        foreach (var sd in _submeshes)
        {
            _mapMesh.SetTriangles(sd.triangles, sd.submeshIndex);
        }
        
        _mapMesh.RecalculateNormals();
        _mapMesh.RecalculateNormals();
        _mFilter.mesh = _mapMesh;
        

        // Calculate the required camera height to fit the grid size.
        // This part is voodoo.
        // credit: https://forum.unity.com/threads/fit-object-exactly-into-perspective-cameras-field-of-view-focus-the-object.496472/
        var mapBounds = _mapMesh.bounds;
        float objectSize = Mathf.Max(mapBounds.max.x, mapBounds.max.z);
        float cameraView = Mathf.Tan(0.5f * Mathf.Deg2Rad * mapCamera.fieldOfView);
        float camHeight = .5f * mapBounds.max.z / cameraView;
        mapCamera.transform.localPosition = new Vector3(
            mapBounds.center.x,
            camHeight,
            mapBounds.center.z
        );
        //
    }

    ///
    /// Represents all triangles for a particular submesh.
    ///
    class SubmeshData
    {
        public List<int> triangles;
        public int submeshIndex;
        public Material Material {get; private set;}

        public SubmeshData(int submeshIndex, Material mat)
        {
            this.submeshIndex = submeshIndex;
            this.Material = mat;
            this.triangles = new List<int>();
        }
    }
}
