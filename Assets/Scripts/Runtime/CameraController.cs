using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using DG.Tweening.Core;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Camera playerCamera;

    [Header("MovementTilt")]
    [SerializeField] private float cameraTiltAngle;

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed;
    [SerializeField] private float baseBobbingAmountX;
    [SerializeField] private float baseBobbingAmountY;
    [SerializeField] private float tiltAngle;
    private float timer;

    [Header("Fall")]
    [SerializeField] private float fallHeight;
    [SerializeField] private float sideFall;
    [SerializeField] private float fallRotationXAngle;
    [SerializeField] private float fallDuration;
    [SerializeField] private AnimationCurve fallCurve;

    [Header("Wind Settings")]
    [SerializeField] private float _windRollAngle;
    [SerializeField] private float _windLateralTranslation;

    [Header("VertigoEffect")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private float transitionDuration;
    [SerializeField] private float cameraTiltAmount;
    [SerializeField] private float cameraShakeIntensity;
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
    private Coroutine startVertigoCoroutine = null;
    private Coroutine stopVertigoCoroutine = null;
    private Coroutine startWindCoroutine = null;
    private Coroutine stopWindCoroutine = null;

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
        WindScript.OnWindBlowingEvent.AddListener(OnStartWind);
        WindScript.OnWindResetEvent.AddListener(OnResetWind);
    }

    private void OnDisable()
    {
        GameManager.OnRespawnEvent.RemoveAllListeners();
        GameManager.OnLoseEvent.RemoveAllListeners();
        PlayerMovement.OnStartVertigoEvent.RemoveAllListeners();
        PlayerMovement.OnStopVertigoEvent.RemoveAllListeners();
        WindScript.OnWindBlowingEvent.RemoveAllListeners();
        WindScript.OnWindResetEvent.RemoveAllListeners();
    }

    private void Update()
    {
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
        WindScript.OnWindStopBlowingEvent.Invoke();
        StopAndSetNullCoroutine(startWindCoroutine);
        StopAndSetNullCoroutine(stopWindCoroutine);
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
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(fallRotationXAngle, 0, tiltDirection);

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
        GameManager.OnLoseEvent.Invoke();
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

    public void OnStartWind(float windSpeed)
    {
        Debug.Log("J'applique l'effet de vent");
        StopAndSetNullCoroutine(startVertigoCoroutine);
        StopAndSetNullCoroutine(stopWindCoroutine);
        startWindCoroutine = StartCoroutine(ApplyWindEffect(windSpeed));
    }

    private IEnumerator ApplyWindEffect(float windSpeed)
    {
        Direction windDirection = WindScript.instance._windDirection;
        float windRollAngle = _windRollAngle;
        float windLateralTranslation = _windLateralTranslation;

        if (windDirection == Direction.West)
            windRollAngle = -windRollAngle;
        else
            windLateralTranslation = -windLateralTranslation;

        Quaternion startingRotation = playerCamera.transform.localRotation;
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(0, 0, windRollAngle);

        Vector3 startingPosition = playerCamera.transform.localPosition;
        Vector3 targetPosition = initialCameraPosition + new Vector3(windLateralTranslation, 0, 0);

        // Appliquer les effets progressivement
        float maxTime = Vector3.Distance(startingPosition, targetPosition) / windSpeed;
        float elapsedTime = 0f;
        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / maxTime);

            // Incliner la caméra
            playerCamera.transform.localRotation = Quaternion.Lerp(startingRotation, targetRotation, lerpFactor);
            playerCamera.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, lerpFactor);

            yield return null; // Attendre une frame
        }
        GameManager.OnLoseEvent.Invoke();
        startWindCoroutine = null;
    }

    public void OnResetWind(float windSpeed)
    {
        Debug.Log("Je reset l'effet de vent");
        StopAndSetNullCoroutine(startWindCoroutine);
        stopWindCoroutine = StartCoroutine(ResetWindEffect(windSpeed));
    }

    private IEnumerator ResetWindEffect(float windSpeed)
    {
        Quaternion startingRotation = playerCamera.transform.localRotation;
        Quaternion targetRotation = initialCameraRotation;

        Vector3 startingPosition = playerCamera.transform.localPosition;
        Vector3 targetPosition = initialCameraPosition;

        // Appliquer les effets progressivement
        float maxTime = Vector3.Distance(startingPosition, targetPosition) / windSpeed;
        float elapsedTime = 0f;
        while (elapsedTime < maxTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / maxTime);

            // Incliner la caméra
            playerCamera.transform.localRotation = Quaternion.Lerp(startingRotation, targetRotation, lerpFactor);
            playerCamera.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, lerpFactor);

            yield return null; // Attendre une frame
        }
        WindScript.OnWindStopBlowingEvent.Invoke();
        stopWindCoroutine = null;
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