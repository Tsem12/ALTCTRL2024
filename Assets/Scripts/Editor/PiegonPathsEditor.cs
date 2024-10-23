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
            for (int j = 0; j < _pigeonPaths.Paths[i].Curves.Length; j++)
            {
                if(!_pigeonPaths.Paths[i].Curves[j].EnableEditPositionsOnScene)
                    continue;

                if (!_pigeonPaths.Paths[i].Curves[j].EditPoints)
                {
                    
                    EditorGUI.BeginChangeCheck();
                    Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths[i].Curves[j].Barycenter), Quaternion.identity);
                    Vector3 translation = worldToLocalMatrix.MultiplyPoint(vector) - _pigeonPaths.Paths[i].Curves[j].Barycenter;
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int k = 0; k < _pigeonPaths.Paths[i].Curves[j].Points.Length; k++)
                        {
                            Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                            _pigeonPaths.Paths[i].Curves[j].Points[k] +=  translation;
                            EditorUtility.SetDirty(_pigeonPaths);
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < _pigeonPaths.Paths[i].Curves[j].Points.Length; k++)
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_pigeonPaths.Paths[i].Curves[j].Points[k]), Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(_pigeonPaths, "Change FreeFollowView");
                            _pigeonPaths.Paths[i].Curves[j].Points[k] = worldToLocalMatrix.MultiplyPoint(vector);
                            EditorUtility.SetDirty(_pigeonPaths);
                        }
                    }
                }
            }
            
        }
    }
}
