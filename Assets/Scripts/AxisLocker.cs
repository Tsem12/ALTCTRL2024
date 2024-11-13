using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLocker : MonoBehaviour
{
    float _initialCoordX;
    float _initialCoordY;

    private void Start()
    {
        _initialCoordX = transform.position.x;
        _initialCoordY = transform.position.y;
    }
    void Update()
    {
        transform.position = new Vector3(_initialCoordX, _initialCoordY, transform.position.z);
    }
}
