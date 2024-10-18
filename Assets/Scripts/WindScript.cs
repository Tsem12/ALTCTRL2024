using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindDirection
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

public class WindScript : MonoBehaviour
{
    [SerializeField] private GameObject windOrigin;
    [SerializeField] private GameObject player;
    [SerializeField] private AudioClip windSound;

    private void Start()
    {
        windOrigin.SetActive(false);
        PlayWindToDirection(WindDirection.East, 5);
    }

    public void PlayWindToDirection(WindDirection windDirection,float duration)
    {
        windOrigin.transform.position = GetWindOrigin(windDirection);
        Vector3 directionToPlayer = player.transform.position - windOrigin.transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero) 
        {
            windOrigin.transform.localRotation = Quaternion.LookRotation(directionToPlayer);

            windOrigin.transform.localRotation *= Quaternion.Euler(0, -60, 0);
        }

        windOrigin.SetActive(true);
        PlayWindSoundFromDirection(windDirection);

        StartCoroutine(DisableWindAfterDuration(duration));
    }

    IEnumerator DisableWindAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        windOrigin.SetActive(false);
    }

    private void PlayWindSoundFromDirection(WindDirection windDirection)
    {
        switch (windDirection)
        {
            case WindDirection.North:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.North, windSound);
                break;
            case WindDirection.NorthEast:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.NorthEast, windSound);
                break;
            case WindDirection.East:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.East, windSound);
                break;
            case WindDirection.SouthEast:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.SouthEast, windSound);
                break;
            case WindDirection.South:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.South, windSound);
                break;
            case WindDirection.SouthWest:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.SouthWest, windSound);
                break;
            case WindDirection.West:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.West, windSound);
                break;
            case WindDirection.NorthWest:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.NorthWest, windSound);
                break;
            default:
                break;

        }
    }

    private Vector3 GetWindOrigin(WindDirection windDirection)
    {
        switch (windDirection)
        {
            case WindDirection.North:
                return SpatializedSoundScript.Instance.spatializedSoundSource.N.transform.position;
            case WindDirection.NorthEast:
                return SpatializedSoundScript.Instance.spatializedSoundSource.NE.transform.position;
            case WindDirection.East:
                return SpatializedSoundScript.Instance.spatializedSoundSource.E.transform.position;
            case WindDirection.SouthEast:
                return SpatializedSoundScript.Instance.spatializedSoundSource.SE.transform.position;
            case WindDirection.South:
                return SpatializedSoundScript.Instance.spatializedSoundSource.S.transform.position;
            case WindDirection.SouthWest:
                return SpatializedSoundScript.Instance.spatializedSoundSource.SW.transform.position;
            case WindDirection.West:
                return SpatializedSoundScript.Instance.spatializedSoundSource.W.transform.position;
            case WindDirection.NorthWest:
                return SpatializedSoundScript.Instance.spatializedSoundSource.NW.transform.position;
            default:
                return Vector3.zero;
        }
    }
}
