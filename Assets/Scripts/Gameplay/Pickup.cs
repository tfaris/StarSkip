using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject itemPrefab;
    public float maxAttractDist = 100f;
    public float attractSpeed = 4f;
    [Tooltip("Despawn after this many seconds if not picked up. If 0, never despawns.")]
    public float despawnTimer = 0;
    public AudioClip pickupSound;
    float _despawnCounter;
    int _prevMapPos = -999;

    void Update()
    {
        var pos = this.transform.position;
        
        // Move towards the player if they are nearby
        if (Game.Instance.playerShip)
        {
            var dir = MathUtil.ToroidalDistance(Game.Instance.playerShip.transform.position, this.transform.position);
            var dist = Mathf.Abs(dir.magnitude);
            if (dist <= maxAttractDist)
            {
                pos += dir.normalized * attractSpeed * Time.deltaTime;
            }
        }
        
        pos.y = 0;
        this.transform.position = pos;
        UpdateMap(this);

        Game.Instance.CheckWorldWrap(this.transform);

        if (this.despawnTimer != 0)
        {
            if (_despawnCounter >= this.despawnTimer)
            {
                UpdateMap(false);
                GameObject.Destroy(this.gameObject);
            }
            _despawnCounter += Time.deltaTime;
        }
    }

    void UpdateMap(bool exists)
    {
        // Highlight pickups as important on the map.
        int gridPos = Game.Instance.GetGrid(this.transform.position);
        GridState gs = Game.Instance.GetGridState(gridPos);
        gs.hasQuestObjective = exists;
        
        if (gridPos != _prevMapPos)
        {
            GridState oldgs = Game.Instance.GetGridState(_prevMapPos);
            oldgs.hasQuestObjective = false;
            _prevMapPos = gridPos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var ps = other.gameObject.GetComponentInParent<PlayerShip>();
        if (ps != null)
        {
            GameObject instance = GameObject.Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, ps.transform);
            instance
                .GetComponent<IAddToShip>()
                .AddToShip(ps);
            if (pickupSound)
            {
                Game.Instance.effectsAudioSource.PlayOneShot(pickupSound);
            }
            UpdateMap(false);
            GameObject.Destroy(this.gameObject);
        }
    }
}
