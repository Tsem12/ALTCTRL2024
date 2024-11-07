using System.Collections;
using System.Collections.Generic;
using System.Net;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class WindScript : MonoBehaviour
{
    public static WindScript instance;
    [SerializeField] private GameObject windOrigin;
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> windEffectList;
    private GameObject windEffect;

    private AudioClip windSound;
    [SerializeField] private GameObject compassArrow;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float compassRotationDuration = 0.5f;
    [SerializeField] private float compassShakeIntensity = 2;

    [SerializeField] private float treshold;
    private float compassRotationAngleDestination;
    private float compassRotationAngleStart;

    private bool isCompassRotating = false;
    private float timeElapsed = 0f;

    public UnityEvent OnWindBlowing;
    public UnityEvent OnWindStopBlowing;

    private bool isWindBlowing = false;
    [HideInInspector]
    public Direction _windDirection;
    
    //public bool test;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de WindManager dans la scene");
            return;
        }
        instance = this;
        windOrigin.SetActive(false);
    }

    private void Update()
    {
        if (isCompassRotating)
        {
            Rotate();
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

        if (isWindBlowing)
        {
            float perchRoll = GyroControler.instance.GetPerchRoll;
            if (_windDirection == Direction.West || _windDirection == Direction.NorthWest || _windDirection == Direction.SouthWest)
            {
                if(perchRoll > treshold)
                {
                    StopWind();
                    OnWindStopBlowing?.Invoke();
                }
            }
            else if (_windDirection == Direction.East || _windDirection == Direction.NorthEast || _windDirection == Direction.SouthEast)
            {
                if(perchRoll< -treshold)
                {
                    StopWind();
                    OnWindStopBlowing?.Invoke();
                }
            }
        }
        /*
        if (test)
        {
            PlayWindToDirection(WindDirection.West, 10);
            OnWindBlowing.Invoke();
        }
        */
    }

    public bool GetIsWindBlowing()
    {
        return isWindBlowing;
    }

    public void TriggerWind()
    {
        int windDirInt = Random.Range(1,7);
        PlayWindToDirection((Direction)windDirInt, 7);
        OnWindBlowing.Invoke();
    }
    /*
    private void Start()
    {
        PlayWindToDirection(WindDirection.West, 10);
    }
    */
    void ChooseRandomGameObject()
    {
        windEffect = windEffectList[Random.Range(0, windEffectList.Count)];
    }

    public void PlayWindToDirection(Direction windDirection, float duration)
    {
        _windDirection = windDirection;
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

    public void StopWind()
    {
        if (windEffect != null)
        {
            windOrigin.SetActive(false);
            isWindBlowing = false;
            isCompassRotating = false;
            SpatializedSoundScript.Instance.StopCurrentAudioSource();
        }
    }

    IEnumerator DisableWindAfterDuration(float duration)
    {
        isWindBlowing = true;
        yield return new WaitForSeconds(duration);
        if (isWindBlowing)
        {
            windOrigin.SetActive(false);
            windEffect.SetActive(false);
            isWindBlowing = false;
            OnWindStopBlowing.Invoke();
            GameManager.OnLoseEvent?.Invoke();
        }
    }

    private void PlayWindSoundFromDirection(Direction windDirection)
    {
        SpatializedSoundScript.Instance.PlayAudioClipAtDirection(windDirection);
    }

    private Vector3 GetWindOrigin(Direction windDirection)
    {
        return SpatializedSoundScript.Instance.WindOrigins[(int)windDirection].position;
    }

    private void RotateCompassWithDirection(Direction windDirection)
    {
        switch (windDirection)
        {
            case Direction.North:
                RotateCompassActionJuicily(180);
                break;
            case Direction.NorthEast:
                RotateCompassActionJuicily(135);
                break;
            case Direction.East:
                RotateCompassActionJuicily(90);
                break;
            case Direction.SouthEast:
                RotateCompassActionJuicily(45);
                break;
            case Direction.South:
                RotateCompassActionJuicily(0);
                break;
            case Direction.SouthWest:
                RotateCompassActionJuicily(225);
                break;
            case Direction.West:
                RotateCompassActionJuicily(270);
                break;
            case Direction.NorthWest:
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
    
    [Button]
    public void TestWind() => TriggerWind();
}
