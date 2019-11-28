using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperLaser : Weapon
{
    public float timeToStartDiminish = 1f;
    public float timeToDiminish = 1f;
    public float shipPushbackForce = .5f;
    bool noDiminishUntilPlayerLetsGo = false;

    Vector3 _scale = new Vector3(1, 1, 1);

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startPosition)
    {
        float dimStartCounter = 0, dimCounter = 0;

        var shipBody = FromShip.GetComponent<Rigidbody>();

        // todo: we're probably going to use a particle system, not LR
        LineRenderer lr = projectileObj.GetComponentInChildren<LineRenderer>();
        lr.transform.localScale = Vector3.Scale(lr.transform.localScale, _scale);

        BoxCollider bc = projectileObj.GetComponent<BoxCollider>();
        // todo: this could end up with collider behind us
        //bc.size = Vector3.Scale(bc.size, _scale);

        float startWidth = lr.startWidth, widthT = 0;

        while (projectileObj != null && FromShip != null)
        {
            // Laser stays facing out front of the ship
            var direction = FromShip.transform.forward;
            projectileObj.transform.position = FromShip.transform.position;
            projectileObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            if (Mathf.Abs(shipBody.velocity.magnitude) < 10)
            {
                shipBody.AddForce(-direction.normalized * shipPushbackForce, ForceMode.Impulse);
            }

            if (dimStartCounter >= timeToStartDiminish)
            {
                lr.startWidth = lr.endWidth = Mathf.Lerp(0, startWidth, 1f - widthT);
                dimCounter += Time.deltaTime;
                widthT += Time.deltaTime * 1f;
            }

            yield return base.ProjectileUpdate(projectileObj, startPosition);

            if (!noDiminishUntilPlayerLetsGo || (noDiminishUntilPlayerLetsGo && !FromShip.IsActivatingSuperLaser))
            {
                dimStartCounter += Time.deltaTime;
            }

            if (dimCounter >= timeToStartDiminish)
            {
                GameObject.Destroy(projectileObj);
            }
        }

        if (projectileObj)
        {
            GameObject.Destroy(projectileObj);
        }
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            _scale = new Vector3(2.5f, 1, 1.5f);
            damage = Mathf.RoundToInt(damage * 1.5f);
        }
        else if (newLevel == 3)
        {
            timeToStartDiminish = 3f;
            timeToDiminish = 3f;
        }
        else if (newLevel == 4)
        {
            _scale = new Vector3(4.5f, 1, 2.5f);
            damage = Mathf.RoundToInt(damage * 1.5f);
        }
        else if (newLevel == 5)
        {
            noDiminishUntilPlayerLetsGo = true;
        }
    }

    public override void AddToShip(Ship ship)
    {
        if (ship is PlayerShip)
        {
            ship.superLaserInstance = this;
        }
        else
        {
            ship.weapons.Add(this);
        }
        
        if (ship is PlayerShip)
        {
            Game.Instance.ShowMessage(UItext.MessageType.WeaponAcquired, "Super Laser", "LT, x, or right click");
        }
    }

    protected override void OnDamageableCollision(GameObject sourceObject, IDamageable damageable)
    {
        damageable.ApplyDamage(this && this.gameObject != null ? this.gameObject : null , this.damage);
    }
}
