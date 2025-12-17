using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParabolaPointsGeneration
{
    public static Vector3[] GenerateParabolaPoints(Vector3 start, Vector3 end, float height, int resolution = 20, bool directionLeft = false, AxisConstraint axis = AxisConstraint.Y)
    {
        Vector3[] points = new Vector3[resolution + 1];
        float directValue = directionLeft ? -1 : 1;
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            Vector3 point = Vector3.Lerp(start, end, t);
            float yOffset = Mathf.Sin(t * Mathf.PI) * height;
            
            if (axis == AxisConstraint.Y)
            {
                point.y += directValue * yOffset;
            }else if (axis == AxisConstraint.X)
            {
                point.x += directValue * yOffset;
            }

            points[i] = point;
        }
        return points;
    }
}
