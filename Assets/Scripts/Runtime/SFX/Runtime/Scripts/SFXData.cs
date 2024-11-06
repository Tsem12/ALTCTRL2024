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

        public string Name => _name;
        public AudioClip[] Clips => _clips;
        public int SizeMax => _sizeMax;
        
        public bool IsLooping => _isLooping;
        public SFXOverflowOperation OverflowOperation => _overflowOperation;
    }
}