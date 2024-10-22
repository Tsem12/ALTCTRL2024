using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public GyroControler gyroControler;
    public UnityEvent OnLoseEvent;

    [SerializeField] private float limitAngle;

    private void Update()
    {
        if(Mathf.Abs(gyroControler.GetPerchRoll) > limitAngle && Time.time > 3f)
        {
            Debug.Log(gyroControler.GetPerchRoll);
            OnLoseEvent.Invoke();
        }
    }
}
