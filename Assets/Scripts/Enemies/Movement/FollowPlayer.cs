using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float giveUpDistance;
    public float keepDistanceToPlayer;
    public float speed;

    float _directionAdjustmentTimer;
    bool _movingTowards = true;
    Rigidbody _body;

    void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var playerT = Game.Instance.playerShip.transform;
        var direction = playerT.position - transform.position;
        var dist = Mathf.Abs(direction.magnitude);

        if ((giveUpDistance == 0 || (dist < giveUpDistance)) && dist > keepDistanceToPlayer)
        {
            if (!_movingTowards)
            {
                _movingTowards = true;
            }

            if (_movingTowards)
            {
                _body.MovePosition(transform.position + direction * speed * Time.deltaTime);
            }
        }
        // .2f for fudge
        else if (dist + .2f <= keepDistanceToPlayer)
        {
            if (_movingTowards)
            {
                _movingTowards = false;
            }

            if (!_movingTowards)
            {
                _body.MovePosition(transform.position - direction * speed * Time.deltaTime);
            }
        }
    }
}
