using UnityEngine;

public class PigeonManager : MonoBehaviour
{
    [SerializeField] private PigeonPaths _pigeonPaths;
    [SerializeField] private PigeonBehaviour[] _pigeonPrefabs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PigeonBehaviour pigeon = Instantiate(_pigeonPrefabs[Random.Range(0, _pigeonPrefabs.Length)]);
            pigeon.Init(_pigeonPaths);
        }
    }
}
