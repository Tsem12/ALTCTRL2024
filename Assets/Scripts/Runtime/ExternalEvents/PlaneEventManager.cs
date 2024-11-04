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
    [SerializeField] private AudioClip[] _clips;

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

    public void PlaySound()
    {
        AudioSource.PlayClipAtPoint(_clips[Random.Range(0, _clips.Length)], Camera.main.transform.position + Vector3.up * 50);
    }
    
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
