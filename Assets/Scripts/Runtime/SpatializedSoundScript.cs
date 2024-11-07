using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Transform[] _windOrigins;

    [SerializeField] private SFX _windSFX;

    private AudioSource currentAudioSource;

    public static SpatializedSoundScript Instance { get; private set; }

    public Transform[] WindOrigins => _windOrigins;


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

    public void PlayAudioClipAtDirection(Direction direction)
    {
        _windSFX.SetSource(WindOrigins[(int)direction]);
        _windSFX.PlaySfx();
    }

    public void StopCurrentAudioSource()
    {
        if (currentAudioSource != null)
        {
            currentAudioSource.Stop();
        }
    }
}
