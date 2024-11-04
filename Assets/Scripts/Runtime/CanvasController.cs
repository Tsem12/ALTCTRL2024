using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject losingScreen;

    [SerializeField] private TextMeshProUGUI losingText;

    [SerializeField] private List<AudioClip> losingSound;
    [SerializeField] private AudioClip fallSound;

    [Header("Fade")] 
    [SerializeField] private Image _fadeBg;
    [SerializeField] private Ease _fallFadeCurve;
    [SerializeField] private Ease _respawnFadeCurve;

    public void OnLose()
    {
        AudioSource.PlayClipAtPoint(fallSound, Camera.main.transform.position);
        StartCoroutine(DebugLosingScreen());
    }
    
    public void FallFade(float duration) => _fadeBg.DOColor(new Vector4(0, 0, 0, 1), duration).SetEase(_fallFadeCurve);
    public void RespawnFade(float duration) => _fadeBg.DOColor(new Vector4(0, 0, 0, 0), duration).SetEase(_respawnFadeCurve);

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

    public void OnWin()
    {
        losingText.text = "Ty as gagné le sang (avec tt le respect)";
        losingScreen.SetActive(true);
    }

    void PlayRandomAudioClip()
    {
        AudioClip clip = losingSound[Random.Range(0, losingSound.Count)];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }
}