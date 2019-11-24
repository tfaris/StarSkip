using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : Weapon
{
    public float maximumHomingDistance;
    [Tooltip("Target recalculation time.")]
    public float retargetTimer = 5f;
    int _limit = 1, _current = 0;
    float 
        // Amount of time the missile is allowed to spend homing.
        homingTimer = .2f, 
        // Amount of time the missile must spend in non-homing mode before
        // switching back to homing.
        nonHomingTimer = .5f, 
        // Number of total times the missile can attempt to retarget.
        maxHomingAttempts = 1;

    Vector3 _projectileScale = new Vector3(1, 1, 1);

    protected override void CheckCooldown()
    {
        if (IsInCooldown)
        {
            CooldownCounter += Time.deltaTime;
            if (CooldownCounter >= CooldownTimer && _current < _limit)
            {
                IsInCooldown = false;
            }
        }
    }

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startingPoint)
    {
        foreach (var partSystem in projectileObj.GetComponentsInChildren<ParticleSystem>())
        {
            partSystem.transform.localScale = Vector3.Scale(partSystem.transform.localScale, _projectileScale);
        }

        var sCollider = projectileObj.GetComponent<SphereCollider>();
        if (sCollider)
        {
            sCollider.radius *= _projectileScale.x;
        }

        _current++;
        Vector3 direction = FromShip.transform.forward,
            previousPos = projectileObj.transform.position;
        
        projectileObj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        bool removedFromCount = false;
        float homingCounter = 0, nonHomingCounter = 0, homingAttempts = 0;

        var fp = projectileObj.AddComponent<FollowPlayer>();
        fp.keepDistanceToPlayer = 0;
        fp.slowOnApproach = false;
        fp.giveUpDistance = 0;
        fp.speed = this.speed;
        while (projectileObj != null)
        {

            //
            // The basic idea is to allow the missile only a certain amount of time to home
            // in on the closest target, and only a limited number of times to retarget.
            // Upgrade levels increase these stats. It kind of works...
            //

            if (nonHomingCounter <= nonHomingTimer)
            {
                fp.trackThisObject = null;
                nonHomingCounter += Time.deltaTime;
            }
            else if (homingCounter <= homingTimer && homingAttempts < maxHomingAttempts)
            {
                GameObject target = GetTarget(projectileObj);
                if (target == null)
                {
                    target = FindClosestTarget(projectileObj);
                }
                if (target != null)
                {
                    fp.trackThisObject = target;
                    homingCounter += Time.deltaTime;
                    homingAttempts++;
                }
            }
            else
            {
                homingCounter = 0;
                nonHomingCounter = 0;
            }

            if (fp.trackThisObject == null)
            {
                projectileObj.transform.position += direction.normalized * speed * Time.deltaTime;
            }

            yield return base.ProjectileUpdate(projectileObj, startingPoint);

            if (projectileObj)
            {
                direction = MathUtil.ToroidalDistance(projectileObj.transform.position, previousPos);
                previousPos = projectileObj.transform.position;

                // If the missile goes out of screen, we allow another to be shot (once cooldown
                // has finished also).
                int playerGrid = Game.Instance.GetCurrentGrid(),
                    missileGrid = Game.Instance.GetGrid(projectileObj.transform.position);
                if (playerGrid != missileGrid && !removedFromCount)
                {
                    _current--;
                    removedFromCount = true;
                }
            }
        }
        if (!removedFromCount)
        {
            _current--;
        }
    }

    ///
    /// Get the current target.
    ///
    public GameObject GetTarget(GameObject projectileObj)
    {
        return FromShip.AttackingThis;
    }

    private bool ShouldCompare(Transform t)
    {
        return t.root != FromShip.transform;
    }

    public GameObject FindClosestTarget(GameObject projectileObj)
    {
        var ships = GameObject.FindObjectsOfType<Ship>();
        
        Transform closest = MathUtil.GetClosest(projectileObj.transform.position, ships.Select(sh => sh.transform), shouldCompare: this.ShouldCompare);
        if (closest != null)
        {
            var dist = Mathf.Abs(MathUtil.ToroidalDistance(closest.position, projectileObj.transform.position).magnitude);
            if (dist < maximumHomingDistance)
            {
                return closest.gameObject;
            }
        }
        return null;
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            _projectileScale = new Vector3(5, 5, 5);
            damage = Mathf.RoundToInt(damage * 1.5f);
        }
        else if (newLevel == 3)
        {
            _limit = 3;
        }
        else if (newLevel == 4)
        {
            homingTimer = 1f;
            nonHomingTimer = .3f;
            speed *= 1.5f;
            maxHomingAttempts = 2;
        }
        else if (newLevel == 5)
        {
            _limit = 5;
            _projectileScale = new Vector3(7, 7, 7);
            damage = Mathf.RoundToInt(damage * 1.5f);
        }
    }
}
