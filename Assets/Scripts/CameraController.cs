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
            Camera.main.transform.position = Game.Instance.GetCenterPositionForGrid(objGrid);
        }
    }

    public void CenterCameraOn(Transform transform)
    {
        Camera.main.transform.position = Game.Instance.GetCenterPositionForGrid(Game.Instance.GetGrid(transform.position));
    }
}
