using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    [SerializeField] private SFX _carSFX;
    private float _angleLerp;
    private float _timeToTravel;
    
    private float _currentTravelTime;
    private bool _isLooping;
    private bool _endCycle;

    private Road _road;

    public void Init(Road road, float timeToLoop, float angle, bool isLooping = false, bool useSfx = false)
    {
        _road = road;
        _isLooping = isLooping;
        _timeToTravel = timeToLoop;
        _angleLerp = angle;
        
        Debug.Log("vroum");
        Vector3 targetDirection = Vector3.zero;
        transform.position = _road.GetPosition(0, ref targetDirection);
        transform.localRotation = Quaternion.LookRotation(targetDirection);
        
        if (useSfx)
        {
            _carSFX.PlaySfx();
        }
    }

    private void Update()
    {
        if (_endCycle)
        {
            Destroy(gameObject, 1f);
        }
        _currentTravelTime += Time.deltaTime;
        float time = Mathf.Clamp01(_currentTravelTime / _timeToTravel);
        Vector3 targetDirection = Vector3.zero;
        Vector3 targetPosition = _road.GetPosition(time, ref targetDirection);
        transform.position = targetPosition;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(targetDirection), _angleLerp);
        if (_currentTravelTime > _timeToTravel)
        {
            if (_isLooping)
            {
                _currentTravelTime = 0;
            }
            else
            {
                _carSFX.StopSfx();
                _endCycle = true;
            }
        }
    }
}
