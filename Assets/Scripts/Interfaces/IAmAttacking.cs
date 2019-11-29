using UnityEngine;

public interface IAmAttacking
{
    ///
    /// Get the object that is being attacked.
    ///
    GameObject AttackingThis { get; }
}