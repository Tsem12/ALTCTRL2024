using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PigeonManager : MonoBehaviour
{
    [SerializeField] private GyroControler _gyroControler;
    [SerializeField] private PigeonPaths _pigeonPaths;
    [SerializeField] private PigeonBehaviour[] _pigeonPrefabs;


    private void Awake()
    {
        //_gyroControler.OnShakePerch += 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PigeonBehaviour pigeon = Instantiate(_pigeonPrefabs[Random.Range(0, _pigeonPrefabs.Length)]);
            pigeon.Init(_pigeonPaths);
        }
    }
}
