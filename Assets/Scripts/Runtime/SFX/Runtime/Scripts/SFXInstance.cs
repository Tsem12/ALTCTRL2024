using UnityEngine;

namespace IIMEngine.SFX
{
    public class SFXInstance
    {
        public string SFXName { get; set; } = "";
        public AudioSource AudioSource { get; set; }
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }

        public bool DestroyWhenComplete { get; set; } = false;
    }
}