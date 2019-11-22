using UnityEngine;

///
/// Represents anything that can be damaged.
///
public interface IDamageable
{
    ///
    /// Apply the specified amount of damage, applied by a specific
    /// damage source.
    ///
    void ApplyDamage(GameObject source, int damage);
}