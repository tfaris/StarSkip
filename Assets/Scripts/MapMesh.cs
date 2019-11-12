using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class MapMesh : MonoBehaviour
{
    int _lastGrid;
    Texture2D _tex;
    MeshFilter _mFilter;
    public Mesh mesh;
    public RawImage image;
    bool _generated;
    int _pixelsPerGrid = 10;

    Color explored = Color.white;
    Color playerCurrent = Color.yellow;// new Color(255f/265f, 255f/165f, 0);

    void Start()
    {
        _mFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        if (_generated)
        {
            int currentGrid = Game.Instance.GetCurrentGrid();
            if (currentGrid != _lastGrid)
            {
                var gs = Game.Instance.GetGridState(currentGrid);
                var lastGridState = Game.Instance.GetGridState(_lastGrid);
                gs.explored = true;
                UpdateMap(_lastGrid, lastGridState);
                UpdateMap(currentGrid, gs);
                _lastGrid = currentGrid;
            }
        }
    }

    void UpdateMap(int gridPos, GridState gs)
    {
        int x = gridPos / Game.Instance.worldGridsWide;
        int y = gridPos % Game.Instance.worldGridsWide;

        int xPix = x * _pixelsPerGrid, yPix = y * _pixelsPerGrid;
        int xEnd = xPix + _pixelsPerGrid, yEnd = yPix + _pixelsPerGrid;
        Color[] pixels = new Color[_pixelsPerGrid * _pixelsPerGrid];

        for (int row=0; row < _pixelsPerGrid; row++)
        {
            for (int col=0; col < _pixelsPerGrid; col++)
            {
                int i = col * _pixelsPerGrid + row;
                Color c;
                if (gridPos == Game.Instance.GetCurrentGrid())
                {
                    c = playerCurrent;
                }
                else if (row == 0 || row == yEnd - 1 || col == 0 || col == xEnd - 1)
                {
                    c = Color.black;
                }
                else
                {
                    c = explored;
                }
                pixels[i] = c;
            }
        }

        int w = (int)(x * _tex.width / (float)Game.Instance.worldGridsWide), h = (int)(y * _tex.height / (float)Game.Instance.worldGridsHigh);

        _tex.SetPixels(xPix, yPix, _pixelsPerGrid, _pixelsPerGrid, pixels, 0);
        _tex.Apply();
        image.texture = _tex;

        // var tris = mesh.triangles;
        // int ti = gridPos * 6;

        // if (gs.explored)
        // {
        //     tris[ti] = gridPos;
        //     tris[ti + 3] = tris[ti + 2] = gridPos + 1;
        //     tris[ti + 4] = tris[ti + 1] = gridPos + Game.Instance.worldGridsWide + 1;
        //     tris[ti + 5] = gridPos + Game.Instance.worldGridsWide + 2;
        // }
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // mesh.RecalculateTangents();
        // _mFilter.mesh = mesh;
    }

    public void GenerateMapMesh()
    {
        _tex = new Texture2D(
            Game.Instance.worldGridsWide * _pixelsPerGrid,
            Game.Instance.worldGridsHigh * _pixelsPerGrid
        );
        _tex.SetPixels(new Color[_tex.width * _tex.height] );
        image.texture = _tex;

        // int xSize = Game.Instance.worldGridsWide, ySize = Game.Instance.worldGridsHigh;

        // mesh = new Mesh();

        // Vector3[] verts = new Vector3[(xSize + 1) * (ySize + 1)];
		// int[] tris = new int[xSize * ySize * 6];

		// for (int i = 0, y = 0; y <= ySize; y++) {
		// 	for (int x = 0; x <= xSize; x++, i++) {
		// 		verts[i] = new Vector3(x, y);
		// 	}
		// }

		// for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) 
        // {
		// 	for (int x = 0; x < xSize; x++, ti += 6, vi++) 
        //     {
        //         var gridVert = verts[vi];
        //         int gridPos = (int)(Game.Instance.worldGridsWide * gridVert.x + gridVert.y);
        //         var gridState = Game.Instance.GetGridState(gridPos);
                
        //         if (gridState.explored)
        //         {
        //             tris[ti] = vi;
        //             tris[ti + 3] = tris[ti + 2] = vi + 1;
        //             tris[ti + 4] = tris[ti + 1] = vi + xSize + 1;
        //             tris[ti + 5] = vi + xSize + 2;
        //         }
		// 	}
		// }
		// mesh.vertices = verts;
		// mesh.triangles = tris;

        // var mf = GetComponent<MeshFilter>();
        // mf.mesh = mesh;
        _generated = true;
    }
}
