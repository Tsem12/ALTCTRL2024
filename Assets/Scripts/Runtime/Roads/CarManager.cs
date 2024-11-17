using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] private Road[] _roads;
    [SerializeField] private Transform _player;
    [SerializeField] private float _maxSpawnDistance;
    [SerializeField] private CarBehaviour _carBehaviour;

    private Road PertinantRoads
    {
        get
        {
            Road[] roads =_roads.Where(x =>
                Vector3.Distance(transform.position, x.transform.position) < _maxSpawnDistance &&
                Vector3.Dot(transform.TransformDirection(Vector3.right), Vector3.Normalize(x.transform.position - _player.position)) > 0).ToArray();
            
            if (roads.Length == 0)
                return null;

            return roads[Random.Range(0, roads.Length)];
        }
    }

    public void SpawnCar()
    {
        Road road = PertinantRoads;
        if(road == null)
            return;

        CarBehaviour car = Instantiate(_carBehaviour, road.transform.position, Quaternion.identity, road.transform);
        car.Init(road);
        
    }

    [Button]
    public void TestSpawnCar() => SpawnCar();
}
