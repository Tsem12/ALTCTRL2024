using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEventManager : MonoBehaviour
{
    [SerializeField] private PlaneEventBehaviour _planePrefab;
    [SerializeField] private PlaneEventPaths _paths;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SpawnPlane();
        }
    }

    public void SpawnPlane()
    {
        PlaneEventBehaviour plane = Instantiate(_planePrefab, transform);
        plane.Init(_paths);
    }
}
