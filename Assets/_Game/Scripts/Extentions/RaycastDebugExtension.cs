using UnityEngine;

public static class RaycastDebugExtension
{
    /// <summary>
    /// Vẽ một dấu "X" tại vị trí
    /// </summary>
    public static void DrawCross(Vector3 pos, float size, Color color, float duration = 0)
    {
        Debug.DrawLine(pos - Vector3.right * size, pos + Vector3.right * size, color, duration);
        Debug.DrawLine(pos - Vector3.up * size, pos + Vector3.up * size, color, duration);
        Debug.DrawLine(pos - Vector3.forward * size, pos + Vector3.forward * size, color, duration);
    }

    /// <summary>
    /// Vẽ vòng tròn theo normal (vuông góc với mặt phẳng)
    /// </summary>
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color, float duration = 0, int segments = 32)
    {
        normal.Normalize();
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
            tangent = Vector3.Cross(normal, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent);

        Vector3 prevPoint = center + tangent * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i * 2 * Mathf.PI) / segments;
            Vector3 nextPoint = center + (tangent * Mathf.Cos(angle) + bitangent * Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
        }
    }

    /// <summary>
    /// Vẽ hình trụ (cylinder) từ p1 đến p2
    /// </summary>
    public static void DrawCylinder(Vector3 p1, Vector3 p2, float radius, Color color, float duration = 0, int segments = 24)
    {
        Vector3 axis = (p2 - p1).normalized;
        Vector3 tangent = Vector3.Cross(axis, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
            tangent = Vector3.Cross(axis, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(axis, tangent);

        Vector3[] circle1 = new Vector3[segments + 1];
        Vector3[] circle2 = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i * 2 * Mathf.PI) / segments;
            Vector3 dir = tangent * Mathf.Cos(angle) + bitangent * Mathf.Sin(angle);

            circle1[i] = p1 + dir * radius;
            circle2[i] = p2 + dir * radius;
        }

        for (int i = 0; i < segments; i++)
        {
            Debug.DrawLine(circle1[i], circle1[i + 1], color, duration);
            Debug.DrawLine(circle2[i], circle2[i + 1], color, duration);
            Debug.DrawLine(circle1[i], circle2[i], color, duration);
        }
    }

    /// <summary>
    /// Vẽ capsule (cylinder + 2 bán cầu)
    /// </summary>
    public static void DrawCapsule(Vector3 p1, Vector3 p2, float radius, Color color, float duration = 0, int segments = 24)
    {
        // Vẽ cylinder
        DrawCylinder(p1, p2, radius, color, duration, segments);

        // Vẽ circle ở 2 đầu
        DrawCircle(p1, (p1 - p2).normalized, radius, color, duration, segments);
        DrawCircle(p2, (p2 - p1).normalized, radius, color, duration, segments);
    }
}
