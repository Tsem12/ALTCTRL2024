using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaneEventPaths)), CanEditMultipleObjects]
public class PlaneEventPathsEditor : Editor
{
    private PlaneEventPaths _planePaths;

    private void OnEnable()
    {
        _planePaths = (PlaneEventPaths)target;
    }

    protected void OnSceneGUI()
    {
        Matrix4x4 localToWorldMatrix = _planePaths.transform.localToWorldMatrix;
        Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
        for (int i = 0; i < _planePaths.Paths.Curves.Length; i++)
        {
            if(!_planePaths.Paths.Curves[i].EnableEditPositionsOnScene)
                continue;

            if (!_planePaths.Paths.Curves[i].EditPoints)
            {
                
                EditorGUI.BeginChangeCheck();
                Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_planePaths.Paths.Curves[i].Barycenter), Quaternion.identity);
                Vector3 translation = worldToLocalMatrix.MultiplyPoint(vector) - _planePaths.Paths.Curves[i].Barycenter;
                if (EditorGUI.EndChangeCheck())
                {
                    for (int k = 0; k < _planePaths.Paths.Curves[i].Points.Length; k++)
                    {
                        Undo.RecordObject(_planePaths, "Change FreeFollowView");
                        _planePaths.Paths.Curves[i].Points[k] +=  translation;
                        EditorUtility.SetDirty(_planePaths);
                    }
                }
            }
            else
            {
                for (int k = 0; k < _planePaths.Paths.Curves[i].Points.Length; k++)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 vector = Handles.PositionHandle(localToWorldMatrix.MultiplyPoint(_planePaths.Paths.Curves[i].Points[k]), Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_planePaths, "Change FreeFollowView");
                        _planePaths.Paths.Curves[i].Points[k] = worldToLocalMatrix.MultiplyPoint(vector);
                        EditorUtility.SetDirty(_planePaths);
                    }
                }
            }
            
            }
    }
}
