using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestRumble : MonoBehaviour {
	
	private List<Joycon> joycons;

    
    public int jc_ind = 0;
    public bool isRumblingInfinitly;
    public float low_frequence = 160;
    public float high_frequence = 320;
    public float amplitude = .6f;
    public int timeInMillisec = 200;


    void Start ()
    {
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
	}

    // Update is called once per frame
    void Update () {
		// make sure the Joycon only gets checked if attached
		if (joycons.Count > 0)
        {
			Joycon j = joycons [jc_ind];

			if (j.GetButtonDown (Joycon.Button.DPAD_DOWN) || isRumblingInfinitly) {
				Debug.Log ("Rumble");

				// Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
				// https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

				j.SetRumble (low_frequence, high_frequence, amplitude, timeInMillisec);

				// The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
                // (Useful for dynamically changing rumble values.)
				// Then call SetRumble(0,0,0) when you want to turn it off.
			}
        }
    }
}