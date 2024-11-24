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
	[SerializeField, Range(0f,1f)] private float _buttonFadePercentageOnSelected = .5f;
	[SerializeField] private MenuStart _menuStart;
	[SerializeField] private PlayerMovement _playerMovement;
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
				_selectedButton.color = new Color(1, 1, 1, 1f);
			}
			_selectedButton = value;
			_selectedButton.color = new Color(1, 1, 1, _buttonFadePercentageOnSelected);
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
	    if (_playerMovement.MovementInput > 0)
	    {
		    SelectedButton = _buttons[0];
		    _currentButtonIndex = 0;
	    }
	    else if (_playerMovement.MovementInput < 0)
	    {
		    SelectedButton = _buttons[2];
		    _currentButtonIndex = 2;
	    }
	    else
	    {
		    SelectedButton = _buttons[1];
		    _currentButtonIndex = 1;
	    }

	    if (accel.magnitude >= _shakeTreshold || Input.GetKeyDown(KeyCode.Space))
	    {
		    Click();
	    }
	    
		if (joycons.Count <= 0)
			return;
		
		Joycon j = joycons [jc_ind.CenterJoyconId];

        // Gyro values: x, y, z axis values (in radians per second)
        gyro = j.GetVector().eulerAngles;
        
        // Accel values:  x, y, z axis values (in Gs)
        accel = j.GetAccel();
        
    }

    private void Click()
    {
		_menuStart.OnInputReceived(_currentButtonIndex);
    }
		
}
