using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class CameraVisualEffects : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;                      // R�f�rence � la cam�ra du joueur
    public float cameraTiltAmount;             // Angle maximal d'inclinaison de la cam�ra vers le bas
    public float cameraShakeIntensity;        // Intensit� du tremblement de la cam�ra

    [Header("Post Processing Settings")]
    public PostProcessVolume postProcessVolume;      // R�f�rence au volume de post-processing
    private MotionBlur motionBlur;                   // Composant Motion Blur
    private ChromaticAberration chromaticAberration; // Composant Chromatic Aberration

    [Header("Vertigo Effect Settings")]
    public float maxBlurAmount;               // Angle maximal de flou de mouvement
    public float maxAberrationAmount;           // Intensit� maximale de l'aberration chromatique
    public float maxFOVChange;                 // FOV change max (ajout� � la FOV de base)

    public float transitionDuration;             // Dur�e de la transition des effets de vertige

    private float initialFOV;                        // FOV initial de la cam�ra
    private Quaternion initialCameraRotation;        // Rotation initiale de la cam�ra
    private bool isVertigoActive = false;            // Statut pour savoir si le vertige est en cours

    private void Start()
    {
        // Stocker le FOV initial et la rotation initiale de la cam�ra
        initialFOV = playerCamera.fieldOfView;

        // R�cup�rer les composants de post-processing (Motion Blur et Chromatic Aberration)
        postProcessVolume.profile.TryGetSettings(out motionBlur);
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);

        // D�sactiver les effets au d�marrage
        motionBlur.enabled.value = false;
        chromaticAberration.enabled.value = false;
    }

    public void TriggerVertigo()
    {
        if (!isVertigoActive)
        {
            isVertigoActive = true;
            StartCoroutine(ApplyVertigoEffect());
        }
    }

    public void StopIdle() // Changer StopVertigo en StopIdle
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
        motionBlur.enabled.value = true;
        chromaticAberration.enabled.value = true;
        initialCameraRotation = playerCamera.transform.localRotation;

        float elapsedTime = 0f; // Variable pour le temps �coul�

        // Appliquer les effets progressivement
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Appliquer les effets avec lerp
            motionBlur.shutterAngle.value = Mathf.Lerp(0, maxBlurAmount, lerpFactor);
            chromaticAberration.intensity.value = Mathf.Lerp(0, maxAberrationAmount, lerpFactor);
            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, initialFOV - maxFOVChange, lerpFactor);

            // Incliner la cam�ra vers le bas
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAmount * lerpFactor, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * 5f);

            // Ajouter un effet de tremblement de la cam�ra pendant la transition
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;

            yield return null; // Attendre une frame
        }

        // Une fois la transition termin�e, les effets restent � leur maximum
        motionBlur.shutterAngle.value = maxBlurAmount;
        chromaticAberration.intensity.value = maxAberrationAmount;
        playerCamera.fieldOfView = initialFOV - maxFOVChange;

        // Continuer le tremblement de la cam�ra tant que l'effet de vertige est actif
        while (isVertigoActive)
        {
            // Ajouter un effet de tremblement de la cam�ra
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;

            yield return null; // Attendre une frame
        }
    }

    private void ResetVertigoEffects()
    {
        // R�initialiser les effets de post-processing
        motionBlur.enabled.value = false;
        chromaticAberration.enabled.value = false;

        // R�initialiser le FOV et la rotation de la cam�ra
        playerCamera.fieldOfView = initialFOV;
        playerCamera.transform.localRotation = initialCameraRotation;
        playerCamera.transform.localPosition = new Vector3(0, 0, 0); // R�initialiser la position
    }
}
