using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PigeonPaths : MonoBehaviour
{
    [System.Serializable]
    public struct Path
    {
        public string name;
        public bool hideGizmo;
        public Curve[] Curves;
        public Transform LandingPoint;
    }
    
    [SerializeField] private bool _drawGizmoOnSelected;
    [SerializeField, Range(0.01f, 1f)] private float _curveGizmoPrecision = 0.1f;
    [field:SerializeField] public Path[] Paths { get; private set; }
    
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(Paths == null || (_drawGizmoOnSelected && Selection.activeGameObject != gameObject))
            return;
        
        for (int i = 0; i < Paths.Length; i++)
        {
            if(Paths[i].hideGizmo)
                continue;
            
            float hue = (float)i / Paths.Length;
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            for (int j = 0; j < Paths[i].Curves.Length; j++)
            {
                Paths[i].Curves[j].DrawGizmo(color, transform.localToWorldMatrix, Selection.activeGameObject == gameObject, _curveGizmoPrecision);
            }
        }
        #endif
    }
}
