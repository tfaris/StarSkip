using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject trackThis;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int cameraGrid = Game.Instance.GetGrid(Camera.main.transform.position),
            objGrid = Game.Instance.GetGrid(trackThis.transform.position);
        if (cameraGrid != objGrid)
        {
            Camera.main.transform.position = GetCameraPositionForGrid(objGrid);
        }
    }

    public void CenterCameraOn(Transform transform)
    {
        Camera.main.transform.position = GetCameraPositionForGrid(Game.Instance.GetGrid(transform.position));
    }

    Vector3 GetCameraPositionForGrid(int gridNum)
    {
        // TODO: This is a problem with negative grid numbers. Probably should just set world boundaries.
        int xStart = gridNum / Game.Instance.worldGridsWide,
            zStart = gridNum % Game.Instance.worldGridsWide;
        return new Vector3(
            xStart * Game.Instance.gridWidth + Game.Instance.gridWidth / 2f,
            Camera.main.transform.position.y,
            zStart * Game.Instance.gridHeight + Game.Instance.gridHeight / 2f
        );
    }
}
