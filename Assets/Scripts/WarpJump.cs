using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WarpJump : MonoBehaviour
{
    public int jumpSpaces;

    public bool IsJumping {get; private set;}

    void Update()
    {
        if (!IsJumping && Input.GetButtonUp("Warp Jump"))
        {
            StartCoroutine(Warp());
        }
    }

    void ToggleColliders(bool enable)
    {
        foreach (var collider in GetComponents<Collider>().Concat(GetComponentsInChildren<Collider>()))
        {
            collider.enabled = enable;
        }
    }

    IEnumerator Warp()
    {
        IsJumping = true;

        ToggleColliders(false);

        var ship = Game.Instance.playerShip;

        var forwardRounded = new Vector3(
            Mathf.Round(ship.transform.forward.x),
            0,
            Mathf.Round(ship.transform.forward.z));

        Vector3 nextGridWorldLocation = ship.transform.position;
        for (int i = 0; i < jumpSpaces; i++)
        {
            var x = new Vector3(forwardRounded.x * Game.Instance.gridWidth, 0, forwardRounded.z * Game.Instance.gridHeight);
            nextGridWorldLocation += x;

            ship.transform.position = nextGridWorldLocation;

            yield return new WaitForSeconds(.5f);

            // pull back from user. If they jumped out of world bounds and wrapped,
            // this will get us the right location.
            nextGridWorldLocation = ship.transform.position;
        }

        ToggleColliders(true);

        IsJumping = false;
    }
}
