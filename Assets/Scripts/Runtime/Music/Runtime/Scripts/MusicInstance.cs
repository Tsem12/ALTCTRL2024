using UnityEngine;

namespace IIMEngine.Music
{
    public class MusicInstance
    {
        public string Name { get; set; } = "";
        public AudioSource AudioSource { get; set; }
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }

        public enum State
        {
            Intro = 0,
            Loop,
            Outro,
        }

        public State CurrentState { get; set; } = State.Intro;
    }
}