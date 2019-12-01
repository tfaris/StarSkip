using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public GameObject backgroundPlane;
    public ZoneType zoneType;

    Renderer backgroundRenderer;
    bool _positionChecked;

    List<GameObject> enemies = new List<GameObject>();

    bool _playerHasDiscovered = false;

    public int DestroyedEnemiesCount
    {
        get; private set;
    }

    public enum ZoneType
    {
        Monster,
        Empire,
        Robo
    }

    void Start()
    {
        backgroundRenderer = backgroundPlane.GetComponent<Renderer>();
        var spawners = this.GetComponentsInChildren<EnemySpawner>();

        foreach (EnemySpawner esp in spawners)
        {
            esp.enemySpawned += HandleEnemySpawned;
        }
    }

    void HandleEnemySpawned(object sender, GameObject enemyObject)
    {
        enemies.Add(enemyObject);
    }

    void OnEnemyDestroyed()
    {
        DestroyedEnemiesCount++;
        if (DestroyedEnemiesCount <= Game.Instance.goalEnemiesPerZone)
        {
            Game.Instance.ShowMessage(
                UItext.MessageType.EnemiesDestroyedGoalUpdate,
                this.name.Replace("(Clone)", string.Empty),
                DestroyedEnemiesCount.ToString(),
                Game.Instance.goalEnemiesPerZone.ToString()
            );
        }
        
        if (DestroyedEnemiesCount >= Game.Instance.goalEnemiesPerZone)
        {
            Game.Instance.uIManager.SetZoneCompleted(this);
        }
    }

    public bool ZoneContainsPoint(Vector3 point)
    {
        // we don't care about y
        point.y = backgroundRenderer.bounds.min.y;
        return backgroundRenderer.bounds.Contains(point);
    }

    void Update()
    {
        if (Game.Instance && Game.Instance.IsSetup)
        {
            if (Game.Instance.playerShip)
            {
                if (!_playerHasDiscovered)
                {
                    Bounds zoneBounds = backgroundRenderer.bounds;
                    if (zoneBounds.Contains(Game.Instance.playerShip.transform.position))
                    {
                        _playerHasDiscovered = true;
                        Game.Instance.ShowMessage(UItext.MessageType.NewArea, this.name.Replace("(Clone)", string.Empty));
                    }
                }
            }

            for (int i=0; i < enemies.Count; i++)
            {
                GameObject enemyObj = enemies[i];
                if (!enemyObj)
                {
                    enemies.RemoveAt(i--);
                    OnEnemyDestroyed();
                }
            }

            // Make sure the zone fits in the world.
            if (!_positionChecked)
            {
                CheckZoneWorldPosition();
                _positionChecked = true;
            }
        }
    }

    ///
    /// Check the world positioning of this zone and adjust it into the
    /// world bounds if it is outside.
    ///
    void CheckZoneWorldPosition()
    {
        Vector3 backgroundPos = backgroundPlane.transform.position;

        Rect worldBounds = Game.Instance.WorldBoundaries;
        Bounds zoneBounds = backgroundRenderer.bounds;

        if (zoneBounds.min.x < worldBounds.min.x)
        {
            backgroundPos.x += worldBounds.min.x - zoneBounds.min.x;
        }
        else if (zoneBounds.max.x > worldBounds.max.x)
        {
            backgroundPos.x -= zoneBounds.max.x - worldBounds.max.x;
        }
        
        if (zoneBounds.min.z < worldBounds.min.y)
        {
            backgroundPos.z += worldBounds.min.y - zoneBounds.min.z;
        }
        else if (zoneBounds.max.z > worldBounds.max.y)
        {
            backgroundPos.z -= zoneBounds.max.z - worldBounds.max.y;
        }

        backgroundPlane.transform.position = backgroundPos;

        var newBounds = backgroundRenderer.bounds;
        for (float x = newBounds.min.x; x < newBounds.max.x; x += Game.Instance.gridWidth)
        {
            for (float z = newBounds.min.z; z < newBounds.max.z; z += Game.Instance.gridHeight)
            {
                int gp = Game.Instance.GetGrid(new Vector3(x, 0, z));
                GridState gs = Game.Instance.GetGridState(gp);
                gs.isEnemyArea = true;
            }
        }
    }
}
