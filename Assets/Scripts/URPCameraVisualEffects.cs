using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class URPCameraVisualEffects : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;                      // R�f�rence � la cam�ra du joueur
    [SerializeField] private float cameraTiltAmount;             // Angle maximal d'inclinaison de la cam�ra vers le bas
    [SerializeField] private float cameraShakeIntensity;        // Intensit� du tremblement de la cam�ra

    [Header("Post Processing Settings")]
    [SerializeField] private Volume postProcessVolume;                 // R�f�rence au Volume de post-processing (URP)
    private MotionBlur motionBlur;                   // Composant Motion Blur (URP)
    private ChromaticAberration chromaticAberration; // Composant Chromatic Aberration (URP)
    private LensDistortion lensDistorsion;
    private Vignette vignette;

    [Header("Vertigo Effect Settings")]
    [SerializeField] private float maxBlurAmount;               // Angle maximal de flou de mouvement
    [SerializeField] private float maxAberrationAmount;           // Intensit� maximale de l'aberration chromatique
    [SerializeField] private float maxFOVChange;                 // FOV change max (ajout� � la FOV de base)
    [SerializeField] private float maxLensDistorsionIntensity;
    [SerializeField] private float maxVignetteIntensity;

    [SerializeField] private float transitionDuration = 2f;            // Dur�e de la transition des effets de vertige

    private float initialFOV;                        // FOV initial de la cam�ra
    private Quaternion initialCameraRotation;        // Rotation initiale de la cam�ra
    private Vector3 initialCameraLocalPosition;        // Rotation initiale de la cam�ra
    private bool isVertigoActive = false;            // Statut pour savoir si le vertige est en cours

    private void Start()
    {
        // Stocker le FOV initial et la rotation initiale de la cam�ra
        initialFOV = playerCamera.fieldOfView;
        initialCameraRotation = playerCamera.transform.localRotation;
        initialCameraLocalPosition = playerCamera.transform.localPosition;

        // R�cup�rer les composants de post-processing (URP)
        if (postProcessVolume.profile.TryGet<MotionBlur>(out motionBlur))
        {
            motionBlur.active = false; // D�sactiver le motion blur au d�marrage
        }

        if (postProcessVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.active = false; // D�sactiver l'aberration chromatique au d�marrage
        }

        if (postProcessVolume.profile.TryGet<LensDistortion>(out lensDistorsion))
        {
            lensDistorsion.active = false;
        }

        if (postProcessVolume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.active = false;
        }
    }

    public void TriggerVertigo()
    {
        if (!isVertigoActive)
        {
            isVertigoActive = true;
            StopAllCoroutines();
            StartCoroutine(ApplyVertigoEffect());
        }
    }

    public void StopIdle()
    {
        if (isVertigoActive)
        {
            isVertigoActive = false;
            StopAllCoroutines();
            StartCoroutine(ResetVertigoEffects());
        }
    }

    private IEnumerator ApplyVertigoEffect()
    {
        // Activer les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = true;
        if (chromaticAberration != null)
            chromaticAberration.active = true;
        if (lensDistorsion != null)
            lensDistorsion.active = true;
        if (vignette != null)
            vignette.active = true;

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
            if (lensDistorsion != null)
                lensDistorsion.intensity.value = Mathf.Lerp(0, maxLensDistorsionIntensity, lerpFactor);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(0, maxVignetteIntensity, lerpFactor);

            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, initialFOV - maxFOVChange, lerpFactor);

            // Incliner la cam�ra vers le bas
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAmount * lerpFactor, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * 5f);

            // Ajouter un effet de tremblement de la cam�ra
            playerCamera.transform.localPosition += Random.insideUnitSphere * Mathf.Lerp(0, cameraShakeIntensity, lerpFactor); ;

            yield return null; // Attendre une frame
        }

        // Continuer � appliquer le tremblement tant que l'effet est actif
        while (isVertigoActive)
        {
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;
            yield return null;
        }
    }

    private IEnumerator ResetVertigoEffects()
    {

        float elapsedTime = 0f; // Variable pour le temps �coul�

        // Transition progressive de retour
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Ramener les effets progressivement � z�ro
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, 0, lerpFactor);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0, lerpFactor);
            if (lensDistorsion != null)
                lensDistorsion.intensity.value = Mathf.Lerp(lensDistorsion.intensity.value, 0, lerpFactor);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, lerpFactor);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initialFOV, lerpFactor);

            // R�initialiser l'inclinaison de la cam�ra
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, initialCameraRotation, lerpFactor);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPosition, lerpFactor);

            yield return null; // Attendre une frame
        }

        // D�sactiver les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = false;
        if (chromaticAberration != null)
            chromaticAberration.active = false;
        if (lensDistorsion != null)
            lensDistorsion.active = false;
        if (vignette != null)
            vignette.active = false;

        // R�initialiser le FOV et la rotation de la cam�ra
        playerCamera.fieldOfView = initialFOV;
        playerCamera.transform.localRotation = initialCameraRotation;
        playerCamera.transform.localPosition = initialCameraLocalPosition;
    }
}