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
    [SerializeField] private AudioClip _pigeonSound;
    [SerializeField] private AudioClip _pigeonFlyAwaySound;
    [SerializeField] private List<PigeonSlot> _pigeonSlots = new List<PigeonSlot>();

    public static PigeonManager instance;
    
    public int PigeonAmountOnPerch { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de PigeonManager dans la scene");
            return;
        }
        instance = this;
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
            PigeonAmountOnPerch--;
            pigeon.currentPigeon = null;
            JoyconRumblingManager.Instance.OnRumbleStop?.Invoke(pigeon.Localisation);
        }
        AudioSource.PlayClipAtPoint(_pigeonFlyAwaySound, Camera.main.transform.position);

    }

    public void TrySpawnPigeonEvent()
    {
        bool quoicoubeh = TrySpawnPigeon();
    }

    public bool TrySpawnPigeon()
    {
        if (_pigeonSlots.TrueForAll(x => x.currentPigeon != null))
            return false;

        PigeonBehaviour pigeon = Instantiate(_pigeonPrefabs[Random.Range(0, _pigeonPrefabs.Length)]);
        List<PigeonSlot> freeSlots = _pigeonSlots.Where(x => x.currentPigeon == null).ToList();
        PigeonSlot slot = freeSlots[Random.Range(0, freeSlots.Count)];
        slot.currentPigeon = pigeon;
        pigeon.OnPigeonLanded += RumblingSender;
        pigeon.Init(_pigeonPaths, slot.PigeonPathPathId);
        AudioSource.PlayClipAtPoint(_pigeonSound, Camera.main.transform.position);
        return true;
    }
    
    private void RumblingSender(PigeonBehaviour pigeon)
    {
        pigeon.OnPigeonLanded -= RumblingSender;
        PigeonAmountOnPerch++;
        JoyconLocalisation loca = _pigeonSlots.Find(x => x.currentPigeon == pigeon).Localisation;
        JoyconRumblingManager.Instance.OnRumbleReceived?.Invoke(new Rumbling(pigeon.RumblingData, loca));
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnPigeon();
        }
    }
    
}

