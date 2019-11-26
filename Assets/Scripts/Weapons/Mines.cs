using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mines : Weapon
{
    float _explosionTimer = .1f;
    List<GameObject> 
        _minesOnScreen = new List<GameObject>(),
        _minesNotOnScreen = new List<GameObject>(),
        _explodingProjectiles = new List<GameObject>();
    int _limitPerScreen = 1;
    Vector3 _explosionSize = new Vector3(1, 1, 1);
    bool _explodeUntilPlayerLeaves;

    public override IEnumerator ProjectileUpdate(GameObject projectileObj, Vector3 startPosition)
    {
        bool isExploding = false;
        float explodingCounter = 0;

        // TODO: Probably need to interact with particle system to change length of explosion effect.
        foreach (var partSystem in projectileObj.GetComponentsInChildren<ParticleSystem>())
        {
            partSystem.transform.localScale = Vector3.Scale(partSystem.transform.localScale, _explosionSize);
        }
        var sCollider = projectileObj.GetComponent<SphereCollider>();
        if (sCollider)
        {
            sCollider.radius *= _explosionSize.x;
        }

        while (projectileObj != null)
        {
            _minesOnScreen.Remove(projectileObj);
            _minesNotOnScreen.Remove(projectileObj);

            if (!isExploding)
            {
                isExploding = _explodingProjectiles.Contains(projectileObj);
            }
            else
            {
                if (!_explodeUntilPlayerLeaves && explodingCounter >= _explosionTimer)
                {
                    _explodingProjectiles.Remove(projectileObj);
                    GameObject.Destroy(projectileObj);
                }
                explodingCounter += Time.deltaTime;
            }

            if (projectileObj)
            {
                int playerGrid = Game.Instance.GetCurrentGrid();
                int mineGrid = Game.Instance.GetGrid(projectileObj.transform.position);
                if (mineGrid == playerGrid)
                {
                    _minesOnScreen.Add(projectileObj);
                }
                else
                {
                    if (isExploding && _explodeUntilPlayerLeaves)
                    {
                        _explodingProjectiles.Remove(projectileObj);
                        GameObject.Destroy(projectileObj);
                    }
                    _minesNotOnScreen.Add(projectileObj);
                }
            }

            yield return base.ProjectileUpdate(projectileObj, startPosition);
        }
        _explodingProjectiles.Remove(projectileObj);
        _minesOnScreen.Remove(projectileObj);
        _minesNotOnScreen.Remove(projectileObj);
    }

    protected override void OnDamageableCollision(GameObject sourceObject, IDamageable damageable)
    {
        // The update method needs to know when the mine objects are exploding
        if (!_explodingProjectiles.Contains(sourceObject))
        {
            _explodingProjectiles.Add(sourceObject);
        }

        damageable.ApplyDamage(this && this.gameObject != null ? this.gameObject : null , this.damage);
    }

    protected override void CheckCooldown()
    {
        if (IsInCooldown)
        {
            CooldownCounter += Time.deltaTime;
            if (CooldownCounter >= CooldownTimer)
            {
                if (_minesOnScreen.Count < _limitPerScreen)
                {
                    IsInCooldown = false;
                }
            }
        }
    }

    protected override void OnUpgraded(int oldLevel, int newLevel)
    {
        if (newLevel == 2)
        {
            _explosionTimer = 5;
        }
        else if (newLevel == 3)
        {
            _explosionSize = new Vector3(3, 3, 3);
        }
        else if (newLevel == 4)
        {
            _limitPerScreen = 2;
        }
        else if (newLevel == 5)
        {
            _explodeUntilPlayerLeaves = true;
            _explosionSize = new Vector3(8, 8, 8);
        }
    }

    public override void AddToShip(Ship ship)
    {
        if (ship is PlayerShip)
        {
            ship.minesWeaponInstance = this;
        }
        else
        {
            ship.weapons.Add(this);
        }
    }
}
