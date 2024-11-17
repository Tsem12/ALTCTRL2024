using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Road : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Vector3[] Points { get; private set;}
    [SerializeField] private float _curveGizmoPrecision;

    private Dictionary<Vector2, (Vector3 pointA, Vector3 pointB)> _distanceDict = new Dictionary<Vector2, (Vector3 pointA, Vector3 pointB)>();
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

    [field: SerializeField] public bool EditPoints { get; set; }


    private void Awake()
    {
        SetDict();
    }

    public void SetDict()
    {
        _distanceDict.Clear();
        float totalDist = 0;
        List<float> dists = new List<float>();
        float previousDist = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            float dist = Vector3.Distance(Points[i], Points[i + 1]);
            totalDist += dist;
            dists.Add(previousDist + dist);
            previousDist = dists[i];
        }

        float previousPercentage = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            _distanceDict[new Vector2(previousPercentage, dists[i] / totalDist) ] = (Points[i], Points[i + 1]);
            previousPercentage = dists[i] / totalDist;
        }
    }

    public Vector3 GetPosition(float time, ref Vector3 direction)
    {
        time = Mathf.Clamp01(time);
        (Vector3 pointA, Vector3 pointB) points = (Vector3.zero, Vector3.zero);
        Vector2 percentages = Vector2.zero;
        foreach (KeyValuePair<Vector2, (Vector3 pointA, Vector3 pointB)> frag in _distanceDict)
        {
            if (time >= frag.Key.x && time < frag.Key.y)
            {
                points = frag.Value;
                percentages = frag.Key;
                break;
            }
        }

        if (points == (Vector3.zero, Vector3.zero))
            return Vector3.zero;

        direction = points.pointB - points.pointA;
        return transform.localToWorldMatrix.MultiplyPoint(Vector3.Lerp(points.pointA, points.pointB, (time - percentages.x) / (percentages.y - percentages.x)));
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(Selection.activeGameObject != gameObject)
            return;
        
        SetDict();
        
        Gizmos.color = Color.red;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(Points[i]), transform.localToWorldMatrix.MultiplyPoint(Points[i+1]));
        }
        
        
        for (int i = 0; i < Points.Length; i++)
        {
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(Points[i]), .1f);
        }
        
        GUIStyle style = new GUIStyle()
        {
            fontSize = 20 
        };
        style.normal.textColor = Color.black;
        Handles.color = Color.red;
        for (int i = 0; i < Points.Length; i++)
        {
            Handles.Label(transform.localToWorldMatrix.MultiplyPoint(Points[i]) - new Vector3(-.25f, -.25f, 0), i.ToString(), style);
        }
        #endif
    }
}

