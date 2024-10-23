using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GyroControler gyroControler;
    public UnityEvent OnLoseEvent;

    [SerializeField] private float limitAngle;

    private bool isPlayerAlive = true;
    private bool hasMoved = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de GameManager dans la scene");
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if(Mathf.Abs(gyroControler.GetPerchRoll) > limitAngle && hasMoved && isPlayerAlive)
        {
            isPlayerAlive = false;
            OnLoseEvent.Invoke();
        }
    }

    public bool GetIsPlayerAlive()
    {
        return isPlayerAlive;
    }

    public void SetIsPlayerAlive(bool target)
    {
        isPlayerAlive=target;
    }

    public bool GetHasMoved()
    {
        return hasMoved;
    }

    public void SetHasMoved(bool target)
    {
        hasMoved = target;
    }
}
