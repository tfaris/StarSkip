using UnityEngine;

///
/// Represents anything that can be damaged.
///
public interface IDamageable
{
    ///
    /// Apply the specified amount of damage, applied by a specific
    /// damage source. Returns true if this object accepted the damage.
    ///
    bool ApplyDamage(GameObject source, Collider collider, int damage);
}