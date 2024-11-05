using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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
    
    [Header("Shake Values")] 
    [SerializeField] private float _minLandingTimeToEnableShake;
    [Header("Pigeon Parameters")] 
    [SerializeField] private PigeonPaths _pigeonPaths;
    [SerializeField] private PigeonBehaviour[] _pigeonPrefabs;
    [SerializeField] private AudioClip _pigeonSound;
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
        GyroControler.OnShakePerch.AddListener(PigeonsShaked);
    }

    private void OnEnable()
    {
        GameManager.OnLoseEvent.AddListener(ClearPigeonAfterDeath);
    }

    private void OnDisable()
    {
        GameManager.OnLoseEvent.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        GyroControler.OnShakePerch.RemoveAllListeners();
    }

    public void PigeonsShaked()
    {
        PigeonSlot[] _pigeons = _pigeonSlots.Where(x => x.currentPigeon != null).ToArray();
        if(_pigeons.Length <= 0)
            return;
        
        float latestPigeonLandingTime = _pigeons.Min(x => x.currentPigeon.LandingTime);
        if(latestPigeonLandingTime + _minLandingTimeToEnableShake > Time.time)
            return;
        
        foreach (PigeonSlot pigeon in _pigeonSlots)
        {
            if(pigeon.currentPigeon == null || !pigeon.currentPigeon.IsLanded)
                continue;

            pigeon.currentPigeon.ShakePigeon();
            PigeonAmountOnPerch--;
            pigeon.currentPigeon = null;
            JoyconRumblingManager.Instance.OnRumbleStop?.Invoke(pigeon.Localisation);
        }
        

    }

    [Button]
    public void ClearPigeonAfterDeath()
    {
        foreach (PigeonSlot pigeon in _pigeonSlots)
        {
            if(pigeon.currentPigeon == null)
                continue;
            
            PigeonAmountOnPerch--;
            Destroy(pigeon.currentPigeon.gameObject);
            pigeon.currentPigeon = null;
            JoyconRumblingManager.Instance.OnRumbleStop?.Invoke(pigeon.Localisation);
        }
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

    [Button]
    public void TestSpawnPigeon() => TrySpawnPigeon();
    
    private void RumblingSender(PigeonBehaviour pigeon)
    {
        pigeon.OnPigeonLanded -= RumblingSender;
        PigeonAmountOnPerch++;
        JoyconLocalisation loca = _pigeonSlots.Find(x => x.currentPigeon == pigeon).Localisation;
        JoyconRumblingManager.Instance.OnRumbleReceived?.Invoke(new Rumbling(pigeon.RumblingData, loca));
    }
    
}

