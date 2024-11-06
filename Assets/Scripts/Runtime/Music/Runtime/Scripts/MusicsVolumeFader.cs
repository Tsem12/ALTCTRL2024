using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace IIMEngine.Music
{
    public class MusicsVolumeFader : MonoBehaviour
    {
        #region DO NOT MODIFY
        #pragma warning disable 0414

        private const string AUDIOMIXER_PARAM_MUSICVOLUME = "MusicVolume";

        [SerializeField] private AudioMixer _audioMixer;
        
        [SerializeField] private float _minVolume = -80f;

        private float _startVolume = 0f;

        public bool IsFadingIn { get; private set; }
        public bool IsFadingOut { get; private set; }

        #pragma warning restore 0414

        private Coroutine _fadeInRoutine;
        private Coroutine _fadeOutRoutine;
        
        #endregion

        private void Awake()
        {
            MusicsGlobals.VolumeFader = this;
            ResetStartVolumeFromAudioMixer();
        }

        public void ResetStartVolumeFromAudioMixer()
        {
            _audioMixer.GetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, out _startVolume);
            //TODO : Set _startVolume From AudioMixer
        }

        public void ResetToStartVolume()
        {
            InterupFade();
            _audioMixer.SetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, _startVolume);

            //TODO: Interrupt FadeIn or FadeOut if running
            //TODO: Reset AudioMixer MusicVolume to _startVolume
        }

        public void FadeIn(float duration)
        {
            InterupFade();
            _fadeInRoutine = StartCoroutine(FadeInRoutine(duration));

            //TODO: Interrupt FadeIn or FadeOut if running
            //TODO: Lerp Value between Current Volume from AudioMixer to _startVolume
            //You can use coroutines if you want
            //Don't Forget to set IsFadingIn while transition is running
        }


        public void FadeOut(float duration)
        {
            InterupFade();
            _fadeOutRoutine = StartCoroutine(FadeOutRoutine(duration));
            
            //TODO: Interrupt FadeIn or FadeOut if running
            //TODO: Lerp Value between Current Volume from AudioMixer to _minVolume
            //You can use coroutines if you want
            //Don't Forget to set IsFadingIn while transition is running
        }
        
        private void InterupFade()
        {
            if (IsFadingIn)
            {
                StopCoroutine(_fadeInRoutine);
                IsFadingIn = false;
                _fadeInRoutine = null;
            }

            if (IsFadingOut)
            {
                StopCoroutine(_fadeOutRoutine);
                IsFadingOut = false;
                _fadeOutRoutine = null;
            }
        }

        private IEnumerator FadeInRoutine(float duration)
        {
            IsFadingIn = true;
            float timer = 0;
            float startVolume;
            _audioMixer.GetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, out startVolume);
            while (timer < duration)
            {
                _audioMixer.SetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, Mathf.Lerp(startVolume, _startVolume, timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
            IsFadingIn = false;
            _fadeInRoutine = null;
        }
        
        private IEnumerator FadeOutRoutine(float duration)
        {
            IsFadingOut = true;
            float timer = 0;
            float startVolume;
            _audioMixer.GetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, out startVolume);
            while (timer < duration)
            {
                _audioMixer.SetFloat(AUDIOMIXER_PARAM_MUSICVOLUME, Mathf.Lerp(startVolume, _minVolume, timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
            IsFadingOut= false;
            _fadeOutRoutine = null;
        }
    }
}