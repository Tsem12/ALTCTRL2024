using System;
using UnityEngine;

namespace IIMEngine.Music
{
    [Serializable]
    public class MusicData
    {
        //Name
        [SerializeField] private string _name = "";

        public string Name => _name;
        
        //Volume
        [SerializeField, Range(0, 1)] private float _volume = 1; 
        
        public float Volume => _volume;
        //Loop
        [SerializeField] private AudioClip _mainClip = null;
        [SerializeField] private bool _isLooping = true;
        
        
        public AudioClip MainClip => _mainClip;
        
        public bool IsLooping => _isLooping;
        
        //Intro
        [SerializeField] private bool _hasIntro = false;
        [SerializeField] private AudioClip _introClip = null;

        public bool HasIntro => _hasIntro;
        public AudioClip IntroClip => _introClip;
        
        //Outro
        [SerializeField] private bool _hasOutro = false;
        [SerializeField] private AudioClip _outroClip = null;

        public bool HasOutro => _hasOutro;
        public AudioClip OutroClip => _outroClip;


    }
}