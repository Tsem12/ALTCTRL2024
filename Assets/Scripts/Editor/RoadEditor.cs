using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road)), CanEditMultipleObjects]
public class RoadEditor : Editor
{
    private Road _road;

    private void OnEnable()
    {
        _road = (Road)target;
    }

    protected void OnSceneGUI()
    {
        Matrix4x4 localToWorldMatrix = _road.transform.localToWorldMatrix;
        Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
      
        if (!_road.EditPoints)
        {
            
            EditorGUI.BeginChangeCheck();
            Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_road.Barycenter), Quaternion.identity);
            Vector3 translation = worldToLocalMatrix.MultiplyPoint(vector) - _road.Barycenter;
            if (EditorGUI.EndChangeCheck())
            {
                for (int k = 0; k < _road.Points.Length; k++)
                {
                    Undo.RecordObject(_road, "Change FreeFollowView");
                    _road.Points[k] +=  translation;
                    EditorUtility.SetDirty(_road);
                }
            }
        }
        else
        {
            for (int k = 0; k < _road.Points.Length; k++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_road.Points[k]), Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_road, "Change FreeFollowView");
                    _road.Points[k] = worldToLocalMatrix.MultiplyPoint(vector);
                    EditorUtility.SetDirty(_road);
                }
            }
        }
    }
}
