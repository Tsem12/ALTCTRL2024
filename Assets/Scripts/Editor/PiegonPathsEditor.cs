using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PigeonPaths)), CanEditMultipleObjects]
public class PiegonPathsEditor : Editor
{
    private PigeonPaths _pigeonPaths;

    private void OnEnable()
    {
        _pigeonPaths = (PigeonPaths)target;
    }

    protected void OnSceneGUI()
    {
        Matrix4x4 localToWorldMatrix = _pigeonPaths.transform.localToWorldMatrix;
        Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
        for (int i = 0; i < _pigeonPaths.Paths.Length; i++)
        {
            if(!_pigeonPaths.Paths[i].EnableEditPositionsOnScene)
                continue;

            if (!_pigeonPaths.Paths[i].EditPoints)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths[i].Barycenter), Quaternion.identity);
                Vector3 translation = worldToLocalMatrix.MultiplyPoint(vector) - _pigeonPaths.Paths[i].Barycenter;
                if (EditorGUI.EndChangeCheck())
                {
                    for (int j = 0; j < _pigeonPaths.Paths[i].Points.Length; j++)
                    {
                        Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                        _pigeonPaths.Paths[i].Points[j] +=  translation;
                        EditorUtility.SetDirty(_pigeonPaths);
                    }
                }
            }
            else
            {
                for (int j = 0; j < _pigeonPaths.Paths[i].Points.Length; j++)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths[i].Points[j]), Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                        _pigeonPaths.Paths[i].Points[j] = worldToLocalMatrix.MultiplyPoint(vector);
                        EditorUtility.SetDirty(_pigeonPaths);
                    }
                }
            }
            
        }
    }
}
