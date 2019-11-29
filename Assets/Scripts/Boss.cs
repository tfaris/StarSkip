using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    public int weakPointHealth = 100;
    public int hurtBoxDamage = 15;
    public float hurtBoxPushForce = 25;

    public List<Collider> hurtBoxes = new List<Collider>();

    public List<Collider> weakPoints = new List<Collider>();

    public GameObject weakPointDamageEffect, weakPointDestructionEffect;

    Dictionary<Collider, int> _weakPointHealthRemaining = new Dictionary<Collider, int>();

    Planet _rotator;
    
    void Start()
    {
        _rotator = GetComponentInChildren<Planet>();
        foreach (Collider c in weakPoints)
        {
            _weakPointHealthRemaining[c] = weakPointHealth;
        }
    }

    void Update()
    {
        if (_weakPointHealthRemaining.Count == 0)
        {
            OnDestroyed();
            Game.Instance.ShowMessage(UItext.MessageType.GameOverWin, Game.Instance.GetRank().ToString());
            GameObject.Destroy(this.gameObject);
        }
    }

    void OnDestroyed()
    {
        if (weakPointDestructionEffect)
        {
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            for (int i=0; i < 10; i++)
            {
                Vector3 explodePoint = new Vector3(
                    Random.Range(mr.bounds.min.x, mr.bounds.max.x),
                    0,
                    Random.Range(mr.bounds.min.z, mr.bounds.max.z)
                );
                GameObject.Instantiate(weakPointDestructionEffect, explodePoint, Quaternion.identity);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint cp = collision.GetContact(0);
        if (hurtBoxes.Contains(cp.thisCollider))
        {
            var dmg = collision.gameObject.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.ApplyDamage(this.gameObject, collision.collider, this.hurtBoxDamage);
                var otherBody = collision.gameObject.GetComponent<Rigidbody>();
                otherBody.AddForce(-cp.normal * hurtBoxPushForce, ForceMode.Impulse);
            }
        }
    }

    public bool ApplyDamage(GameObject source, Collider collider, int damage)
    {
        // Don't take damage from non-weapons.
        var firedFromWeapon = source.GetComponent<Weapon>();
        if (firedFromWeapon != null)
        {
            if (_weakPointHealthRemaining.TryGetValue(collider, out int wpHealth))
            {
                wpHealth -= damage;
                _weakPointHealthRemaining[collider] = wpHealth;
                if (wpHealth <= 0)
                {
                    OnWeakPointDestroyed(collider);
                }
                else
                {
                    if (weakPointDamageEffect != null)
                    {
                        GameObject.Instantiate(weakPointDamageEffect, collider.gameObject.transform.position, Quaternion.identity);
                    }
                }
                return true;
            }
            if (hurtBoxes.Contains(collider))
            {
                // Return true so the weapon projectile is destroyed, but don't
                // actually apply any damage to the boss on hurtboxes.
                return true;
            }
        }
        return false;
    }

    void OnWeakPointDestroyed(Collider weakPointCollider)
    {
        if (weakPointDestructionEffect != null)
        {
            GameObject.Instantiate(weakPointDestructionEffect, weakPointCollider.gameObject.transform.position, Quaternion.identity);
        }
        GameObject.Destroy(weakPointCollider.gameObject);
        _weakPointHealthRemaining.Remove(weakPointCollider);

        _rotator.speed += 2.5f;
    }
}
