using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject losingScreen;

    [SerializeField] private List<AudioClip> losingSound;
    [SerializeField] private AudioClip fallSound;

    public void OnLose()
    {
        AudioSource.PlayClipAtPoint(fallSound, Camera.main.transform.position);
        StartCoroutine(DebugLosingScreen());
    }

    private IEnumerator DebugLosingScreen()
    {
        losingScreen.SetActive(true);
        if (losingSound != null && losingSound.Count > 0)
        {
            PlayRandomAudioClip();
        }
        yield return new WaitForSeconds(3f);
        losingScreen.SetActive(false);
    }

    void PlayRandomAudioClip()
    {
        AudioClip clip = losingSound[Random.Range(0, losingSound.Count)];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}