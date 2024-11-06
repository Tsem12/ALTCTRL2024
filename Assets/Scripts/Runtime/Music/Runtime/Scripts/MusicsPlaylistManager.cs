using System;
using System.Collections.Generic;
using UnityEngine;

namespace IIMEngine.Music
{
    public class MusicsPlaylistManager : MonoBehaviour
    {
        #region DO NOT MODIFY
        
        [SerializeField] private AudioSource _audioSourceTemplate;
        [SerializeField] private MusicsBank _bank;

        private Dictionary<string, MusicInstance> _musicInstancesDict = new Dictionary<string, MusicInstance>();
        private Dictionary<string, MusicData> _musicDataDict = new Dictionary<string, MusicData>();

        private MusicInstance _currentMusicInstance = null;

        public bool IsPaused { get; private set; } = false;
        
        #endregion

        private void Awake()
        {
            MusicsGlobals.PlaylistManager = this;
            
            _InitInstancesDict();
            _InitDatasDict();
            _LoadAllAudiosData();
        }

        private void Update()
        {
            if (IsPaused) return;
            if (_currentMusicInstance == null) return;

            switch (_currentMusicInstance.CurrentState)
            {
                case MusicInstance.State.Intro:
                    if(_currentMusicInstance.AudioSource.isPlaying)
                        return;
                    _currentMusicInstance.AudioSource.clip = _musicDataDict[_currentMusicInstance.Name].MainClip;
                    _currentMusicInstance.CurrentState = MusicInstance.State.Loop;
                    _currentMusicInstance.AudioSource.loop = _musicDataDict[_currentMusicInstance.Name].IsLooping;
                    PlayMusic(_currentMusicInstance.Name, true);
                    return;
                case MusicInstance.State.Loop:
                    if (!_currentMusicInstance.AudioSource.isPlaying && !_currentMusicInstance.AudioSource.loop)
                    {
                        _currentMusicInstance.AudioSource.loop = false;
                        if (_musicDataDict[_currentMusicInstance.Name].HasOutro)
                        {
                            _currentMusicInstance.CurrentState = MusicInstance.State.Outro;
                            _currentMusicInstance.AudioSource.clip = _musicDataDict[_currentMusicInstance.Name].OutroClip;
                            PlayMusic(_currentMusicInstance.Name, true);
                            return;
                        }
                    }
                    return;
                case MusicInstance.State.Outro:
                    if (!_currentMusicInstance.AudioSource.isPlaying)
                    {
                        StopMusic();
                    }
                    return;
            }
            
            //TODO: Update Current Music Instance
            
            //Check Music Instance State
            
            //If State is "Intro"
                //Wait if audio source is playing
                //If not playing
                    //Play Music Loop and set state to Loop
            
            //If State is "Loop"
                //if AudioSource not looping and not playing =>
                //If Music has Outro
                    //Play Music Outro and set state to "Outro"
                    
            //If State is "Outro"
                //if AudioSource not playing
                    //Reset MusicInstance
        }

        private void _InitInstancesDict()
        {
            foreach (MusicData music in _bank.MusicDatas)
            {
                AudioSource instance = Instantiate(_audioSourceTemplate, _audioSourceTemplate.transform.position,
                    Quaternion.identity, _audioSourceTemplate.transform.parent);
                instance.clip = music.IntroClip;
                instance.gameObject.name = music.Name;
                
                _musicInstancesDict[music.Name] = new MusicInstance
                {
                    Name = music.Name,
                    AudioSource = instance,
                    GameObject = instance.gameObject,
                    Transform = instance.transform
                };
            } 
            
            //Loop over all Music Datas inside Bank and Fill Music Instances into _musicInstancesDict
        }

        private void _InitDatasDict()
        {
            foreach (MusicData data in _bank.MusicDatas)
            {
                _musicDataDict[data.Name] = data;
            }
            
            //Loop over all Music Datas inside Bank and Fill Music Data into _musicDataDict
        }

        private void _LoadAllAudiosData()
        {

            foreach (var instance in _musicDataDict)
            {
                instance.Value.MainClip.LoadAudioData();
                if(instance.Value.HasIntro) instance.Value.IntroClip.LoadAudioData();
                if(instance.Value.HasOutro) instance.Value.OutroClip.LoadAudioData();
            }
            
            //AudioClips are not load by default
            //We need to load it using LoadAudioData
            //See : https://docs.unity3d.com/ScriptReference/AudioClip.LoadAudioData.html
            
            //Music Instances have until 3 audioClip
            //MainClip => always set
            //IntroClip => only set when HasIntro is true
            //OutroClip => only set when HasOutro is true
        }

        public void PlayMusic(string name, bool forceReset = false)
        {
            if(!_musicInstancesDict.ContainsKey(name))
                return;

            MusicInstance musicInstanceFound = _musicInstancesDict[name];
            
            if(_currentMusicInstance == musicInstanceFound && !forceReset)
                return;
            
            StopMusic(forceReset);
            _currentMusicInstance = musicInstanceFound;
            _currentMusicInstance.AudioSource.Play();

            //Find Music Instance From _musicInstancesDict
            //(Be careful to check if there is an instance is found)

            //Do not replay MusicInstance if _currentMusicInstance is musicInstanceFound

            //Stop _currentMusicInstance

            //Set _currentMusicInstance with musicInstanceFound

            //Play musicInstanceFound
        }

        public void PauseMusic()
        {
            if(_currentMusicInstance == null)
                return;
            //Do nothing if there is no _currentMusicInstance
            IsPaused = true;
            _currentMusicInstance.AudioSource.Pause();
            //Pause _currentMusicInstance AudioSource
        }

        public void ResumeMusic()
        {
            if(_currentMusicInstance == null)
                return;
            //Do nothing if there is no _currentMusicInstance
            IsPaused = false;
            _currentMusicInstance.AudioSource.UnPause();
            //Play _currentMusicInstance AudioSource
        }

        public void StopMusic(bool forceReset = false)
        {
            //Do nothing if there is no _currentMusicInstance
            if (_currentMusicInstance == null) return;
            _currentMusicInstance.AudioSource.Stop();
            if (!forceReset)
            {
                _currentMusicInstance.CurrentState = MusicInstance.State.Intro;
                _currentMusicInstance.AudioSource.loop = false;
                _currentMusicInstance.AudioSource.clip = _musicDataDict[_currentMusicInstance.Name].IntroClip;
            }
            _currentMusicInstance = null;
            //Stop _currentMusicInstance AudioSource
            //Don't forget to remove current reference to _currentMusicInstance
        }
    }
}