using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEventBehaviour : MonoBehaviour
{
    [SerializeField] private float _timeToTravelCurve;
    [SerializeField] private AnimationCurve _mouvementCurve; 
    
    private PlaneEventPaths _planePath;
    private PlaneEventPaths.Path _path;
    private Curve _curve;

    private Coroutine _killRoutine;
    private float _currentTimeOnCurve;

    public void Init(PlaneEventPaths dronePaths)
    {
        _planePath = dronePaths;
        _path = dronePaths.Paths;
        _curve = _path.Curves[Random.Range(0, _path.Curves.Length)];
        transform.position = _curve.GetPosition(0f, _planePath.transform.localToWorldMatrix);
    }
    
    private void Update()
    {
        if (_currentTimeOnCurve < _timeToTravelCurve)
        {
            _currentTimeOnCurve += Time.deltaTime;
            Vector3 targetPosition = _curve.GetPosition(_mouvementCurve.Evaluate(_currentTimeOnCurve / _timeToTravelCurve), _planePath.transform.localToWorldMatrix);
            Vector3 targetPositionDirection = _curve.GetPosition(_mouvementCurve.Evaluate(_currentTimeOnCurve + Time.deltaTime / _timeToTravelCurve), _planePath.transform.localToWorldMatrix);
            
            transform.position = targetPosition;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPositionDirection - targetPosition), 10f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator KillRoutine()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
