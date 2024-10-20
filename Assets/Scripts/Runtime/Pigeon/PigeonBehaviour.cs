using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PigeonBehaviour : MonoBehaviour
{
    [SerializeField] private PigeonPaths _pigeonPaths;
    [SerializeField] private float _timeToLand;
    private float _currentLandingTime;
    private Curve _curve;

    private void Awake()
    {
        _curve = _pigeonPaths.Paths[Random.Range(0, _pigeonPaths.Paths.Length)];
        transform.position = _curve.GetPosition(0f, _pigeonPaths.transform.localToWorldMatrix);
    }

    private void Update()
    {
        _currentLandingTime += Time.deltaTime;
        Vector3 targetPosition = _curve.GetPosition(_currentLandingTime / _timeToLand, _pigeonPaths.transform.localToWorldMatrix);
        Vector3 targetPositionDirection = _curve.GetPosition(_currentLandingTime + Time.deltaTime / _timeToLand, _pigeonPaths.transform.localToWorldMatrix);
        
        transform.position = targetPosition;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPositionDirection - targetPosition), 10f);
    }
}
