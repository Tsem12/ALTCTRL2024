using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroControler : MonoBehaviour
{
	private List<Joycon> joycons;

	[SerializeField] private Transform _parent;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;
    private Vector3 _refEuler;

    void Start ()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
		_refEuler = joycons[jc_ind].GetVector().eulerAngles;
    }


    void Update () {

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
		

        stick = j.GetStick();

        // Gyro values: x, y, z axis values (in radians per second)
        gyro = j.GetGyro();

        // Accel values:  x, y, z axis values (in Gs)
        accel = j.GetAccel();

        orientation = j.GetVector();

        transform.Rotate(0,(gyro.x)/2,0);
        _parent.Rotate(0,0,-(gyro.y)/2);
        
    }

    private void OnGUI()
    {
	    bool isGyroStable = ToolBox.Approximately(0, gyro.x, ToolBox.Epsilone) && ToolBox.Approximately(0, gyro.y, ToolBox.Epsilone) && ToolBox.Approximately(0, gyro.z, ToolBox.Epsilone);
	    string gyroStability = isGyroStable ? "Stable" : "Not stable";
	    
	    bool isAccelAlign = ToolBox.Approximately(0, accel.x, .01f) && ToolBox.Approximately(0, accel.y, .01f) && ToolBox.Approximately(-1f, accel.z, .01f);
	    string accelAlign = isAccelAlign ? "Aligned" : "Not aligned";
	    GUILayout.Label($"Gyroscope Values => {gyroStability} => {gyro.ToString()} \n"+ $"Acceleration Values => {accelAlign} =>  {accel}", new GUIStyle(){fontSize = 60});
    }
}
