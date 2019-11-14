using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RawImage image;
    int _lastGrid = -1;
    Texture2D _tex;
    bool _generated;
    int _pixelsPerGrid = 10;
    bool _fullScreen;
    Vector2 _tmpOffsetMin, _tmpOffsetMax, _tmpSizeDelta;
    Vector3 _tmpPos;


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
                if (_lastGrid >= 0)
                {
                    var lastGridState = Game.Instance.GetGridState(_lastGrid);
                    UpdateMap(_lastGrid, lastGridState);
                }

                var gs = Game.Instance.GetGridState(currentGrid);
                gs.explored = true;
                UpdateMap(currentGrid, gs);
                _lastGrid = currentGrid;
            }

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
    }

    void UpdateMap(int gridPos, GridState gs)
    {
        int x = gridPos / Game.Instance.worldGridsWide;
        int y = gridPos % Game.Instance.worldGridsWide;

        int xPix = x * _pixelsPerGrid, yPix = y * _pixelsPerGrid;
        int xEnd = xPix + _pixelsPerGrid - 1, yEnd = yPix + _pixelsPerGrid - 1;
        Color[] pixels = new Color[(_pixelsPerGrid - 1) * (_pixelsPerGrid - 1)];

        // Fill in all of the pixels at this grid position on 
        // the map.
        Color c;
        if (gridPos == Game.Instance.GetCurrentGrid())
        {
            c = playerCurrent;
        }
        else if (gs.asteroidSpawner != null)
        {
            c = asteroidColor;
        }
        else
        {
            c = explored;
        }

        for (int i=0; i < pixels.Length; i++)
        {
            pixels[i] = c;
        }

        _tex.SetPixels(xPix, yPix, _pixelsPerGrid - 1, _pixelsPerGrid - 1, pixels, 0);
        _tex.Apply();
        image.texture = _tex;
    }

    public void GenerateMap()
    {
        _tex = new Texture2D(
            Game.Instance.worldGridsWide * _pixelsPerGrid + 2,
            Game.Instance.worldGridsHigh * _pixelsPerGrid + 2
        );
        var pix = new Color[_tex.width * _tex.height];
        // for (int i=0; i < pix.Length; i++)
        // {
        //     pix[i] = Color.white;
        // }
        _tex.SetPixels(pix);
        image.texture = _tex;
        image.color = new Color(image.color.r, image.color.g, image.color.b, .5f);
        _generated = true;
    }
}
