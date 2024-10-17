using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct SpatializedSoundSource
{
    public AudioSource N;
    public AudioSource NE;
    public AudioSource E;
    public AudioSource SE;
    public AudioSource S;
    public AudioSource SW;
    public AudioSource W;
    public AudioSource NW;
    public AudioSource TOP;
    public AudioSource DOWN;


}
public enum Direction
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
    Top,
    Down

}

public class SpatializedSoundScript : MonoBehaviour
{
    [SerializeField] private SpatializedSoundSource spatializedSoundSource;

    private AudioSource currentAudioSource;

    public static SpatializedSoundScript Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  
        }

        DontDestroyOnLoad(gameObject); 
    }

    public void PlayAudioClipAtDirection(Direction direction, AudioClip audioClip)
    {
        AudioSource audioSource = GetAudioSourceByDirection(direction);

        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            currentAudioSource = audioSource;
        }
        else
        {
            Debug.LogWarning("AudioSource not found for the specified direction.");
        }
    }

    private AudioSource GetAudioSourceByDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => spatializedSoundSource.N,
            Direction.NorthEast => spatializedSoundSource.NE,
            Direction.East => spatializedSoundSource.E,
            Direction.SouthEast => spatializedSoundSource.SE,
            Direction.South => spatializedSoundSource.S,
            Direction.SouthWest => spatializedSoundSource.SW,
            Direction.West => spatializedSoundSource.W,
            Direction.NorthWest => spatializedSoundSource.NW,
            Direction.Top => spatializedSoundSource.TOP,
            Direction.Down => spatializedSoundSource.DOWN,
            _ => null
        };
    }

}
