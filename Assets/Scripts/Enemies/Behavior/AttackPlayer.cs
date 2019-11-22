using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    public GameObject attackThis;
    public float maxDistanceOfAttack;
    public float attackSpeed;
    [Range(0, 100)]
    public float precision = 100;
    Ship _ship;
    bool _attacking;

    // Start is called before the first frame update
    void Start()
    {
        _ship = GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackThis != null && !_attacking)
        {
            float dist = Mathf.Abs((this.transform.position - attackThis.transform.position).magnitude);
            if (dist <= maxDistanceOfAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        _attacking = true;

        Weapon weapon = _ship.GetCurrentWeapon();

        Vector3 target = CalculateInterceptCourse(
            attackThis.transform.position,
            attackThis.GetComponent<Rigidbody>().velocity,
            this.transform.position,
            weapon.speed
        );

        float maxPrecision = 100;
        float randomRange = maxPrecision - precision;
        target.x += Random.Range(-randomRange, randomRange);
        target.z += Random.Range(-randomRange, randomRange);

        this.transform.rotation = Quaternion.LookRotation(
            target,
            this.transform.up
        );
        _ship.FireWeapon();
        yield return new WaitForSeconds(attackSpeed);
        _attacking = false;
    }

    //
    //
    // credit: https://answers.unity.com/questions/296949/how-to-calculate-a-position-to-fire-at.html
    //
    public static Vector3 CalculateInterceptCourse(
        Vector3 aTargetPos,
        Vector3 aTargetSpeed,
        Vector3 aInterceptorPos,
        float aInterceptorSpeed
    )
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;
        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;
        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }

}
