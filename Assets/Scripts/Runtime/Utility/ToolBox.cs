using UnityEngine;

public class ToolBox
{
    public static float Epsilone = 0.05f;
    
    public static bool Approximately(float a, float b, float epsilone)
    {
        return (double) Mathf.Abs(b - a) < (double) Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), epsilone * 8f);
    }
}