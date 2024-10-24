using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private DroneBehaviour _dronePrefab;
    [SerializeField] private DronePaths _paths;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DroneBehaviour drone = Instantiate(_dronePrefab, transform);
            drone.Init(_paths);
        }
    }
}
