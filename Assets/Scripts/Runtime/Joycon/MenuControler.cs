using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuControler : MonoBehaviour
{

	private List<Joycon> joycons;

	[SerializeField] private Image[] _buttons;
	[SerializeField] private MenuStart _menuStart;
	[FormerlySerializedAs("_accelThreshold")] [SerializeField] private float _angleThreshold = 0.1f;
	private int _currentButtonIndex;

	private Image _selectedButton;
	private Image SelectedButton
	{
		set
		{
			if(value == _selectedButton) return;

			if (_selectedButton != null)
			{
				_selectedButton.color = new Color(1, 1, 1, .7f);
			}
			_selectedButton = value;
			_selectedButton.color = new Color(1, 1, 1, 1f);
		}
	}
	
    [SerializeField] private JoyconIdConfig jc_ind;
    
    [Header("ShakeValues")]
	[SerializeField] private float _shakeTreshold;
    [SerializeField] float _minTimeToShake = .5f;
    [SerializeField] float _shakeDuration = .1f;

    // Values made available via Unity
    private Vector3 gyro;
    private Vector3 accel;
    private Quaternion orientation;
    private Vector3 _initPos;
    private Coroutine _shakeRoutine;
    private Tween _shakeTween;

    public static UnityEvent OnShakePerch = new UnityEvent();
    
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
		if (joycons.Count <= 0)
			return;
		
		Joycon j = joycons [jc_ind.CenterJoyconId];

        // Gyro values: x, y, z axis values (in radians per second)
        gyro = j.GetVector().eulerAngles;
        
        // Accel values:  x, y, z axis values (in Gs)
        accel = j.GetAccel();

        

        if (gyro.z <= 90 - _angleThreshold)
        {
	        SelectedButton = _buttons[0];
	        _currentButtonIndex = 0;
        }
        else if (accel.z >= 90 + _angleThreshold)
        {
	        SelectedButton = _buttons[2];
	        _currentButtonIndex = 2;
        }
        else
        {
	        SelectedButton = _buttons[1];
	        _currentButtonIndex = 1;
        }

        if (accel.magnitude >= _shakeTreshold)
        {
	        Click();
        }
        
    }

    private void Click()
    {
		_menuStart.OnInputReceived(_currentButtonIndex);
    }

    private void OnGUI()
    {
	    GUILayout.Label($"Shake value => {accel.magnitude}, Gyro {orientation.eulerAngles}", new GUIStyle(){fontSize = 60});
	    Debug.Log($"Shake value => {(int)accel.magnitude}");
    }
}
