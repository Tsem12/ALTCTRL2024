using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLocker : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    void Update()
    {
        transform.position = new Vector3(_followTarget.position.x, transform.position.y, transform.position.z);
    }
}
