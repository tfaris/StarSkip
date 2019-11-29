using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { goUp, goDown, goRight, goLeft };

public class Asteroid : MonoBehaviour, IDamageable
{
    public Direction goDirection;
    public Pickup drop;
    public float dropChance;

    public bool speedy = false;

    public float speed = 4;
    [Tooltip("Amount of damage this asteroid does to things it hits.")]
    public int damage = 10;
    public int health = 10;

    public float spin;
    public Rigidbody rb;

    [HideInInspector]
    public AsteroidController spawner;

    public GameObject destructionEffect;

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

    void Update()
    {
        int playerGrid = Game.Instance.GetCurrentGrid();
        int asteroidGrid = Game.Instance.GetGrid(this.transform.position);
        // Don't despawn if we're on the same grid as the player.
        if (playerGrid != asteroidGrid)
        {
            var dist = Mathf.Abs((this.transform.position - spawner.transform.position).magnitude);
            if (dist > spawner.distanceToDespawn)
            {
                GameObject.Destroy(this.gameObject);
            }
        }

        // clear zones of asteroids
        // if (Random.Range(0f, 1f) < .5f)
        // {
        //     foreach (Zone zone in Game.Instance.Zones)
        //     {
        //         if (zone.ZoneContainsPoint(this.transform.position))
        //         {
        //             GameObject.Destroy(this.gameObject);
        //         }
        //     }
        // }

        Game.Instance.CheckWorldWrap(this.transform);
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

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 15);
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

    void OnCollisionEnter(Collision other)
    {
        // Don't destroy other asteroids
        if (other.gameObject.GetComponent<Asteroid>() != null)
        {
            other.rigidbody.AddForce(
                rb.velocity,
                ForceMode.Impulse
            );
        }
        else
        {
            IDamageable dmg = other.gameObject.GetComponent<IDamageable>();
            if (dmg != null)
            {
                var otherBody = other.gameObject.GetComponent<Rigidbody>();
                if (otherBody != null)
                {
                    otherBody.AddForce(
                        rb.velocity,
                        ForceMode.Impulse
                    );
                }
                dmg.ApplyDamage(this.gameObject, other.collider, this.damage);
            }
        }
    }

    public bool ApplyDamage(GameObject source, Collider collider, int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (Game.Instance.TrackingObject == this.gameObject)
            {
                Game.Instance.TrackingObject = source;
            }

            // Drop on destroy
            if (drop && Random.Range(0f, 1f) < dropChance)
            {
                GameObject.Instantiate(drop, this.transform.position, Quaternion.identity);
            }

            if (destructionEffect)
            {
                GameObject.Instantiate(destructionEffect, this.transform.position, Quaternion.identity);
            }
            GameObject.Destroy(this.gameObject);
        }
        return true;
    }
}
