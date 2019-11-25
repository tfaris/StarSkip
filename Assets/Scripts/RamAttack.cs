using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamAttack : MonoBehaviour
{
    public float ramSpeed;
    public float cooldownTimer;
    public int ramDamage;
    FollowPlayer _followScript;
    Rigidbody _body;
    bool _isRamming, _cooldown;
    float _cooldownCounter;
    Vector3 _lastRamDirection, _ramStartingPosition;

    void Start()
    {
        _followScript = GetComponent<FollowPlayer>();
        _body = GetComponent<Rigidbody>();

        if (ramSpeed == 0)
        {
            ramSpeed = _followScript.speed;
        }
    }

    void Update()
    {
        if (_followScript.trackThisObject != null)
        {
            if (_cooldown)
            {
                _cooldownCounter += Time.deltaTime;
                if (_cooldownCounter >= cooldownTimer)
                {
                    _cooldown = false;
                    _cooldownCounter = 0;
                }
            }
            else if (_isRamming)
            {
                _body.MovePosition(_body.position + _lastRamDirection.normalized * ramSpeed * Time.deltaTime);

                // We've gone too far
                if (Mathf.Abs(MathUtil.ToroidalDistance(_ramStartingPosition, this.transform.position).magnitude) > _followScript.keepDistanceToPlayer * 2)
                {
                    _isRamming = false;
                    _cooldown = true;
                    _followScript.enabled = true;
                }
            }
            else
            {
                if (_followScript.IsInDeadzone)
                {
                    // We're close enough to ram
                    _followScript.enabled = false;
                    _isRamming = true;
                    _ramStartingPosition = this.transform.position;
                    _lastRamDirection = MathUtil.ToroidalDistance(_followScript.trackThisObject.transform.position, _ramStartingPosition);
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        IDamageable dmg = other.gameObject.GetComponent<IDamageable>();
        if (dmg != null)
        {
            var otherBody = other.gameObject.GetComponent<Rigidbody>();
            if (otherBody != null)
            {
                otherBody.AddForce(_lastRamDirection.normalized * ramSpeed * .5f, ForceMode.Impulse);
            }
            dmg.ApplyDamage(this.gameObject, this.ramDamage);
            _isRamming = false;
            _cooldown = true;
            _followScript.enabled = true;
        }
    }
}
