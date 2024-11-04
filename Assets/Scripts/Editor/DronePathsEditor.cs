using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DronePaths)), CanEditMultipleObjects]
public class DronePathsEditor : Editor
{
    private DronePaths _pigeonPaths;

    private void OnEnable()
    {
        _pigeonPaths = (DronePaths)target;
    }

    protected void OnSceneGUI()
    {
        Matrix4x4 localToWorldMatrix = _pigeonPaths.transform.localToWorldMatrix;
        Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
        for (int i = 0; i < _pigeonPaths.Paths.Curves.Length; i++)
        {
            if(!_pigeonPaths.Paths.Curves[i].EnableEditPositionsOnScene)
                continue;

            if (!_pigeonPaths.Paths.Curves[i].EditPoints)
            {
                
                EditorGUI.BeginChangeCheck();
                Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths.Curves[i].Barycenter), Quaternion.identity);
                Vector3 translation = worldToLocalMatrix.MultiplyPoint(vector) - _pigeonPaths.Paths.Curves[i].Barycenter;
                if (EditorGUI.EndChangeCheck())
                {
                    for (int k = 0; k < _pigeonPaths.Paths.Curves[i].Points.Length; k++)
                    {
                        Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                        _pigeonPaths.Paths.Curves[i].Points[k] +=  translation;
                        EditorUtility.SetDirty(_pigeonPaths);
                    }
                }
            }
            else
            {
                for (int k = 0; k < _pigeonPaths.Paths.Curves[i].Points.Length; k++)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths.Curves[i].Points[k]), Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                        _pigeonPaths.Paths.Curves[i].Points[k] = worldToLocalMatrix.MultiplyPoint(vector);
                        EditorUtility.SetDirty(_pigeonPaths);
                    }
                }
            }
        }
            
        
    }
}
