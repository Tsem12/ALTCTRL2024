using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PigeonBehaviour : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationCurve _mouvementCurve;
    [SerializeField] private float _timeToLand;
    [SerializeField] private float _timeToLookAtCam;

    
    private PigeonPaths _pigeonPaths;
    private float _currentLandingTime;
    private Curve _curve;
    private PigeonPaths.Path _path;
    

    public void Init(PigeonPaths pigeonPaths)
    {
        _pigeonPaths = pigeonPaths;
        _path = _pigeonPaths.Paths[Random.Range(0, _pigeonPaths.Paths.Length)];
        _curve = _path.Curves[Random.Range(0, _path.Curves.Length)];
        transform.position = _curve.GetPosition(0f, _pigeonPaths.transform.localToWorldMatrix);
    }

    private void Update()
    {
        if(_currentLandingTime >= _timeToLand)
            return;
        
        _currentLandingTime += Time.deltaTime;
        Vector3 targetPosition = _curve.GetPosition(_mouvementCurve.Evaluate(_currentLandingTime / _timeToLand), _pigeonPaths.transform.localToWorldMatrix);
        Vector3 targetPositionDirection = _curve.GetPosition(_mouvementCurve.Evaluate(_currentLandingTime + Time.deltaTime / _timeToLand), _pigeonPaths.transform.localToWorldMatrix);
        
        transform.position = targetPosition;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPositionDirection - targetPosition), 10f);

        if (_currentLandingTime >= _timeToLand)
        {
            StartCoroutine(LookCamRoutine());
            _animator.SetTrigger("TriggerLand");
        }
    }

    IEnumerator LookCamRoutine()
    {
        float currentTime = 0;
        Vector3 origin = transform.position;
        while (currentTime < _timeToLookAtCam)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Camera.main.transform.position, -_path.LandingPoint.right), currentTime / _timeToLookAtCam);
            transform.position = Vector3.Lerp(origin,_path.LandingPoint.position , currentTime / _timeToLookAtCam);
            currentTime += Time.deltaTime;
            yield return null;
        }

        transform.parent = _path.LandingPoint;
        
        //TEMP
        yield return new WaitForSeconds(2);
        Destroy(this);
    }
}
