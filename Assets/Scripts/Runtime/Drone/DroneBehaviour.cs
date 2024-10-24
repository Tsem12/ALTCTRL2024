using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DroneBehaviour : MonoBehaviour
{
    [SerializeField] private float _timeToTravelCurve;
    [SerializeField] private float _droneSpeedOutOfCurve = 10f;
    [SerializeField] private AnimationCurve _mouvementCurve;

    private float _currentTimeOnCurve;
    
    private DronePaths _dronePath;
    private DronePaths.Path _path;
    private Curve _curve;

    private Coroutine _killRoutine;

    private CameraController _cameraController;

    private bool hasDroneEventStarted;

    public float DistanceToPlayer => Vector3.Distance(transform.position, Camera.main.transform.position);
    public void Init(DronePaths dronePaths)
    {
        _dronePath = dronePaths;
        _path = dronePaths.Paths;
        _curve = _path.Curves[Random.Range(0, _path.Curves.Length)];
        transform.position = _curve.GetPosition(0f, _dronePath.transform.localToWorldMatrix);
    }

    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if (ToolBox.Approximately(DistanceToPlayer, 0f, 1f) && !hasDroneEventStarted)
        {
            Debug.Log("apagnan");
            hasDroneEventStarted = true;
            CameraController.OnDroneEvent?.Invoke();
        }
        if (_currentTimeOnCurve < _timeToTravelCurve)
        {
            _currentTimeOnCurve += Time.deltaTime;
            Vector3 targetPosition = _curve.GetPosition(_mouvementCurve.Evaluate(_currentTimeOnCurve / _timeToTravelCurve), _dronePath.transform.localToWorldMatrix);
            Vector3 targetPositionDirection = _curve.GetPosition(_mouvementCurve.Evaluate((_currentTimeOnCurve + Time.deltaTime) / _timeToTravelCurve), _dronePath.transform.localToWorldMatrix);
            
            transform.position = targetPosition;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPositionDirection - targetPosition), 10f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-_dronePath.transform.right), Time.deltaTime);
            transform.position += -_dronePath.transform.right * (Time.deltaTime * _droneSpeedOutOfCurve);
        }

        if (transform.position.x < Camera.main.transform.position.z && _killRoutine == null)
        {
            _killRoutine = StartCoroutine(KillRoutine());
        }
    }

    IEnumerator KillRoutine()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
}
