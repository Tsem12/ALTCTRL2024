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
    NorthWest
}

public class SpatializedSoundScript : MonoBehaviour
{
    [SerializeField] private SpatializedSoundSource spatializedSoundSource;
    [SerializeField] private AudioClip windSound;

    private AudioSource currentAudioSource;


    #region Debug
    private int _debugCount = 0;
    #endregion

    private void Start()
    {
        StartCoroutine(Repeat());
    }


    private void PlayAudioClipAtDirection(Direction direction, AudioClip audioClip)
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
            _ => null
        };
    }



    private void DebuugSounds()
    {
        switch (_debugCount)
        {
            case 0:
                PlayAudioClipAtDirection(Direction.North, windSound);
                break;
            case 1:
                PlayAudioClipAtDirection(Direction.NorthEast, windSound);
                break;
            case 2:
                PlayAudioClipAtDirection(Direction.East, windSound);
                break;
            case 3:
                PlayAudioClipAtDirection(Direction.SouthEast, windSound);
                break;
            case 4:
                PlayAudioClipAtDirection(Direction.South, windSound);
                break;
            case 5:
                PlayAudioClipAtDirection(Direction.SouthWest, windSound);
                break;
            case 6:
                PlayAudioClipAtDirection(Direction.West, windSound);
                break;
            case 7:
                PlayAudioClipAtDirection(Direction.NorthWest, windSound);
                break;
        }
        if(_debugCount == 7)
        {
            _debugCount = 0;
        }
        else
        {
            _debugCount++;
        }
        
    }

    private IEnumerator Repeat()
    {
        while (true) 
        {
            DebuugSounds(); 
            yield return new WaitForSeconds(2.5f); 
        }
    }
}
