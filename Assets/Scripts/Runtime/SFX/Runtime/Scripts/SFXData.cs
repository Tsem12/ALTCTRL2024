using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace IIMEngine.SFX
{
    [Serializable]
    public class SFXData
    {
        [SerializeField] private string _name = "";
        [SerializeField] private AudioClip[] _clips = null;
        [SerializeField] private int _sizeMax = 1;
        [SerializeField] private bool _isLooping = false;
        [SerializeField] private SFXOverflowOperation _overflowOperation = SFXOverflowOperation.ReuseOldest;
        
        [Header("Audio SOurce Parameters")]
        [SerializeField, Range(0, 256)] private int _priority = 128; 
        [SerializeField, Range(0, 1)] private float _volume = 1; 
        [SerializeField, Range(0, 1)] private float _pitch = 1; 
        [SerializeField, Range(-1, 1)] private float _stereoPan = 0; 
        [SerializeField, Range(0, 1)] private float _spatialBlend = 0; 
        [SerializeField, Range(0, 1.1f)] private float _reverbZoneMin = 1; 
        [SerializeField, Range(0, 5)] private float _dopplerLevel = 1; 
        [SerializeField, Range(0, 360)] private int _spreadLevel = 0; 
        [SerializeField] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
        [SerializeField] private float _minDistance = 1;
        [SerializeField] private float _maxDistance = 500;

        public string Name => _name;
        public AudioClip[] Clips => _clips;
        public int SizeMax => _sizeMax;
        
        public bool IsLooping => _isLooping;
        public SFXOverflowOperation OverflowOperation => _overflowOperation;

       

        public int Priority => _priority;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public float StereoPan => _stereoPan;
        public float SpatialBlend => _spatialBlend;
        public float ReverbZoneMin => _reverbZoneMin;
        public float DopplerLevel => _dopplerLevel;
        public int SpreadLevel => _spreadLevel;
        public AudioRolloffMode RolloffMode => _rolloffMode;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;
    }
}