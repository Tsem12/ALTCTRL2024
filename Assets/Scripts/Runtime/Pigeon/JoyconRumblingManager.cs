using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconRumblingManager : MonoBehaviour
{
    [SerializeField] private JoyconIdConfig _joyconIdConfig;

    private List<Joycon> _joycons;
    private bool _areJoyconsConneted = true;

    public Func<Rumbling> OnRumbleReceived; 
    void Start ()
    {
        // get the public Joycon array attached to the JoyconManager in scene
        _joycons = JoyconManager.Instance.j;
        if (_joycons.Count > _joyconIdConfig.GetMaxId)
        {
            Debug.LogError("Joycons are not connected");
            _areJoyconsConneted = false;
        }
    }
    
    
    void ComputeVibration(int joyconId) {
        // make sure the Joycon only gets checked if attached
        Joycon j = _joycons[joyconId];

        if (j.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
            Debug.Log ("Rumble");
            _startPigeon = true;
            _currentTimer = 0;
        }
		
        if (j.GetButtonDown (Joycon.Button.DPAD_UP)) {
            Debug.Log ("Rumble");
            _startPigeon = false;
        }

        if (_startPigeon)
        {
            _currentTimer += Time.deltaTime;
            float low_frequence = Mathf.Lerp(_rumblingData.StartLowFrequence, _rumblingData.EndLowFrequence, _rumblingData.StartToEndCurve.Evaluate(_currentTimer / _rumblingData.StartToEndDuration));
            float high_frequence = Mathf.Lerp(_rumblingData.StartHighFrequence, _rumblingData.EndHighFrequence, _rumblingData.StartToEndCurve.Evaluate(_currentTimer / _rumblingData.StartToEndDuration));
            float amplitude = Mathf.Lerp(_rumblingData.StartAmplitude, _rumblingData.EndAmplitude, _rumblingData.StartToEndCurve.Evaluate(_currentTimer / _rumblingData.StartToEndDuration));
            int timeInMillisec = (int)Mathf.Lerp(_rumblingData.StartTimeInMillisec, _rumblingData.EndTimeInMillisec, _rumblingData.StartToEndCurve.Evaluate(_currentTimer / _rumblingData.StartToEndDuration));
			
            // Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
            // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

            j.SetRumble (low_frequence, high_frequence, amplitude, timeInMillisec);

            // The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
            // (Useful for dynamically changing rumble values.)
            // Then call SetRumble(0,0,0) when you want to turn it off.
        }
        
    }
}
