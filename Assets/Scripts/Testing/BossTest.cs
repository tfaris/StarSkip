using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : MonoBehaviour
{
    Boss _boss;
    bool _b;

    void Start()
    {
        _boss = GameObject.FindObjectOfType<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_b && Game.Instance != null && Game.Instance.IsSetup)
        {
            Game.Instance.playerShip.transform.position = _boss.transform.position + new Vector3(50, 0, 0);
            _b = true;
        }
    }
}
