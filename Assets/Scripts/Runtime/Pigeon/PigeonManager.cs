using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PigeonManager : MonoBehaviour
{
    [System.Serializable]
    public class PigeonSlot
    {
        public PigeonBehaviour currentPigeon;
        public int pigeonPathPathId;
    }
    
    [SerializeField] private GyroControler _gyroControler;
    [SerializeField] private PigeonPaths _pigeonPaths;
    [SerializeField] private PigeonBehaviour[] _pigeonPrefabs;

    [SerializeField] private List<PigeonSlot> _pigeonSlots = new List<PigeonSlot>();
 

    private void Awake()
    {
        _gyroControler.OnShakePerch += PigeonsShaked;
    }

    private void OnDestroy()
    {
        _gyroControler.OnShakePerch -= PigeonsShaked;
    }

    public void PigeonsShaked()
    {
        foreach (PigeonSlot pigeon in _pigeonSlots)
        {
            if(pigeon.currentPigeon == null)
                continue;

            pigeon.currentPigeon.ShakePigeon();
            pigeon.currentPigeon = null;
        }
    }

    public bool TrySpawnPigeon()
    {
        if (_pigeonSlots.TrueForAll(x => x.currentPigeon != null))
            return false;

        PigeonBehaviour pigeon = Instantiate(_pigeonPrefabs[Random.Range(0, _pigeonPrefabs.Length)]);
        List<PigeonSlot> freeSlots = _pigeonSlots.Where(x => x.currentPigeon == null).ToList();
        PigeonSlot slot = freeSlots[Random.Range(0, freeSlots.Count)];
        slot.currentPigeon = pigeon;
        pigeon.Init(_pigeonPaths, slot.pigeonPathPathId);
        return true;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySpawnPigeon();
        }
    }
}
