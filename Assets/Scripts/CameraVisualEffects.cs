using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class URPCameraVisualEffects : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;                      // R�f�rence � la cam�ra du joueur
    public float cameraTiltAmount = 10f;             // Angle maximal d'inclinaison de la cam�ra vers le bas
    public float cameraShakeIntensity = 0.1f;        // Intensit� du tremblement de la cam�ra

    [Header("Post Processing Settings")]
    public Volume postProcessVolume;                 // R�f�rence au Volume de post-processing (URP)
    private MotionBlur motionBlur;                   // Composant Motion Blur (URP)
    private ChromaticAberration chromaticAberration; // Composant Chromatic Aberration (URP)

    [Header("Vertigo Effect Settings")]
    public float maxBlurAmount = 180f;               // Angle maximal de flou de mouvement
    public float maxAberrationAmount = 1f;           // Intensit� maximale de l'aberration chromatique
    public float maxFOVChange = 20f;                 // FOV change max (ajout� � la FOV de base)

    public float transitionDuration = 2f;            // Dur�e de la transition des effets de vertige

    private float initialFOV;                        // FOV initial de la cam�ra
    private Quaternion initialCameraRotation;        // Rotation initiale de la cam�ra
    private bool isVertigoActive = false;            // Statut pour savoir si le vertige est en cours

    private void Start()
    {
        // Stocker le FOV initial et la rotation initiale de la cam�ra
        initialFOV = playerCamera.fieldOfView;
        initialCameraRotation = playerCamera.transform.localRotation;

        // R�cup�rer les composants de post-processing (URP)
        if (postProcessVolume.profile.TryGet<MotionBlur>(out motionBlur))
        {
            motionBlur.active = false; // D�sactiver le motion blur au d�marrage
        }

        if (postProcessVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.active = false; // D�sactiver l'aberration chromatique au d�marrage
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

        float elapsedTime = 0f; // Variable pour le temps �coul�

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

            // Incliner la cam�ra vers le bas
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAmount * lerpFactor, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * 5f);

            // Ajouter un effet de tremblement de la cam�ra
            playerCamera.transform.localPosition += Random.insideUnitSphere * Mathf.Lerp(0, cameraShakeIntensity, lerpFactor);

            yield return null; // Attendre une frame
        }

        // Une fois la transition termin�e, les effets restent � leur maximum
        if (motionBlur != null)
            motionBlur.intensity.value = maxBlurAmount;
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = maxAberrationAmount;
        playerCamera.fieldOfView = initialFOV - maxFOVChange;

        // Continuer � appliquer le tremblement tant que l'effet est actif
        while (isVertigoActive)
        {
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;
            yield return null;
        }
    }

    private void ResetVertigoEffects()
    {
        // R�initialiser les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = false;
        if (chromaticAberration != null)
            chromaticAberration.active = false;

        // R�initialiser le FOV et la rotation de la cam�ra
        playerCamera.fieldOfView = initialFOV;
        playerCamera.transform.localRotation = initialCameraRotation;
        playerCamera.transform.localPosition = Vector3.zero;
    }
}
