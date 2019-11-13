using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RawImage image;
    int _lastGrid;
    Texture2D _tex;
    bool _generated;
    int _pixelsPerGrid = 10;

    Color explored = Color.white;
    Color playerCurrent = Color.yellow;// new Color(255f/265f, 255f/165f, 0);
    Color asteroidColor = new Color32(123, 79, 44, 255);

    void Update()
    {
        if (_generated)
        {
            int currentGrid = Game.Instance.GetCurrentGrid();
            if (currentGrid != _lastGrid)
            {
                var lastGridState = Game.Instance.GetGridState(_lastGrid);
                var gs = Game.Instance.GetGridState(currentGrid);
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

        // Fill in all of the pixels at this grid position on 
        // the map.
        
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
                else if (gs.asteroidSpawner != null)
                {
                    c = asteroidColor;
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
    }

    public void GenerateMap()
    {
        _tex = new Texture2D(
            Game.Instance.worldGridsWide * _pixelsPerGrid,
            Game.Instance.worldGridsHigh * _pixelsPerGrid
        );
        _tex.SetPixels(new Color[_tex.width * _tex.height] );
        image.texture = _tex;
        _generated = true;
    }
}
