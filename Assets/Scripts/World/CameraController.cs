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
            CenterCameraOn(trackThis.transform);
        }
    }

    public void CenterCameraOn(Transform transform)
    {
        float tmpY = Camera.main.transform.position.y;
        var pos = Game.Instance.GetCenterPositionForGrid(Game.Instance.GetGrid(transform.position));
        pos.y = tmpY;
        Camera.main.transform.position = pos;
    }
}
