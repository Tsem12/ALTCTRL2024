using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Camera playerCamera;

    [Header("MovementTilt")]
    [SerializeField] private float cameraTiltAngle = 5f;

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;
    [SerializeField] private float baseBobbingAmountX = 0.02f;
    [SerializeField] private float baseBobbingAmountY = 0.01f;
    [SerializeField] private float tiltAngle = 5f;
    private float timer = 0.0f;

    [Header("Fall")]
    [SerializeField] private float fallHeight;
    [SerializeField] private float sideFall;
    [SerializeField] private float fallDuration;
    [SerializeField] private AnimationCurve fallCurve;

    [Header("Wind Settings")]
    [SerializeField] private float windSpeed = 10f;
    [SerializeField] private float windTiltMultiplier = 1f;
    [SerializeField] private float windTranslationMultiplier = 0.1f;

    [Header("VertigoEffect")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private float cameraTiltAmount;
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float maxBlurAmount;
    [SerializeField] private float maxAberrationAmount;
    [SerializeField] private float maxFOVChange;
    [SerializeField] private float maxLensDistorsionIntensity;
    [SerializeField] private float maxVignetteIntensity;

    private MotionBlur motionBlur;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistorsion;
    private Vignette vignette;

    private Coroutine fallCoroutine = null;
    private Coroutine jumpCoroutine = null;
    private Coroutine startVertigoCoroutine = null;
    private Coroutine stopVertigoCoroutine = null;

    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    private float initialFOV;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de CameraController dans la scene");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        initialCameraPosition = playerCamera.transform.localPosition;
        initialCameraRotation = playerCamera.transform.localRotation;
        initialFOV = playerCamera.fieldOfView;


        if (postProcessVolume.profile.TryGet<MotionBlur>(out motionBlur))
            motionBlur.active = false;

        if (postProcessVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
            chromaticAberration.active = false;

        if (postProcessVolume.profile.TryGet<LensDistortion>(out lensDistorsion))
            lensDistorsion.active = false;

        if (postProcessVolume.profile.TryGet<Vignette>(out vignette))
            vignette.active = false;
    }

    private void OnEnable()
    {
        GameManager.OnRespawnEvent.AddListener(OnRespawn);
        GameManager.OnLoseEvent.AddListener(OnLose);
        PlayerMovement.OnStartVertigoEvent.AddListener(OnStartVertigo);
        PlayerMovement.OnStopVertigoEvent.AddListener(OnStopVertigo);
    }

    private void OnDisable()
    {
        GameManager.OnRespawnEvent.RemoveAllListeners();
        GameManager.OnLoseEvent.RemoveAllListeners();
        PlayerMovement.OnStartVertigoEvent.RemoveAllListeners();
        PlayerMovement.OnStopVertigoEvent.RemoveAllListeners();
    }

    private void Update()
    {
        /*
        if (isWindBlowing)
        {
            // Appliquer une rotation sur l'axe Z (roll) selon la vitesse du vent
            float windRollAngle = cameraTiltAngle * windSpeed * windTiltMultiplier; // Calcul du roll en fonction de la vitesse du vent

            if (_windScript._windDirection == WindDirection.West || _windScript._windDirection == WindDirection.NorthWest || _windScript._windDirection == WindDirection.SouthWest)
            {
                windRollAngle = -windRollAngle;
            }

            // Inclure l'effet de roll dans la rotation de la caméra
            Quaternion windRotation = initialCameraRotation * Quaternion.Euler(0, 0, windRollAngle);

            // Calculer la translation latérale en fonction de la direction du vent
            float windTranslationX = 0f;  // Variable pour stocker la translation en X

            if (_windScript._windDirection == WindDirection.West || _windScript._windDirection == WindDirection.NorthWest || _windScript._windDirection == WindDirection.SouthWest)
            {
                windTranslationX = windTranslationMultiplier;  // Translation vers la gauche
            }
            else if (_windScript._windDirection == WindDirection.East || _windScript._windDirection == WindDirection.NorthEast || _windScript._windDirection == WindDirection.SouthEast)
            {
                windTranslationX = -windTranslationMultiplier;  // Translation vers la droite
            }

            // Appliquer le déplacement latéral en plus de la rotation
            Vector3 targetPosition = new Vector3(
                initialCameraPosition.x + windTranslationX, // Décalage latéral
                playerCamera.transform.localPosition.y,     // Garder la position Y actuelle
                playerCamera.transform.localPosition.z      // Garder la position Z actuelle
            );

            // Appliquer la rotation et la position de la caméra de manière fluide
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, windRotation, 0.1f);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, 0.1f);
        }
        else
        {
            // Remettre le roll et la translation latérale à zéro de manière fluide quand le vent ne souffle pas
            Quaternion resetRotation = initialCameraRotation; // Retour à la rotation initiale (sans inclinaison)
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, resetRotation, 0.1f);

            Vector3 resetPosition = new Vector3(
                initialCameraPosition.x, // Retour à la position initiale X
                playerCamera.transform.localPosition.y,  // Garder la position Y actuelle
                playerCamera.transform.localPosition.z   // Garder la position Z actuelle
            );

            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, resetPosition, 0.1f);
        }
        */
        if (PlayerMovement.instance.GetMoveSpeed() != 0 && GameManager.instance.GetIsPlayerAlive() && !WindScript.instance.GetIsWindBlowing())
        {
            ApplyMovementCamera();
        }
    }

    private void ApplyMovementCamera()
    {
        //Debug.Log("j'applique le mouvement de base");
        // Appliquer l'inclinaison � la cam�ra selon la vitesse
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle * (PlayerMovement.instance.GetMoveSpeed() / PlayerMovement.instance.GetMaxSpeed()), 0, 0);
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);


        // Calculer la quantit� de bobbing en fonction de la vitesse du joueur
        float speedFactor = Mathf.Clamp01(PlayerMovement.instance.GetMoveSpeed() / PlayerMovement.instance.GetMaxSpeed());
        float bobbingAmountX = baseBobbingAmountX * speedFactor; // Amplitude bas�e sur la vitesse
        float bobbingAmountY = baseBobbingAmountY * speedFactor; // Amplitude bas�e sur la vitesse

        // Appliquer le head bobbing
        ApplyHeadBobbing(bobbingAmountX, bobbingAmountY);

        // G�rer l'inclinaison de la cam�ra en fonction de l'input
        ApplyCameraTilt();
    }

    private void ApplyHeadBobbing(float bobbingAmountX, float bobbingAmountY)
    {
        //Debug.Log("j'applique le headBobbing");
        // Mettre � jour le timer
        timer += Time.deltaTime * bobbingSpeed;

        // Calculer les nouvelles positions bas�es sur le timer
        float offsetX = Mathf.Sin(timer) * bobbingAmountX; // Oscillation sur l'axe X
        float offsetY = Mathf.Sin(timer) * bobbingAmountY; // Oscillation sur l'axe Y

        // Appliquer le mouvement de bobbing � la position de la cam�ra en conservant la position initiale
        playerCamera.transform.localPosition = new Vector3(initialCameraPosition.x + offsetX, initialCameraPosition.y + offsetY, initialCameraPosition.z);
    }

    private void ApplyCameraTilt()
    {
        //Debug.Log("j'applique le caméraTilt");
        float tilt = 0f;
        float movementInput = PlayerMovement.instance.GetMovementInput();
        if (movementInput > 0) // Fl�che du haut maintenue
        {
            tilt = -tiltAngle; // Pencher � gauche
        }
        else if (movementInput < 0) // Fl�che du bas maintenue
        {
            tilt = tiltAngle;  // Pencher � droite
        }

        // Appliquer l'inclinaison sur l'axe Z
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(0, 0, tilt);
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime);
    }

    public void OnLose()
    {
        if (fallCoroutine != null) return;
        bool side = Random.value > 0.5f;
        fallCoroutine = StartCoroutine(DeathCameraFall(side));
    }

    private IEnumerator DeathCameraFall(bool side)
    {
        Debug.Log("j'applique le coroutine effet de mort");
        InstantStopVertigoEffects();
        float elapsedTime = 0f;

        // D�terminer la direction de la chute (gauche ou droite)
        float tiltDirection = side ? 90f : -90f; // 90 degr�s � gauche ou � droite

        // Ajuster la translation horizontale (X) en fonction de la direction de la chute
        float horizontalShift = side ? sideFall : -sideFall; // Tomber plus loin sur le c�t� (1.5 unit�s � gauche ou � droite)

        Quaternion startingRotation = playerCamera.transform.localRotation;
        Vector3 startingPosition = playerCamera.transform.localPosition;

        // Position et rotation finales apr�s la chute
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(0, 0, tiltDirection);

        // Ajouter un d�calage plus prononc� vers le bas (-3 unit�s sur Y) et sur le c�t� en fonction du param�tre 'side'
        Vector3 targetPosition = initialCameraPosition + new Vector3(horizontalShift, -fallHeight, 0);
        // Animer la chute
        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;

            // Lerp la rotation vers la cible
            playerCamera.transform.localRotation = Quaternion.Lerp(startingRotation, targetRotation, fallCurve.Evaluate(t));

            // Lerp la position vers la cible (tomber vers le bas et � gauche/droite)
            playerCamera.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, fallCurve.Evaluate(t));

            yield return null; // Attendre la prochaine frame
        }
        // Assurer que la cam�ra termine exactement dans sa position finale
        playerCamera.transform.localRotation = targetRotation;
        playerCamera.transform.localPosition = targetPosition;
        GameManager.OnRespawnEvent?.Invoke();
    }

    public void OnRespawn()
    {
        //StopAllCoroutines();
        InstantStopVertigoEffects();
        ResetCamera();
        fallCoroutine = null;
    }

    public void ResetCamera()
    {
        Debug.Log("j'applique le reset de camera");
        playerCamera.transform.localPosition = initialCameraPosition;
        playerCamera.transform.localRotation = initialCameraRotation;
    }

    public void OnStartVertigo()
    {
        StopAndSetNullCoroutine(stopVertigoCoroutine);
        startVertigoCoroutine = StartCoroutine(ApplyVertigoEffect());
    }

    private IEnumerator ApplyVertigoEffect()
    {
        Debug.Log("j'applique le vertige");
        // Activer les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = true;
        if (chromaticAberration != null)
            chromaticAberration.active = true;
        if (lensDistorsion != null)
            lensDistorsion.active = true;
        if (vignette != null)
            vignette.active = true;

        float elapsedTime = 0f; // Variable pour le temps écoulé

        Quaternion startingRotation = playerCamera.transform.localRotation;
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAmount, 0, 0);

        // Appliquer les effets progressivement
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Appliquer les effets avec lerp
            motionBlur.intensity.value = Mathf.Lerp(0, maxBlurAmount, lerpFactor);
            chromaticAberration.intensity.value = Mathf.Lerp(0, maxAberrationAmount, lerpFactor);
            lensDistorsion.intensity.value = Mathf.Lerp(0, maxLensDistorsionIntensity, lerpFactor);
            vignette.intensity.value = Mathf.Lerp(0, maxVignetteIntensity, lerpFactor);
            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, initialFOV - maxFOVChange, lerpFactor);

            // Incliner la caméra vers le bas
            playerCamera.transform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, lerpFactor);

            // Ajouter un effet de tremblement de la caméra
            playerCamera.transform.localPosition += Random.insideUnitSphere * Mathf.Lerp(0, cameraShakeIntensity, lerpFactor); ;

            yield return null; // Attendre une frame
        }
        // Continuer à appliquer le tremblement tant que l'effet est actif
        while (true)
        {
            playerCamera.transform.localPosition += Random.insideUnitSphere * cameraShakeIntensity;
            yield return null;
        }
    }

    public void OnStopVertigo()
    {
        StopAndSetNullCoroutine(startVertigoCoroutine);
        stopVertigoCoroutine = StartCoroutine(ResetVertigoEffects());
    }

    private IEnumerator ResetVertigoEffects()
    {
        Debug.Log("Je Reset le Vertige");
        float elapsedTime = 0f; // Variable pour le temps écoulé

        // Transition progressive de retour
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Ramener les effets progressivement à zéro
            if (motionBlur != null)
                motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, 0, lerpFactor);
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0, lerpFactor);
            if (lensDistorsion != null)
                lensDistorsion.intensity.value = Mathf.Lerp(lensDistorsion.intensity.value, 0, lerpFactor);
            if (vignette != null)
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0, lerpFactor);

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initialFOV, lerpFactor);

            // Réinitialiser l'inclinaison de la caméra
            playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, initialCameraRotation, lerpFactor);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraPosition, lerpFactor);

            yield return null; // Attendre une frame
        }

        // Désactiver les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = false;
        if (chromaticAberration != null)
            chromaticAberration.active = false;
        if (lensDistorsion != null)
            lensDistorsion.active = false;
        if (vignette != null)
            vignette.active = false;

        // Réinitialiser le FOV et la rotation de la caméra
        playerCamera.fieldOfView = initialFOV;
        playerCamera.transform.localRotation = initialCameraRotation;
        playerCamera.transform.localPosition = initialCameraPosition;

        stopVertigoCoroutine = null;
    }

    public void InstantStopVertigoEffects()
    {
        StopAndSetNullCoroutine(startVertigoCoroutine);
        StopAndSetNullCoroutine(stopVertigoCoroutine);
        // Désactiver les effets de post-processing
        if (motionBlur != null)
            motionBlur.active = false;
        if (chromaticAberration != null)
            chromaticAberration.active = false;
        if (lensDistorsion != null)
            lensDistorsion.active = false;
        if (vignette != null)
            vignette.active = false;
    }

    private void StopAndSetNullCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}