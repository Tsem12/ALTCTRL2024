using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class URPCameraVisualEffects : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;                      // Référence à la caméra du joueur
    public float cameraTiltAmount = 10f;             // Angle maximal d'inclinaison de la caméra vers le bas
    public float cameraShakeIntensity = 0.1f;        // Intensité du tremblement de la caméra

    [Header("Post Processing Settings")]
    public Volume postProcessVolume;                 // Référence au Volume de post-processing (URP)
    private MotionBlur motionBlur;                   // Composant Motion Blur (URP)
    private ChromaticAberration chromaticAberration; // Composant Chromatic Aberration (URP)

    [Header("Vertigo Effect Settings")]
    public float maxBlurAmount = 180f;               // Angle maximal de flou de mouvement
    public float maxAberrationAmount = 1f;           // Intensité maximale de l'aberration chromatique
    public float maxFOVChange = 20f;                 // FOV change max (ajouté à la FOV de base)

    public float transitionDuration = 2f;            // Durée de la transition des effets de vertige

    private float initialFOV;                        // FOV initial de la caméra
    private Quaternion initialCameraRotation;        // Rotation initiale de la caméra
    private bool isVertigoActive = false;            // Statut pour savoir si le vertige est en cours

    private void Start()
    {
        // Stocker le FOV initial et la rotation initiale de la caméra
        initialFOV = playerCamera.fieldOfView;
        initialCameraRotation = playerCamera.transform.localRotation;

        // Récupérer les composants de post-processing (URP)
        if (postProcessVolume.profile.TryGet<MotionBlur>(out motionBlur))
        {
            motionBlur.active = false; // Désactiver le motion blur au démarrage
        }

        if (postProcessVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.active = false; // Désactiver l'aberration chromatique au démarrage
        }
    }

    public void TriggerVertigo()
    {
        if (!isVertigoActive)
        {
            isVertigoActive = true;
            StartCoroutine(ApplyVertigoEffect());
        }
    }

    public void StopIdle()
    {
        if (isVertigoActive)
        {
            isVertigoActive = false;
            StopAllCoroutines();
            ResetVertigoEffects();
        }
    }

    private IEnumerator ApplyVertigoEffect()
    {
        // Activer les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = true;
        if (chromaticAberration != null)
            chromaticAberration.active = true;

        float elapsedTime = 0f; // Variable pour le temps écoulé

        // Appliquer les effets progressivement
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Appliquer les effets avec lerp
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(0, maxBlurAmount, lerpFactor);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(0, maxAberrationAmount, lerpFactor);

            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, initialFOV - maxFOVChange, lerpFactor);

            // Incliner la caméra vers le bas
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAmount * lerpFactor, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * 5f);

            // Ajouter un effet de tremblement de la caméra
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;

            yield return null; // Attendre une frame
        }

        // Une fois la transition terminée, les effets restent à leur maximum
        if (motionBlur != null)
            motionBlur.intensity.value = maxBlurAmount;
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = maxAberrationAmount;
        playerCamera.fieldOfView = initialFOV - maxFOVChange;

        // Continuer à appliquer le tremblement tant que l'effet est actif
        while (isVertigoActive)
        {
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;
            yield return null;
        }
    }

    private void ResetVertigoEffects()
    {
        // Réinitialiser les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = false;
        if (chromaticAberration != null)
            chromaticAberration.active = false;

        // Réinitialiser le FOV et la rotation de la caméra
        playerCamera.fieldOfView = initialFOV;
        playerCamera.transform.localRotation = initialCameraRotation;
        playerCamera.transform.localPosition = Vector3.zero;
    }
}
