using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    [SerializeField] private DroneBehaviour _dronePrefab;
    [SerializeField] private DronePaths _paths;

    public void SpawnDrone()
    {
        DroneBehaviour drone = Instantiate(_dronePrefab, transform);
        drone.Init(_paths);
    }

    [Button]
    public void TrySpawnDrone() => SpawnDrone();
}
