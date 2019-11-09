using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    CameraController cc;

    public Text labelSector;


    void Start()
    {
        cc = FindObjectOfType<CameraController>();    
    }

    // Update is called once per frame
    void Update()
    {
        int gridNum = Game.Instance.GetCurrentGrid();
        labelSector.text = "Sector: " + Game.Instance.GetSectorName(gridNum);
    }
}
