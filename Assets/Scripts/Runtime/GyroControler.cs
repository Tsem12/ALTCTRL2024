using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GyroControler : MonoBehaviour
{
	private List<Joycon> joycons;

	[SerializeField] private bool _enableDebugLabels;
	[SerializeField] private float _maxPitch;
	[SerializeField] private float _pitchTriggerTreshold;
	[SerializeField] private int _shakeTreshold;
    [SerializeField] private int jc_ind = 0;
    
    [Header("ShakeValues")]
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
    public float GetPerchPitch => _currentPitch;
    
    void Start ()
    {
	    _initPos = transform.localPosition;
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
	    OnShakePerch?.Invoke();
	    _shakeTween = transform.DOShakePosition(_shakeDuration, _shakeVibrato).SetEase(_shakeEase).SetLoops(-1, LoopType.Yoyo);
	    _shakeTween.Play();
	    yield return new WaitUntil(() => accel.magnitude < 1);
	    _shakeTween.Kill();
	    transform.DOLocalMove(_initPos, _shakeRecoveryDuration).SetEase(_shakeRecoveryEase);
	    _shakeRoutine = null;
	    joycons[jc_ind].Recenter();

    }

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
