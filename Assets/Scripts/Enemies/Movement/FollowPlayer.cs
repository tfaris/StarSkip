using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float giveUpDistance;
    public float keepDistanceToPlayer;
    public float speed;
    // Corrects for jerky movement when keeping distance.
    public float distanceDeadzone = 1f;
    public bool slowOnApproach = true;
    public GameObject trackThisObject;

    float _directionAdjustmentTimer;
    bool _movingTowards = true;
    Rigidbody _body;

    ///
    /// Get whether the follower is in the deadzone where it will keep
    /// its preferred distance.
    ///
    public bool IsInDeadzone
    {
        get; private set;
    }

    ///
    /// Get whether the follower has given up following.
    ///
    public bool HasGivenUp
    {
        get; private set;
    }

    public bool IsIdle
    {
        get; private set;
    } = true;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (trackThisObject != null)
        {
            float maxKeepDist = keepDistanceToPlayer + distanceDeadzone, minKeepDist = keepDistanceToPlayer - distanceDeadzone;
            var direction = MathUtil.ToroidalDistance(trackThisObject.transform.position, transform.position);
            var dist = Mathf.Abs(direction.magnitude);

            IsInDeadzone = dist >= minKeepDist && dist <= maxKeepDist;

            float nowSpeed = speed;
            if (slowOnApproach)
            {
                float maxDistForMaxSpeed = speed;
                if ( dist > maxDistForMaxSpeed)
                {
                    nowSpeed = speed;
                }
                else
                {                
                    var distRatio=(dist - keepDistanceToPlayer)/(maxDistForMaxSpeed - keepDistanceToPlayer);            
                    // This is the extra speed above min speed he can go up too            
                    var diffSpeed = speed;
                    // Final calc 
                    nowSpeed = (distRatio * diffSpeed);
                }
            }
            
            if (!IsInDeadzone && (dist < keepDistanceToPlayer && keepDistanceToPlayer != 0))
            {
                HasGivenUp = false;
                IsIdle = false;

                // Move away
                if (_movingTowards)
                {
                    _movingTowards = false;
                }

                if (!_movingTowards)
                {
                    if (_body != null)
                    {
                        _body.MovePosition(transform.position - direction.normalized * -nowSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position -= direction.normalized * -nowSpeed * Time.deltaTime;
                    }
                }
            }
            else if ((giveUpDistance == 0 || dist < giveUpDistance) && !IsInDeadzone)
            {
                HasGivenUp = false;
                IsIdle = false;

                // Move towards
                if (!_movingTowards)
                {
                    _movingTowards = true;
                }

                if (_movingTowards)
                {
                    if (_body != null)
                    {
                        _body.MovePosition(transform.position + direction.normalized * nowSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position += direction.normalized * nowSpeed * Time.deltaTime;
                    }
                }
            }
            else if (dist >= giveUpDistance && !IsIdle)
            {
                HasGivenUp = true;
                IsIdle = true;
            }
            else if (IsIdle)
            {
                HasGivenUp = false;
            }

            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }
    }
}
