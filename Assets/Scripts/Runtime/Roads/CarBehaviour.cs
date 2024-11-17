using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    [SerializeField] private float _angleLerp;
    [SerializeField] private float _timeToTravel;
    private float _currentTravelTime;
    [SerializeField] private SFX _carSFX;

    private Road _road;

    public void Init(Road road)
    {
        _road = road;
        _carSFX.PlaySfx();
        Debug.Log("vroum");
        Vector3 targetDirection = Vector3.zero;
        transform.position = _road.GetPosition(0, ref targetDirection);
        transform.localRotation = Quaternion.LookRotation(targetDirection);
    }

    private void Update()
    {
        _currentTravelTime += Time.deltaTime;
        float time = Mathf.Clamp01(_currentTravelTime / _timeToTravel);
        Vector3 targetDirection = Vector3.zero;
        Vector3 targetPosition = _road.GetPosition(time, ref targetDirection);
        transform.position = targetPosition;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(targetDirection), _angleLerp);
        if (_currentTravelTime >= _timeToTravel)
        {
            _carSFX.StopSfx();
            Destroy(gameObject, 2f);
        }
    }
}
