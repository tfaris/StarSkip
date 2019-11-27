using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBG : MonoBehaviour
{
    public GameObject bgRobo;
    public GameObject bgEmpire;
    public GameObject bgMonster;

    public bool inRobo;
    public bool inEmpire;
    public bool inMonster;


    public void CheckArea()
    {
        if (inRobo)
        {
            bgRobo.SetActive(true);
            bgEmpire.SetActive(false);
            bgMonster.SetActive(false);
        }
        else if (inEmpire)
        {
            bgRobo.SetActive(false);
            bgEmpire.SetActive(true);
            bgMonster.SetActive(false);
        }
        else if(inMonster)
        {
            bgRobo.SetActive(false);
            bgEmpire.SetActive(false);
            bgMonster.SetActive(true);
        }
        else
        {
            bgRobo.SetActive(false);
            bgEmpire.SetActive(false);
            bgMonster.SetActive(false);
        }
    }
}
