using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    public float speed, probability;
    float shootTimer;

    bool shoot = false, shooting = false;
    Vector3 start, end;
    TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        StartCoroutine(DoComets());
    }

    IEnumerator DoComets()
    {
        while (true)
        {
            if (Game.Instance != null)
            {
                if (!shooting)
                {
                    this.transform.position = Game.Instance.GetOutOfBoundsPosition();
                    shoot = Random.Range(0f, 1f) > (1f - probability);

                    if (shoot)
                    {
                        if (shootTimer != 0)
                        {
                            shootTimer = 0;
                        }
                        else
                        {
                            bool startHigh = Random.Range(0, 1f) > .5f;

                            shootTimer = 0;
                            Rect worldCameraBounds = Game.Instance.GetBoundsForGrid(Game.Instance.GetCurrentGrid());
                            start = new Vector3(
                                Random.Range(worldCameraBounds.xMin, worldCameraBounds.xMax),
                                0,
                                startHigh ? worldCameraBounds.yMax + 5 : worldCameraBounds.yMin - 5
                            );
                            end = new Vector3(
                                Random.Range(worldCameraBounds.xMin, worldCameraBounds.xMax),
                                0,
                                startHigh ? worldCameraBounds.yMin - 5 : worldCameraBounds.yMax + 5
                            );
                            if (trail != null)
                            {
                                trail.enabled = false;
                                yield return new WaitForSeconds(1f);
                            }
                            this.transform.position = start;
                            if (trail != null)
                            {
                                trail.enabled = true;
                            }
                            shooting = true;
                            shoot = false;
                        }
                    }
                }
                else
                {
                    this.transform.position = Vector3.Lerp(start, end, shootTimer);
                    shootTimer += Time.deltaTime * speed;

                    if (shootTimer > 1)
                    {
                        shooting = shoot = false;
                        if (trail != null)
                        {
                            yield return new WaitForSeconds(trail.time);
                        }
                        this.transform.position = Game.Instance.GetOutOfBoundsPosition();
                    }
                }
            }
            yield return new WaitForSeconds(0);
        }
    }
}
