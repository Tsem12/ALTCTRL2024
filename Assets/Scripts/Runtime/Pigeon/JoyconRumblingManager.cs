using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconRumblingManager : MonoBehaviour
{
    public static JoyconRumblingManager Instance;
    [SerializeField] private bool _enbleRumbling;
    [SerializeField] private JoyconIdConfig _joyconIdConfig;

    private Coroutine _leftRumbleRoutine;
    private Coroutine _rightRumbleRoutine;
    private Coroutine _centerRumbleRoutine;

    private List<Joycon> _joycons;
    private bool _areJoyconsConneted = true;

    public Action<Rumbling> OnRumbleReceived;
    public Action<JoyconLocalisation> OnRumbleStop;

    private void Awake()
    {
        if(Instance != null)
            Destroy(gameObject);

        Instance = this;

        OnRumbleReceived += StartRumble;
        OnRumbleStop += StopRumble;
    }
    
    void Start ()
    {
        _joycons = JoyconManager.Instance.j;
        if (_joycons.Count > _joyconIdConfig.GetMaxId)
        {
            Debug.LogError("Joycons are not connected");
            _areJoyconsConneted = false;
        }
    }

    #region RoutinesSetup

    private void StartRumble(Rumbling rumble)
    {
        if(!_enbleRumbling)
            return;
        
        switch (rumble.Localisation)
        {
            case JoyconLocalisation.left:
                SetupRoutines(rumble.RumblingData, ref _leftRumbleRoutine, _joyconIdConfig.LeftJoyconId);
                break;
            case JoyconLocalisation.right:
                SetupRoutines(rumble.RumblingData, ref _rightRumbleRoutine, _joyconIdConfig.RightJoyconId);
                break;
            case JoyconLocalisation.middle:
                SetupRoutines(rumble.RumblingData, ref _centerRumbleRoutine, _joyconIdConfig.CenterJoyconId);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void StopRumble(JoyconLocalisation joyconLocalisation)
    {
        if(!_enbleRumbling)
            return;

        switch (joyconLocalisation)
        {
            case JoyconLocalisation.left:
                StopRoutine(ref _leftRumbleRoutine);
                break;
            case JoyconLocalisation.right:
                StopRoutine(ref _rightRumbleRoutine);
                break;
            case JoyconLocalisation.middle:
                StopRoutine(ref _centerRumbleRoutine);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(joyconLocalisation), joyconLocalisation, null);
        }
    }
    private void SetupRoutines(RumblingData data, ref Coroutine routine, int joyconId)
    {
        if (routine != null)
        {
            StopCoroutine(_leftRumbleRoutine);
            routine = null;
        }
        routine = StartCoroutine(ComputeRumbleRoutine(data, joyconId));
    }
    private void StopRoutine(ref Coroutine routine)
    {
        
        StopCoroutine(routine);
        routine = null;
    }

    #endregion
    
    private IEnumerator ComputeRumbleRoutine(RumblingData data, int joyconId)
    {
        if (joyconId >= _joycons.Count)
        {
            throw new Exception($"No Joycon connected for id {joyconId}");
        }
        Joycon j = _joycons[joyconId];
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            float percentage = timer / data.StartToEndDuration;
            float low_frequence = Mathf.Lerp(data.StartLowFrequence, data.EndLowFrequence, data.StartToEndCurve.Evaluate(percentage));
            float high_frequence = Mathf.Lerp(data.StartHighFrequence, data.EndHighFrequence, data.StartToEndCurve.Evaluate(percentage));
            float amplitude = Mathf.Lerp(data.StartAmplitude, data.EndAmplitude, data.StartToEndCurve.Evaluate(percentage));
            int timeInMillisec = (int)Mathf.Lerp(data.StartTimeInMillisec, data.EndTimeInMillisec, data.StartToEndCurve.Evaluate(percentage));
            
            j.SetRumble (low_frequence, high_frequence, amplitude, timeInMillisec);
            yield return null;
        }
    }
}
