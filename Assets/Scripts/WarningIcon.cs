using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Doesn't work with wrapping.
public class WarningIcon : MonoBehaviour
{
    public GameObject warningIcon;
    public float stopWarningWhenThisFarFromGridEdge = 15;
    GameObject _warningIcon;

    // Update is called once per frame
    void Update()
    {
        int playerGrid = Game.Instance.GetCurrentGrid();
        int thisObjectGrid = Game.Instance.GetGrid(this.transform.position);
        
        if (playerGrid != thisObjectGrid)
        {
            Rect worldGridBounds = Game.Instance.GetBoundsForGrid(playerGrid);

            // Find intersecting point of the line between the center of the grid space
            // and this object. That's where we'll place the icon.
            var pos = PointOnRect(
                new Vector2(this.transform.position.x, this.transform.position.z),
                new Vector2(worldGridBounds.center.x, worldGridBounds.center.y),
                worldGridBounds.xMin, worldGridBounds.yMin, worldGridBounds.xMax, worldGridBounds.yMax
            );

            float distToEdge = Mathf.Abs((this.transform.position - pos).magnitude);
            if (distToEdge < stopWarningWhenThisFarFromGridEdge)
            {
                if (_warningIcon == null)
                {
                    _warningIcon = GameObject.Instantiate(warningIcon, Game.Instance.GetOutOfBoundsPosition(), Quaternion.identity);
                }
                _warningIcon.transform.position = pos;

                var rot = Quaternion.LookRotation(pos - this.transform.position, Vector3.up);
                rot.x = rot.z = 0;
                _warningIcon.transform.rotation = rot;
            }
            else
            {
                GameObject.Destroy(_warningIcon);
            }
        }
        else
        {
            GameObject.Destroy(_warningIcon);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_warningIcon != null)
        {
            Gizmos.DrawLine(transform.position, _warningIcon.transform.position);
        }
    }

    public static Vector3 PointOnRect(Vector2 pointA, Vector2 pointB, float minX, float minY, float maxX, float maxY) {
        var midX = pointB.x;// (minX + maxX) / 2;
        var midY = pointB.y;// (minY + maxY) / 2;
        // if (midX - x == 0) -> m == ±Inf -> minYx/maxYx == x (because value / ±Inf = ±0)
        float x = pointA.x, y = pointA.y;
        var m = (midY - y) / (midX - x);

        if (x <= midX) { // check "left" side
            var minXy = m * (minX - x) + y;
            if (minY <= minXy && minXy <= maxY)
                return new Vector3(minX,  0,minXy);
        }

        if (x >= midX) { // check "right" side
            var maxXy = m * (maxX - x) + y;
            if (minY <= maxXy && maxXy <= maxY)
                return new Vector3(maxX,  0,maxXy);
        }

        if (y <= midY) { // check "top" side
            var minYx = (minY - y) / m + x;
            if (minX <= minYx && minYx <= maxX)
                return new Vector3(minYx, 0, minY);
        }

        if (y >= midY) { // check "bottom" side
            var maxYx = (maxY - y) / m + x;
            if (minX <= maxYx && maxYx <= maxX)
                return new Vector3(maxYx, 0, maxY);
        }

        // edge case when finding midpoint intersection: m = 0/0 = NaN
        if (x == midX && y == midY)
        {
            return new Vector3(x, 0, y);
        }

        return Vector3.zero;
    }
}
