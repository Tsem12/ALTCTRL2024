using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Curve
{
    [field: SerializeField] public bool EnableEditPositionsOnScene { get; private set; }
    [field: SerializeField] public bool EditPoints { get; private set; }
    [field: SerializeField, ReadOnly] public Vector3[] Points { get; private set;}
    
    public Vector3 Barycenter
    {
        get
        {
            Vector3 result = Vector3.zero;
            for (int i = 0; i < Points.Length; i++)
            {
                result += Points[i];
            }
            result /= Points.Length;
            return result;
        }
    }
    
    private Vector3 GetPosition(float t) => ToolBox.Bezier(Points, t);
    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix) => localToWorldMatrix.MultiplyPoint(GetPosition(t));

    public void DrawGizmo(Color c, Matrix4x4 localToWorldMatrix, bool isSelected, float curveGizmoPrecision)
    {
        #if UNITY_EDITOR
        Gizmos.color = c;
        if (curveGizmoPrecision <= 0)
            curveGizmoPrecision = 0.1f;
        for (float i = 0f; i < 1; i += 1f * curveGizmoPrecision)
        {
            Gizmos.DrawLine(GetPosition(i, localToWorldMatrix), GetPosition(i + 1f * curveGizmoPrecision, localToWorldMatrix));
        }
        
        if(!isSelected)
            return;
        
        
        for (int i = 0; i < Points.Length; i++)
        {
            Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(Points[i]), .1f);
        }
        
        GUIStyle style = new GUIStyle()
        {
            fontSize = 20 
        };
        style.normal.textColor = Color.black;
        Handles.color = c;
        for (int i = 0; i < Points.Length; i++)
        {
            Handles.Label(localToWorldMatrix.MultiplyPoint(Points[i]) - new Vector3(-.25f, -.25f, 0), i.ToString(), style);
        }
        #endif
    }
}