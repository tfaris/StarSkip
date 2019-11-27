using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : Weapon
{
    public float spreadDegrees = 90f;
    public float delayBetweenPellets = 0;
    [System.NonSerialized]
    public int numPellets = 3;
    [System.NonSerialized]
    public Vector3 scale = Vector3.one;
    

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startPosition)
    {
        var direction = FromShip.transform.forward;

        float spreadAngle = this.spreadDegrees / numPellets;
        float lowAngle = -spreadAngle * (numPellets / 2);

        List<GameObject> pellets = new List<GameObject>();
        for (int i=0; i < numPellets; i++)
        {
            GameObject pelletObj;
            if (i == 0)
            {
                pelletObj = projectileObj;
            }
            else
            {
                pelletObj = CreateProjectile();
                Game.Instance.StartCoroutine(base.CheckDestroy(pelletObj, startPosition));
                Game.Instance.StartCoroutine(CheckDestroy(pelletObj, startPosition));
            }

            foreach (var partSystem in pelletObj.GetComponentsInChildren<ParticleSystem>())
            {
                partSystem.transform.localScale = Vector3.Scale(partSystem.transform.localScale, this.scale);
            }
            var sCollider = pelletObj.GetComponent<SphereCollider>();
            if (sCollider)
            {
                sCollider.radius *= this.scale.x;
            }

            pelletObj.transform.position = startPosition;
            pelletObj.name = this.name + " " + (i + 1);
            
            float pelletAngle = i * spreadAngle + lowAngle;
            Debug.Log(i + " -> " + pelletAngle);
            // start off facing same way as ship
            pelletObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            Debug.Log(pelletObj.transform.rotation);
            // .... then apply the spread rotation
            pelletObj.transform.Rotate(0, pelletAngle, 0);
            Debug.Log(pelletObj.transform.rotation);
            pellets.Add(pelletObj);
            pelletObj.SetActive(false);
        }

        for (int i=0; i < pellets.Count; i++)
        {
            pellets[i].SetActive(true);
            startPosition = FromShip.transform.position;
            Game.Instance.StartCoroutine(PelletUpdate(i, pellets[i], startPosition));
            yield return new WaitForSeconds(delayBetweenPellets);
        }
    }

    protected virtual IEnumerator PelletUpdate(int pelletIndex, GameObject pelletObj, Vector3 startPosition)
    {
        pelletObj.transform.position = startPosition;
        while (pelletObj)
        {
            Vector3 pelletDir = GetDirectionForPellet(pelletIndex, pelletObj);
            pelletObj.transform.position += pelletDir * speed * Time.deltaTime;
            // If we yield here we get slow moving projectiles, so manually invoke
            // the enumerator for this function.
            yield return base.ProjectileUpdate(pelletObj, startPosition);
        }
    }

    protected virtual Vector3 GetDirectionForPellet(int pelletIndex, GameObject pelletObj)
    {
        return pelletObj.transform.forward;
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            CooldownTimer -= .15f;
        }
        else if (newLevel == 3)
        {
            numPellets = 5;
        }
        else if (newLevel == 4)
        {
            CooldownTimer -= .15f;
        }
        else if (newLevel == 5)
        {
            numPellets = 7;
        }
    }

    public override void AddToShip(Ship ship)
    {
        foreach (var ss in ship.weapons.OfType<StandardShot>())
        {
            ss.spreadshotUpgrade = this;
        }
    }
}
