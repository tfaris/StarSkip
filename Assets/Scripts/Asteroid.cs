using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { goUp, goDown, goRight, goLeft };

public class Asteroid : MonoBehaviour
{
    public Direction goDirection;

    public bool speedy = false;

    float speed = 4;

    public float spin;
    public Rigidbody rb;

    float rh;
    float rv;

    bool spinGo = true;
    bool pushGo = true;

    void Start()
    {
        rh = Random.Range(-.3f, .3f);
        rv = Random.Range(-.3f, .3f);
        rb = GetComponent<Rigidbody>();

        speed = Random.Range(3,7);
        if (speedy)
        {
            speed = speed * 2;
        }
    }

    void FixedUpdate()
    {
        if (spinGo == true)
        {
            addSpin();
            spinGo = false;
        }

        if (pushGo == true)
        {
            if (goDirection == Direction.goUp)
            {
                addPush(Random.Range(-.8f,.8f), 1);
            }
            else if (goDirection == Direction.goDown)
            {
                addPush(Random.Range(-.8f, .8f), -1);
            }
            else if (goDirection == Direction.goRight)
            {
                addPush(1, Random.Range(-.8f, .8f));
            }
            else
            {
                addPush(-1, Random.Range(-.8f, .8f));
            }
            pushGo = false;
        }
    }

    void addSpin()
    {
        rb.AddTorque(transform.up * rh, ForceMode.Impulse);
        rb.AddTorque(transform.right * rv, ForceMode.Impulse);
    }

    void addPush(float pushx, float pushz)
    {
        rb.AddForce(new Vector3(pushx,0,pushz) * speed, ForceMode.Impulse);
    }
}
