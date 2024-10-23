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
        [field: SerializeField] public int PigeonPathPathId { get; private set; }
        [field: SerializeField] public JoyconLocalisation Localisation { get; private set; }
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
            pigeon.currentPigeon.ShakePigeon();
            JoyconRumblingManager.Instance.OnRumbleStop?.Invoke(pigeon.Localisation);
        }
    }

    public bool TrySpawnPigeon()
    {
        if (!_pigeonSlots.TrueForAll(x => x.currentPigeon == null))
            return false;

        PigeonBehaviour pigeon = Instantiate(_pigeonPrefabs[Random.Range(0, _pigeonPrefabs.Length)]);
        List<PigeonSlot> freeSlots = _pigeonSlots.Where(x => x.currentPigeon == null).ToList();
        PigeonSlot slot = freeSlots[Random.Range(0, freeSlots.Count)];
        pigeon.Init(_pigeonPaths, slot.PigeonPathPathId);
        return true;
    }

    private void RumblingSender(PigeonBehaviour pigeon)
    {
        JoyconLocalisation loca = _pigeonSlots.Find(x => x.currentPigeon == pigeon).Localisation;
        JoyconRumblingManager.Instance.OnRumbleReceived?.Invoke(new Rumbling(pigeon.RumblingData, loca));
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySpawnPigeon();
        }
    }
}
