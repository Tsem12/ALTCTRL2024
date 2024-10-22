using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject losingScreen;

    public void OnLose()
    {
        StartCoroutine(DebugLosingScreen());
    }

    private IEnumerator DebugLosingScreen()
    {
        losingScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        losingScreen.SetActive(false);
    }
}
