using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    [SerializeField] private List<AudioClip> windSoundList;
    [SerializeField] private List<GameObject> windEffectList;
    private GameObject windEffect;

    private AudioClip windSound;
    [SerializeField] private GameObject compassArrow;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float compassRotationDuration = 0.5f;
    [SerializeField] private float compassShakeIntensity=2;

    private float compassRotationAngleDestination;
    private float compassRotationAngleStart;

    private bool isWindBlowing = false;
    private bool isCompassRotating = false;
    private float timeElapsed = 0f;


    private void Awake()
    {
        windOrigin.SetActive(false);
    }

    private void Update()
    {
        if (isCompassRotating)
        {
            Rotate();
            Debug.Log("Rotating compass");
        }
        else
        {
            if (isWindBlowing)
            {
                compassArrow.transform.localRotation = Quaternion.Euler(90, 0, Random.Range(compassRotationAngleDestination - compassShakeIntensity, compassRotationAngleDestination + compassShakeIntensity));
            }
            else
            {
                compassArrow.transform.localRotation = Quaternion.Euler(90, 0, compassRotationAngleDestination);
            }
        }
    }

    private void Start()
    {
        PlayWindToDirection(WindDirection.West, 5);
    }

    AudioClip ChooseRandomAudioClip()
    {
        return windSoundList[Random.Range(0, windSoundList.Count)];
    }

    void ChooseRandomGameObject()
    {
         windEffect = windEffectList[Random.Range(0, windSoundList.Count)];
    }

    public void PlayWindToDirection(WindDirection windDirection, float duration)
    {
        ChooseRandomGameObject();
        windOrigin.transform.position = GetWindOrigin(windDirection);
        Vector3 directionToPlayer = player.transform.position - windOrigin.transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            windOrigin.transform.localRotation = Quaternion.LookRotation(directionToPlayer);

            windOrigin.transform.localRotation *= Quaternion.Euler(0, -60, 0);
        }

        windOrigin.SetActive(true);
        windEffect.SetActive(true);
        PlayWindSoundFromDirection(windDirection);
        RotateCompassWithDirection(windDirection);
        StartCoroutine(DisableWindAfterDuration(duration));
    }

    IEnumerator DisableWindAfterDuration(float duration)
    {
        isWindBlowing = true;
        yield return new WaitForSeconds(duration);
        windOrigin.SetActive(false);
        windEffect.SetActive(false);
        isWindBlowing = false;
    }

    private void PlayWindSoundFromDirection(WindDirection windDirection)
    {
        switch (windDirection)
        {
            case WindDirection.North:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.North, ChooseRandomAudioClip());
                break;
            case WindDirection.NorthEast:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.NorthEast, ChooseRandomAudioClip());
                break;
            case WindDirection.East:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.East, ChooseRandomAudioClip());
                break;
            case WindDirection.SouthEast:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.SouthEast, ChooseRandomAudioClip());
                break;
            case WindDirection.South:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.South, ChooseRandomAudioClip());
                break;
            case WindDirection.SouthWest:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.SouthWest, ChooseRandomAudioClip());
                break;
            case WindDirection.West:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.West, ChooseRandomAudioClip());
                break;
            case WindDirection.NorthWest:
                SpatializedSoundScript.Instance.PlayAudioClipAtDirection(Direction.NorthWest, ChooseRandomAudioClip());
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

    private void RotateCompassWithDirection(WindDirection windDirection)
    {
        switch (windDirection)
        {
            case WindDirection.North:
                RotateCompassActionJuicily(180);
                break;
            case WindDirection.NorthEast:
                RotateCompassActionJuicily(135);
                break;
            case WindDirection.East:
                RotateCompassActionJuicily(90);
                break;
            case WindDirection.SouthEast:
                RotateCompassActionJuicily(45);
                break;
            case WindDirection.South:
                RotateCompassActionJuicily(0);
                break;
            case WindDirection.SouthWest:
                RotateCompassActionJuicily(225);
                break;
            case WindDirection.West:
                RotateCompassActionJuicily(270);
                break;
            case WindDirection.NorthWest:
                RotateCompassActionJuicily(315);
                break;
        }
    }


    private void RotateCompassActionJuicily(float angle)
    {
        isCompassRotating = true;
        compassRotationAngleDestination = angle;
        compassRotationAngleStart = compassArrow.transform.localRotation.eulerAngles.z;
        StartCoroutine(DisableCompassRotationAfterDuration(compassRotationDuration));
    }

    IEnumerator DisableCompassRotationAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration - 0.2f);
        isCompassRotating = false;
    }

    private void Rotate()
    {
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / compassRotationDuration;

        float curvedT = curve.Evaluate(t);

        compassArrow.transform.localRotation = Quaternion.Euler(90, 0, Mathf.Lerp(compassRotationAngleStart, compassRotationAngleDestination, curvedT));

        if (timeElapsed >= compassRotationDuration)
        {
            timeElapsed = 0f;
        }
    }
}
