using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public Image select1;
    public Image select2;
    public Image select3;
    public Image select4;
    public Image select5;
    public Image select6;
    public Image select7;
    public Image select8;
    public Image select9;

    public Image star1;
    public Image star2;
    public Image star3;
    public Image star4;
    public Image star5;
    public Image star6;
    public Image star7;
    public Image star8;
    public Image star9;

    public Image warpJuice1;
    public Image warpJuice2;
    public Image warpJuice3;
    public Image warpJuice4;
    public Image warpJuice5;
    public Image warpJuice6;
    public Image warpJuice7;
    public Image warpJuice8;
    public Image warpJuice9;
    public Image warpJuice10;

    public Image hpBar;

    public Text textForPlayer;


    List<Image> juices = new List<Image>();
    List<Image> stars = new List<Image>();
    List<Image> selections = new List<Image>();

    int curWarps;
    float curHP;
    public int curUpgradeSelection;

    void Start()
    {
        juices.Add(warpJuice1);
        juices.Add(warpJuice2);
        juices.Add(warpJuice3);
        juices.Add(warpJuice4);
        juices.Add(warpJuice5);
        juices.Add(warpJuice6);
        juices.Add(warpJuice7);
        juices.Add(warpJuice8);
        juices.Add(warpJuice8);
        juices.Add(warpJuice10);

        stars.Add(star1);
        stars.Add(star2);
        stars.Add(star3);
        stars.Add(star4);
        stars.Add(star5);
        stars.Add(star6);
        stars.Add(star7);
        stars.Add(star8);
        stars.Add(star9);

        selections.Add(select1);
        selections.Add(select2);
        selections.Add(select3);
        selections.Add(select4);
        selections.Add(select5);
        selections.Add(select6);
        selections.Add(select7);
        selections.Add(select8);
        selections.Add(select9);
    }

    void Update()
    {
        curHP = hpBar.fillAmount;

        if (Input.GetButtonDown("Upgrade Selection Down"))
        {
            if (curUpgradeSelection == 0)
            {
                curUpgradeSelection = 8;
            }
            else
            curUpgradeSelection--;

            MoveUpgradeSelection();
        }
        if (Input.GetButtonDown("Upgrade Selection Up"))
        {
            if (curUpgradeSelection == 8)
            {
                curUpgradeSelection = 0;
            }
            else
            curUpgradeSelection++;

            MoveUpgradeSelection();
        }
    }


    public void AddJuice()
    {
        if (curWarps < 10)
        {
            curWarps++;
            juices[curWarps].enabled = true;
        }
    }

    public void emptyJuice()
    {
        curWarps = 0;
        foreach (Image image in juices)
        {
            image.enabled = false;
        }
    }

    public void ShowStar()
    {
        //
    }

    void MoveUpgradeSelection()
    {
        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].GetComponent<Image>().enabled = false;
        }
        selections[curUpgradeSelection].GetComponent<Image>().enabled = true;
    }

}
