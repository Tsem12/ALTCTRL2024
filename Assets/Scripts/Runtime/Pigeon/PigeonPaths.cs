using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PigeonPaths : MonoBehaviour
{
    
    [SerializeField, Range(0.01f, 1f)] private float _curveGizmoPrecision = 0.1f;
    [field:SerializeField] public Curve[] Paths { get; private set; }



    private void OnDrawGizmos()
    {
        if(Paths == null)
            return;
        
        for (int i = 0; i < Paths.Length; i++)
        {
            float hue = (float)i / Paths.Length;
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            Paths[i].DrawGizmo(color, transform.localToWorldMatrix, Selection.activeGameObject == gameObject, _curveGizmoPrecision);
        }
    }
}
