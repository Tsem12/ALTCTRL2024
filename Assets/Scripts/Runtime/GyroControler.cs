using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroControler : MonoBehaviour
{
	private List<Joycon> joycons;

	[SerializeField] private bool _enableDebugLabels;
	[SerializeField] private float _maxPitch;
	[SerializeField] private float _pitchTriggerTreshold;
    [SerializeField] private int jc_ind = 0;

    // Values made available via Unity
    private Vector3 gyro;
    private Vector3 accel;
    private Quaternion orientation;
    private float _currentPitch;
    public float GetPerchRoll => transform.localEulerAngles.z - 270;
    public float GetPerchPitch => _currentPitch;
    
    void Start ()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
    }


    void Update () 
    {
		if (joycons.Count < 0)
			return;
		
		Joycon j = joycons [jc_ind];
		if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
		{
			Debug.Log ("Shoulder button 2 pressed");
			// GetStick returns a 2-element vector with x/y joystick components
			Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}",j.GetStick()[0],j.GetStick()[1]));
            
			// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
			j.Recenter ();
		}

        // Gyro values: x, y, z axis values (in radians per second)
        gyro = j.GetGyro();

        // Accel values:  x, y, z axis values (in Gs)
        accel = j.GetAccel();

        orientation = j.GetVector();
        
        orientation.ToAngleAxis(out float angle, out Vector3 axis);

        if (accel.y < .95f && accel.y > -.95f)
        {
	        Quaternion rotationArroundRoll = Quaternion.AngleAxis(angle * axis.x, Vector3.forward);
			/*
	        transform.localRotation = rotationArroundRoll;	
			gameObject.transform.localRotation = Quaternion.RotateTowards(
			gameObject.transform.localRotation,
			rotationArroundRoll,
			300 * Time.deltaTime);
			*/
			transform.localRotation = rotationArroundRoll;	
	        // if (Mathf.Abs(accel.y) > _pitchTriggerTreshold)
	        // {
		       //  float pitch = -Mathf.Lerp(-_maxPitch, _maxPitch, (accel.y + .5f) / 1f);
		       //  _currentPitch = pitch;
		       //  transform.localEulerAngles = new Vector3(pitch, transform.localEulerAngles.y ,transform.localEulerAngles.z);
	        // }
	        // else
	        // {
		       //  _currentPitch = 0;
	        // }
        }
        
    }

    private void OnGUI()
    {
	    if(!_enableDebugLabels)
		    return;
	    
	    bool isGyroStable = ToolBox.Approximately(0, gyro.x, ToolBox.Epsilone) && ToolBox.Approximately(0, gyro.y, ToolBox.Epsilone) && ToolBox.Approximately(0, gyro.z, ToolBox.Epsilone);
	    string gyroStability = isGyroStable ? "Stable" : "Not stable";
	    
	    bool isAccelAlign = ToolBox.Approximately(0, accel.x, .01f) && ToolBox.Approximately(0, accel.y, .01f) && ToolBox.Approximately(-1f, accel.z, .01f);
	    string accelAlign = isAccelAlign ? "Aligned" : "Not aligned";
	    GUILayout.Label($"Gyroscope Values => {gyroStability} => {gyro.ToString()} \n"+ $"Acceleration Values => {accelAlign} =>  {accel}", new GUIStyle(){fontSize = 60});
	    GUILayout.Label($"{1/Time.deltaTime}");
    }
}
