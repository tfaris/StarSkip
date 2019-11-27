using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public GameObject backgroundPlane;

    Renderer backgroundRenderer;
    bool _positionChecked;

    void Start()
    {
        backgroundRenderer = backgroundPlane.GetComponent<Renderer>();
    }

    void Update()
    {
        if (Game.Instance && Game.Instance.IsSetup)
        {
            // Make sure the zone fits in the world.
            if (!_positionChecked)
            {
                Vector3 backgroundPos = backgroundPlane.transform.position;

                Rect worldBounds = Game.Instance.WorldBoundaries;
                Bounds zoneBounds = backgroundRenderer.bounds;

                if (zoneBounds.min.x < worldBounds.min.x)
                {
                    backgroundPos.x += worldBounds.min.x - zoneBounds.min.x;
                }
                else if (zoneBounds.max.x > worldBounds.max.x)
                {
                    backgroundPos.x -= zoneBounds.max.x - worldBounds.max.x;
                }
                
                if (zoneBounds.min.z < worldBounds.min.y)
                {
                    backgroundPos.z += worldBounds.min.y - zoneBounds.min.z;
                }
                else if (zoneBounds.max.z > worldBounds.max.y)
                {
                    backgroundPos.z -= zoneBounds.max.z - worldBounds.max.y;
                }

                if (backgroundPos != backgroundPlane.transform.position)
                {
                    Debug.Log("Moved from " + backgroundPlane.transform.position + " to " + backgroundPos);
                }

                backgroundPlane.transform.position = backgroundPos;
                _positionChecked = true;

                var newBounds = backgroundRenderer.bounds;
                for (float x = newBounds.min.x; x < newBounds.max.x; x += Game.Instance.gridWidth)
                {
                    for (float z = newBounds.min.z; z < newBounds.max.z; z += Game.Instance.gridHeight)
                    {
                        int gp = Game.Instance.GetGrid(new Vector3(x, 0, z));
                        GridState gs = Game.Instance.GetGridState(gp);
                        gs.isEnemyArea = true;
                    }
                }
            }
        }
    }
}
