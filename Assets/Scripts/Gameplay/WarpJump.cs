using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WarpJump : MonoBehaviour
{
    public ParticleSystem warpEffect;
    public int jumpSpaces;
    public int spacesPerWarpPoint = 3;
    public AudioClip warpStartingSoundEffect;
    public AudioSource warpSoundSource;

    public bool IsJumping {get; private set; }

    void ToggleColliders(bool enable)
    {
        foreach (var collider in GetComponents<Collider>().Concat(GetComponentsInChildren<Collider>()))
        {
            collider.enabled = enable;
        }
    }

    public IEnumerator Warp()
    {
        IsJumping = true;

        ToggleColliders(false);

        if (warpEffect)
        {
            warpEffect.gameObject.SetActive(true);
        }
        if (warpStartingSoundEffect)
        {
            Game.Instance.effectsAudioSource.PlayOneShot(warpStartingSoundEffect);
            yield return new WaitForSeconds(4.2f);
        }
        if (warpSoundSource)
        {
            warpSoundSource.Play();
        }

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

        
        if (warpSoundSource)
        {
            warpSoundSource.Stop();
        }

        if (warpEffect)
        {
            warpEffect.gameObject.SetActive(false);
        }

        ToggleColliders(true);

        IsJumping = false;
    }
}
