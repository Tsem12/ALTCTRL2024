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
    public UnityEvent OnWinEvent;
    public bool test;
    [SerializeField] private float limitAngle;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private AudioClip _victorySound;

    private bool isPlayerAlive = true;
    private bool hasMoved = false;
    private bool isStillInGame = true;

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
        if(playerMovement.GetDistance() >= 10f && isStillInGame == true)
        {
            isStillInGame = false;
            AudioSource.PlayClipAtPoint(_victorySound, Camera.main.transform.position);
            OnWinEvent.Invoke();
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
