using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class GyroControler : MonoBehaviour
{
	private List<Joycon> joycons;

	[SerializeField] private bool _enableDebugLabels;
	[SerializeField] private float _maxPitch;
	[SerializeField] private float _pitchTriggerTreshold;
	[SerializeField] private int _shakeTreshold;
    [SerializeField] private JoyconIdConfig jc_ind;
    
    [Header("ShakeValues")]
    [SerializeField] float _minTimeToShake = .5f;
    [SerializeField] float _shakeDuration = .1f;
    [SerializeField] Vector3 _shakeForce = new Vector3(0,.25f,0);
    [SerializeField] int _shakeVibrato = 1;
    [SerializeField] Ease _shakeEase = Ease.OutCubic;
    [SerializeField] float _shakeRecoveryDuration = .5f;
    [SerializeField] private Ease _shakeRecoveryEase = Ease.InOutExpo;

    // Values made available via Unity
    private Vector3 gyro;
    private Vector3 accel;
    private Quaternion orientation;
    private Vector3 _initPos;
    private Coroutine _shakeRoutine;
    private Tween _shakeTween;

    public Action OnShakePerch;
    
    private float _currentPitch;
    public float GetPerchRoll => transform.localEulerAngles.z - 270;
    
    void Start ()
    {
	    _initPos = transform.localPosition;
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind.CenterJoyconId+1)
		{
			Debug.LogError("No Joycon for Gyroscope");
		}
    }


    void Update () 
    {
		if (joycons.Count < 0)
			return;
		
		Joycon j = joycons [jc_ind.CenterJoyconId];
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

        if (accel.y < .95f && accel.y > -.95f && _shakeRoutine == null)
        {
	        Quaternion rotationArroundRoll = Quaternion.AngleAxis(angle * axis.x, Vector3.forward);
	        transform.localRotation = rotationArroundRoll;	
	        gameObject.transform.localRotation = Quaternion.RotateTowards(
		        gameObject.transform.localRotation,
		        rotationArroundRoll,
		        10);
			//transform.localRotation = rotationArroundRoll;	
        }

        if (accel.magnitude >= _shakeTreshold)
        {
	        if (_shakeRoutine == null)
	        {
		        _shakeRoutine = StartCoroutine(ShakeRoutine());
	        }
        }
        
    }

    IEnumerator ShakeRoutine()
    {
	    _shakeTween = transform.DOShakePosition(_shakeDuration, _shakeVibrato).SetEase(_shakeEase).SetLoops(-1, LoopType.Yoyo);
	    _shakeTween.Play();
	    float timer = 0f;
	    while (accel.magnitude > 1)
	    {
		    timer += Time.deltaTime;
		    if (timer >= _minTimeToShake)
		    {
				OnShakePerch?.Invoke();
		    }
		    yield return null;
	    }
	    _shakeTween.Kill();
	    transform.DOLocalMove(_initPos, _shakeRecoveryDuration).SetEase(_shakeRecoveryEase);
	    _shakeRoutine = null;
	    joycons[jc_ind.CenterJoyconId].Recenter();

    }
    
    [Button]
    public void TestShake() => OnShakePerch?.Invoke();

    private void OnGUI()
    {
	    if(!_enableDebugLabels)
		    return;
	    
	    bool isAccelAlign = ToolBox.Approximately(0, accel.x, .01f) && ToolBox.Approximately(0, accel.y, .01f) && ToolBox.Approximately(-1f, accel.z, .01f);
	    string accelAlign = isAccelAlign ? "Aligned" : "Not aligned";
	    GUILayout.Label($"Shake value => {(int)accel.magnitude}", new GUIStyle(){fontSize = 60});
	    Debug.Log($"Shake value => {(int)accel.magnitude}");
    }
}
