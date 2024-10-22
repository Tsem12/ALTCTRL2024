using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject losingScreen;

    public void OnLose()
    {
        losingScreen.SetActive(true);
    }
}
