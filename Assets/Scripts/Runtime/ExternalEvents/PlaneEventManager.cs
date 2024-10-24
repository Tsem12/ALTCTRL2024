using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaneEventManager : MonoBehaviour
{
    [SerializeField] private PlaneEventBehaviour _planePrefab;
    [SerializeField] private PlaneEventPaths _paths;

    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;

    public UnityEvent OnPlaneSpawed; 
    private void Start()
    {
        StartCoroutine(SpawnPlaneEventRoutine());
    }

    public void SpawnPlane()
    {
        OnPlaneSpawed?.Invoke();
        PlaneEventBehaviour plane = Instantiate(_planePrefab, transform);
        plane.Init(_paths);
    }

    private
    
    private IEnumerator SpawnPlaneEventRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
            SpawnPlane();
            yield return null;
        }
    }
}
