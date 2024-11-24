using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static UnityEvent OnLoseEvent = new UnityEvent();
    public static UnityEvent OnWinEvent = new UnityEvent();
    public static UnityEvent OnRespawnEvent = new UnityEvent();

    public bool test;
    [SerializeField] private float limitAngle;

    [SerializeField] private SFX _victorySound;

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

    private void OnEnable()
    {
        OnRespawnEvent.AddListener(OnRespawn);
        OnLoseEvent.AddListener(OnLose);
        OnWinEvent.AddListener(OnWin);
    }

    private void OnDisable()
    {
        OnRespawnEvent.RemoveAllListeners();
        OnLoseEvent.RemoveAllListeners();
        OnWinEvent.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && isPlayerAlive)
        {
            OnLoseEvent.Invoke();
        }
        if(Mathf.Abs(GyroControler.instance.GetPerchRoll) > limitAngle && hasMoved && isPlayerAlive)
        {
            OnLoseEvent.Invoke();
        }
        /*
        if(PlayerMovement.instance.GetDistance() >= 85f && isStillInGame == true)
        {
            OnWinEvent.Invoke();
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
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

    public void OnRespawn()
    {
        hasMoved = false;
        isPlayerAlive = true;
    }

    public void OnLose()
    {
        isPlayerAlive = false;
    }

    public void OnWin()
    {
        isStillInGame = false;
        _victorySound.PlaySfx();
    }

}
