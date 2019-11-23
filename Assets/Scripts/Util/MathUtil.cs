using UnityEngine;

public static class MathUtil
{
    ///
    /// Get distance from a to b taking into account the world wrap.
    ///
    // https://blog.demofox.org/2017/10/01/calculating-the-distance-between-points-in-wrap-around-toroidal-space/
    public static Vector3 ToroidalDistance(Vector3 a, Vector3 b)
    {
        var direction = a - b;
        float width = Game.Instance.WorldBoundaries.size.x, height = Game.Instance.WorldBoundaries.size.y;
        float distX = Mathf.Abs(direction.x), distZ = Mathf.Abs(direction.z);
        
        if (distX > width / 2)
        {
            // Wrap X
            if (b.x < width / 2)
            {
                direction.x = distX - width;
            }
            else
            {
                direction.x = width - distX;
            }
        }
        if (distZ > Game.Instance.WorldBoundaries.size.y / 2)
        {
            // Wrap Z
            if (b.z < height / 2)
            {
                direction.z = distZ - height;
            }
            else
            {
                direction.z = height - distZ;
            }
        }
        return direction;
    }
}