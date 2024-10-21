using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrawer : MonoBehaviour
{
    public bool _enableCameraGizmo;
    
    public void OnDrawGizmos()
    {
        if(!_enableCameraGizmo)
            return;
        
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, .25f);
        Vector3 position = transform.position;
        Gizmos.DrawLine(transform.position, position);
        Gizmos.matrix = Matrix4x4.TRS(position, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, Camera.main.fieldOfView, Camera.main.farClipPlane, Camera.main.nearClipPlane, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
