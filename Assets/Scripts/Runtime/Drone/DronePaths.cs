using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DronePaths : MonoBehaviour
{
    [System.Serializable]
    public struct Path
    {
        public string name;
        public bool hideGizmo;
        public Curve[] Curves;
    }
    
    [SerializeField] private bool _drawGizmoOnSelected;
    [SerializeField, Range(0.01f, 1f)] private float _curveGizmoPrecision = 0.1f;
    [field:SerializeField] public Path Paths { get; private set; }
    
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(Paths.Curves == null || (_drawGizmoOnSelected && Selection.activeGameObject != gameObject))
            return;
        
        if(Paths.hideGizmo)
            return;
            
        for (int i = 0; i < Paths.Curves.Length; i++)
        {
            float hue = (float)i / Paths.Curves.Length;
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            Paths.Curves[i].DrawGizmo(color, transform.localToWorldMatrix, Selection.activeGameObject == gameObject, _curveGizmoPrecision);
        }
        #endif
    }
}
