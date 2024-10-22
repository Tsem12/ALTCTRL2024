using System.Linq;
using UnityEngine;

public static class ToolBox
{
    public static float Epsilone = 0.05f;
    
    public static bool Approximately(float a, float b, float epsilone)
    {
        return (double) Mathf.Abs(b - a) < (double) Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), epsilone * 8f);
    }

    public static Vector3 Bezier(Vector3[] points, float time)
    {
        if (points == null || points.Length == 0)
            return Vector3.zero;
        if (points.Length == 1)
            return points[0];
        if (points.Length == 2)
            return Vector3.Lerp(points[0], points[1], time);

        Vector3[] array1 = points.Take(points.Length - 1).ToArray();
        Vector3[] array2 = points.Skip(1).ToArray();
        return Vector3.Lerp(Bezier(array1, time), Bezier(array2, time), time);
    }
}