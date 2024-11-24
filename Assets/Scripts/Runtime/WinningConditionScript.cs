using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningConditionScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("toto");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Fin du jeu");
            GameManager.OnWinEvent.Invoke();
        }
    }
}
