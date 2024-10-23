using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RumblingData : ScriptableObject
{
    [SerializeField] private float _startToEndDuration;
    [SerializeField] private AnimationCurve _startToEndCurve;
    
    [Header("startValues")]
    [SerializeField] private float _startLowFrequence = 160;
    [SerializeField] private float _startHighFrequence = 320;
    [SerializeField] private float _startAmplitude = .6f;
    [SerializeField] private int _startTimeInMillisec = 200;
    
    [Header("endValues")]
    [SerializeField] private float _endLowFrequence = 160;
    [SerializeField] private float _endHighFrequence = 320;
    [SerializeField] private float _endAmplitude = .6f;
    [SerializeField] private int _endTimeInMillisec = 200;

    
    public float StartToEndDuration => _startToEndDuration;
    public AnimationCurve StartToEndCurve => _startToEndCurve;
    public float StartLowFrequence => _startLowFrequence;
    public float StartHighFrequence => _startHighFrequence;
    public float StartAmplitude => _startAmplitude;
    public int StartTimeInMillisec => _startTimeInMillisec;
    public float EndLowFrequence => _endLowFrequence;
    public float EndHighFrequence => _endHighFrequence;
    public float EndAmplitude => _endAmplitude;
    public int EndTimeInMillisec => _endTimeInMillisec;
}
