using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IIMEngine.SFX
{
    public class SFXsManager : MonoBehaviour
    {
        #region DO NOT MODIFY
        #pragma warning disable 0414

        public static SFXsManager Instance { get; private set; }

        [Header("Bank")]
        [SerializeField] private SFXsBank _bank;

        [Header("Audio Source")]
        [SerializeField] private AudioSource _audioSourceTemplate = null;

        private Dictionary<string, List<SFXInstance>> _poolInstancesDict = new Dictionary<string, List<SFXInstance>>();
        private Dictionary<string, List<SFXInstance>> _playingInstancesDict = new Dictionary<string, List<SFXInstance>>();
        private Dictionary<string, SFXData> _datasDict = new Dictionary<string, SFXData>();
            
        #pragma warning restore 0414
        #endregion

        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void Update()
        {
            _CleanupNonPlayingInstances();
        }

        public void Init()
        {
            _InitDatasDict();
            _InitPoolDict();
            _InitPlayingInstancesDict();
            _LoadAllAudiosData();
        }

        private void _CleanupNonPlayingInstances()
        {
            foreach (KeyValuePair<string, List<SFXInstance>> instance in _playingInstancesDict)
            {
                for(int i = 0; i < instance.Value.Count; i++)
                {
                    if (!instance.Value[i].AudioSource.isPlaying)
                    {
                        if (instance.Value[i].DestroyWhenComplete)
                        {
                            Destroy(instance.Value[i].AudioSource);
                        }
                        else
                        {
                            
                            instance.Value[i].AudioSource.transform.parent = _audioSourceTemplate.transform.parent;
                            instance.Value[i].AudioSource.priority = _audioSourceTemplate.priority;
                            instance.Value[i].AudioSource.volume = _audioSourceTemplate.volume;
                            instance.Value[i].AudioSource.pitch = _audioSourceTemplate.pitch;
                            instance.Value[i].AudioSource.panStereo = _audioSourceTemplate.panStereo;
                            instance.Value[i].AudioSource.spatialBlend = _audioSourceTemplate.spatialBlend;
                            instance.Value[i].AudioSource.reverbZoneMix = _audioSourceTemplate.reverbZoneMix;
                            instance.Value[i].AudioSource.dopplerLevel = _audioSourceTemplate.dopplerLevel;
                            instance.Value[i].AudioSource.spread = _audioSourceTemplate.spread;
                            instance.Value[i].AudioSource.rolloffMode = _audioSourceTemplate.rolloffMode;
                            instance.Value[i].AudioSource.minDistance = _audioSourceTemplate.minDistance;
                            instance.Value[i].AudioSource.maxDistance = _audioSourceTemplate.maxDistance;
                            instance.Value[i].AudioSource.Stop();
                            _poolInstancesDict[instance.Key].Add(instance.Value[i]);
                        }
                        instance.Value.Remove(instance.Value[i]);
                    }
                }
            }
            
            //Loop over all playing instance
                //If SFXInstance audiosource is playing
                    //Destroy Instance if DestroyWhenComplete is true
                    //Reset Instance and move it to pool if DestroyWhenComplete is false
        }

        private void _InitDatasDict()
        {
            foreach (var sfx in _bank.SFXDatasList)
            {
                _datasDict[sfx.Name] = sfx;
            }
            //Loop over all SFXsData inside bank and fill _datasDict dictionary
        }

        private void _InitPoolDict()
        {
            foreach (SFXData sfx in _bank.SFXDatasList)
            {
                List<SFXInstance> instances = new List<SFXInstance>();
                for (int i = 0; i < sfx.SizeMax; i++)
                {
                    AudioSource audioSource = Instantiate(_audioSourceTemplate, _audioSourceTemplate.transform.position, Quaternion.identity, _audioSourceTemplate.transform.parent);
                    SFXInstance instance = new SFXInstance();
                    instance.SFXName = sfx.Name;
                    audioSource.gameObject.name = sfx.Name;
                    audioSource.loop = sfx.IsLooping;
                    audioSource.clip = sfx.Clips[Random.Range(0, sfx.Clips.Length)];
                    instance.AudioSource = audioSource;
                    instance.Transform = audioSource.transform;
                    instance.GameObject = audioSource.gameObject;
                    instance.AudioSource.priority = sfx.Priority;
                    instance.AudioSource.volume = sfx.Volume;
                    instance.AudioSource.pitch = sfx.Pitch;
                    instance.AudioSource.panStereo = sfx.StereoPan;
                    instance.AudioSource.spatialBlend = sfx.SpatialBlend;
                    instance.AudioSource.reverbZoneMix = sfx.ReverbZoneMin;
                    instance.AudioSource.dopplerLevel = sfx.DopplerLevel;
                    instance.AudioSource.spread = sfx.SpreadLevel;
                    instance.AudioSource.rolloffMode = sfx.RolloffMode;
                    instance.AudioSource.minDistance = sfx.MinDistance;
                    instance.AudioSource.maxDistance = sfx.MaxDistance;
                    instances.Add(instance);
                }
                _poolInstancesDict[sfx.Name] = instances;
            }
            //Loop over all SFXsData inside bank
            //Create multiple SFXsInstance using SizeMax property inside SFXData
            //And store it into _poolInstancesDict
        }
        
        private void _InitPlayingInstancesDict()
        {
            foreach (SFXData sfx in _bank.SFXDatasList)
            {
                _playingInstancesDict[sfx.Name] = new List<SFXInstance>();
            }
            //Loop over all SFXsData inside bank
            //Init PlayingInstances Dictionary using SizeMax property inside SFXData
        }

        public SFXInstance PlaySound(string name,Transform source = null)
        {
            SFXInstance sfxInstance = _PikUpInstanceFromPool(name, source);
            if (sfxInstance == null) return null;
            if (source != null)
            {
                sfxInstance.Transform.parent = source;
            }
            sfxInstance.Transform.localPosition = Vector2.zero;
            //Forcing SetActive for a gameobject containing an AudioSource replay the sound inside
            // sfxInstance.GameObject.SetActive(false);
            // sfxInstance.GameObject.SetActive(true);
            sfxInstance.AudioSource.PlayOneShot(sfxInstance.AudioSource.clip);
            return sfxInstance;
        }
        private SFXInstance _PikUpInstanceFromPool(string name, Transform source = null)
        {
            if (!_poolInstancesDict.ContainsKey(name)) return null;

            SFXInstance sfx = null;
            bool areInstanceAvailable = _poolInstancesDict[name].Count > 0;

            if (areInstanceAvailable)
            {
                sfx = _poolInstancesDict[name][0];
                _poolInstancesDict[name].Remove(sfx);
                _playingInstancesDict[name].Add(sfx);
            }
            else
            {
                switch (_datasDict[name].OverflowOperation)
                {
                    case SFXOverflowOperation.Cancel:
                        return null;
                    
                    case SFXOverflowOperation.ReuseOldest:
                        return _playingInstancesDict[name][0];
                    
                    case SFXOverflowOperation.CreateAndDestroy:
                        SFXData sfxTemplate = _datasDict[name];
                        AudioSource audioSource = Instantiate(_audioSourceTemplate, _audioSourceTemplate.transform.position, Quaternion.identity, source == null ? _audioSourceTemplate.transform.parent : source);
                        SFXInstance instance = new SFXInstance();
                        instance.SFXName = sfxTemplate.Name;
                        audioSource.gameObject.name = sfxTemplate.Name;
                        audioSource.loop = sfxTemplate.IsLooping;
                        audioSource.clip = sfxTemplate.Clips[Random.Range(0, sfxTemplate.Clips.Length)];
                        instance.AudioSource = audioSource;
                        instance.Transform = audioSource.transform;
                        instance.GameObject = audioSource.gameObject;
                        instance.DestroyWhenComplete = true;
                        _playingInstancesDict[name].Add(instance);
                        return instance;
                }
            }
            
            return sfx;
            //Try to find an SFXInstance inside Pool Dictionary
            
            //If an Instance is available
                //Remove sfx instance from Pool Dictionary
                //Add sfx instance from PlayingSFX Dictionary
                //return sfx instance
            //Else
                //Check Overflow operation
                //If Overflow is cancel
                    //Do nothing, cancel means we do not play sounds if there is no sounds available in the pool
                //If Overflow is ReuseOldest
                    //Find sfx instance from PlayingSFX Dictionary
                //If Overflow is Create And Destroy
                    //Create sfx instance using SFXData
                    //Mark sfx instance as Destroyable (DestroyOnComplete = true)
                //Add Found sfx instance to PlayingSFX Dictionary
                //return Instance

            
        }

        private void _LoadAllAudiosData()
        {
            foreach (var instance in _poolInstancesDict)
            {
                foreach (SFXInstance sfxInstance in instance.Value)
                {
                    sfxInstance.AudioSource.clip.LoadAudioData();
                }
            }
            //AudioClips are not load by default
            //We need to load it using LoadAudioData
            //See : https://docs.unity3d.com/ScriptReference/AudioClip.LoadAudioData.html
        }
    }
}