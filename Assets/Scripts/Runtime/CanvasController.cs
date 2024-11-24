using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static CanvasController instance;

    [SerializeField] private GameObject losingScreen;

    [SerializeField] private TextMeshProUGUI losingText;
    
    [SerializeField] private SFX _ropeSFX;
    [SerializeField] private SFX _fallingSFX;

    [Header("Fade")] 
    [SerializeField] private Image _fadeBg;
    [SerializeField] private Ease _fallFadeCurve;
    [SerializeField] private Ease _respawnFadeCurve;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de CanvasController dans la scene");
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnLoseEvent.AddListener(OnLose);
        GameManager.OnWinEvent.AddListener(OnWin);
        GameManager.OnRespawnEvent.AddListener(OnRespawn);
    }

    private void OnDisable()
    {
        GameManager.OnLoseEvent.RemoveAllListeners();
        GameManager.OnWinEvent.RemoveAllListeners();
        GameManager.OnRespawnEvent.RemoveAllListeners();
    }

    public void OnLose()
    {
        _fallingSFX.PlaySfx();
        _ropeSFX.PlaySfx();
        FallFade(3f);
        //StartCoroutine(DebugLosingScreen());
    }

    public void OnRespawn()
    {
        RespawnFade(0.2f);
    }

    public void FallFade(float duration) => _fadeBg.DOColor(new Vector4(0, 0, 0, 1), duration).SetEase(_fallFadeCurve);
    public void StartFade(float duration) => _fadeBg.DOColor(new Vector4(0, 0, 0, 1), duration).SetEase(_fallFadeCurve).SetLoops(2, LoopType.Yoyo);
    public void RespawnFade(float duration) => _fadeBg.DOColor(new Vector4(0, 0, 0, 0), duration).SetEase(_respawnFadeCurve);

    private IEnumerator DebugLosingScreen()
    {
        losingScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        losingScreen.SetActive(false);
    }

    public void OnWin()
    {
        //losingText.text = "Ty as gagn� le sang (avec tt le respect)";
        losingScreen.SetActive(true);
    }
}