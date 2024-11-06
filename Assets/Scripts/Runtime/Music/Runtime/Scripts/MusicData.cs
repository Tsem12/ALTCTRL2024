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