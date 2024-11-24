using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarManager : MonoBehaviour
{
    [SerializeField] private Road[] _roads;
    [SerializeField] private Road[] _playerRoads;
    [SerializeField] private Road _menuRoad;
    [SerializeField] private Transform _player;
    [SerializeField] private float _maxSpawnDistance;
    [SerializeField] private CarBehaviour _carBehaviour;
    
    [Header("SpawnParameters")]
    [SerializeField] private Vector2 _citySpawnRate;
    [SerializeField] private Vector2 _playerSpawnRate;
    private float _currentPlayerTimeToSpawn;
    private float _currentCityTimeToSpawn;
    private float _currentCitySpawnTime;
    private float _currentPlayerSpawnTime;

    private int _cityCarCallCount = 0;
    private int _playerCarCallCount = 0;
    private Road ExternalRoads
    {
        get
        {
            Road[] roads =_roads.Where(x =>
                x.LastCall < _cityCarCallCount + 3 ||
                (Vector3.Distance(transform.position, x.transform.position) < _maxSpawnDistance &&
                Vector3.Dot(transform.TransformDirection(Vector3.right), Vector3.Normalize(x.transform.position - _player.position)) > 0)).ToArray();
            
            if (roads.Length == 0)
                return null;
            Road selectedRoad = roads[Random.Range(0, roads.Length)];
            selectedRoad.LastCall = ++_cityCarCallCount;
            return selectedRoad;
        }
    }
    
    private Road PlayerRoads
    {
        get
        {
            Road[] roads = _playerRoads.Where(x => x.LastCall < _playerCarCallCount + 3).ToArray();
            if (roads.Length == 0)
                return null;
            Road selectedRoad = roads[Random.Range(0, roads.Length)];
            selectedRoad.LastCall = ++_playerCarCallCount;
            return selectedRoad;
        }
    }

    private void Start()
    {
        CarBehaviour car = Instantiate(_carBehaviour, _menuRoad.transform.position, Quaternion.identity, _menuRoad.transform);
        car.Init(_menuRoad, _menuRoad.TimeToComplete, _menuRoad.AngleLerp,true, true);
        _currentCityTimeToSpawn = Random.Range(_citySpawnRate.x, _citySpawnRate.y);
        _currentPlayerTimeToSpawn = Random.Range(_playerSpawnRate.x, _playerSpawnRate.y);
    }

    private void Update()
    {
        if(!MenuStart.Instance.HasGameStarted)
            return;
        
        _currentCitySpawnTime += Time.deltaTime;
        _currentPlayerSpawnTime += Time.deltaTime;

        if (_currentCitySpawnTime >= _currentCityTimeToSpawn)
        {
            SpawnCarInCity();
            _currentCityTimeToSpawn = Random.Range(_citySpawnRate.x, _citySpawnRate.y);
            _currentCitySpawnTime = 0f;
        }
        
        if (_currentPlayerSpawnTime >= _currentPlayerTimeToSpawn)
        {
            SpawnCarNearToPlayer();
            _currentPlayerTimeToSpawn = Random.Range(_playerSpawnRate.x, _playerSpawnRate.y);
            _currentPlayerSpawnTime = 0f;
        }
    }

    public void SpawnCarInCity()
    {
        Road road = ExternalRoads;
        if(road == null)
            return;
        CarBehaviour car = Instantiate(_carBehaviour, road.transform.position, Quaternion.identity, road.transform);
        car.Init(road, road.TimeToComplete, road.AngleLerp);
    }
    
    public void SpawnCarNearToPlayer()
    {
        Road road = PlayerRoads;
        if(road == null)
            return;
        CarBehaviour car = Instantiate(_carBehaviour, road.transform.position, Quaternion.identity, road.transform);
        car.Init(road, road.TimeToComplete, road.AngleLerp, false, true);
    }

    [Button]
    public void TestSpawnCarCity() => SpawnCarInCity();
    [Button]
    public void TestSpawnCarPlayer() => SpawnCarNearToPlayer();
}
