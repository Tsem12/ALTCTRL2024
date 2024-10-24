using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;             // R�f�rence � la cam�ra du joueur
    [SerializeField] private URPCameraVisualEffects effects;

    // New Input System
    private PlayerControls controls;

    [Header("MovementTilt")]
    [SerializeField] private float cameraTiltAngle = 5f;            // Angle de rotation pour pencher la cam�ra lat�ralement

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;       // Vitesse du balancement
    [SerializeField] private float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement lat�ral (gauche-droite)
    [SerializeField] private float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    [SerializeField] private float tiltAngle = 5f;            // Angle de rotation pour pencher la cam�ra lat�ralement

    [Header("Fall")]
    [SerializeField] private float fallHeight;
    [SerializeField] private float sideFall;
    [SerializeField] private float fallDuration;

    private Coroutine fallCoroutine = null;

    private PlayerMovement playerMovement;  // R�f�rence au script PlayerMovement
    private float timer = 0.0f;             // Timer pour l'oscillation
    private Vector3 initialCameraPosition;   // Stocker la position initiale de la cam�ra
    private Quaternion initialCameraRotation; // Stocker la rotation initiale de la cam�ra

    [Header("Wind Settings")]
    [SerializeField] private WindScript _windScript;
    [SerializeField] private float windSpeed = 10f;
    [SerializeField] private float windTiltMultiplier = 1f;
    [SerializeField] private float windTranslationMultiplier = 0.1f;

    public static UnityEvent OnDroneEvent = new UnityEvent();

    private bool isAlive = true;
    private bool isWindBlowing = false;
    private bool isJumping = false;

    private void Awake()
    {
        // Initialisation des Input Actions
        controls = new PlayerControls();
    }

    private void Start()
    {
        // Obtenir la r�f�rence au script PlayerMovement
        playerMovement = GetComponent<PlayerMovement>();
        // Stocker la position initiale et la rotation initiale de la cam�ra
        initialCameraPosition = playerCamera.transform.localPosition;
        initialCameraRotation = playerCamera.transform.localRotation;
    }

    private void OnEnable()
    {
        // Activer les actions du joueur quand l'objet est activé
        controls.Player.Enable();

        // Assigner l'action "Jump" à une méthode de callback
        controls.Player.Jump.performed += OnJumpPerformed;
        OnDroneEvent.AddListener(OnDroneEventFct);
    }

    private void OnDisable()
    {
        // Désactiver les actions du joueur quand l'objet est désactivé
        controls.Player.Disable();

        // Désabonner la méthode de callback pour éviter les erreurs
        controls.Player.Jump.performed -= OnJumpPerformed;
        OnDroneEvent.RemoveAllListeners();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        StartCoroutine(JumpCoroutine());
    }

    private void Update()
    {
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
        if (playerMovement.GetMoveSpeed() != 0 && isAlive && !isWindBlowing)
        {
            ApplyMovementCamera();
        }
    }

    private void ApplyMovementCamera()
    {
        // Appliquer l'inclinaison � la cam�ra selon la vitesse
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle * (playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed()), 0, 0);
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);


        // Calculer la quantit� de bobbing en fonction de la vitesse du joueur
        float speedFactor = Mathf.Clamp01(playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed());
        float bobbingAmountX = baseBobbingAmountX * speedFactor; // Amplitude bas�e sur la vitesse
        float bobbingAmountY = baseBobbingAmountY * speedFactor; // Amplitude bas�e sur la vitesse

        // Appliquer le head bobbing
        ApplyHeadBobbing(bobbingAmountX, bobbingAmountY);

        // G�rer l'inclinaison de la cam�ra en fonction de l'input
        ApplyCameraTilt();
    }

    private void ApplyHeadBobbing(float bobbingAmountX, float bobbingAmountY)
    {
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
        float tilt = 0f;
        float movementInput = playerMovement.GetMovementInput();
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

    public void ApplyDeathCameraEffect(bool side)
    {
        if (fallCoroutine != null) return;
        isAlive = false;
        // Commence une coroutine pour animer la chute de la cam�ra
        fallCoroutine = StartCoroutine(DeathCameraFall(side));
    }

    private IEnumerator DeathCameraFall(bool side)
    {
        effects.InstantStopVertigoEffects();
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
            playerCamera.transform.localRotation = Quaternion.Lerp(startingRotation, targetRotation, t);

            // Lerp la position vers la cible (tomber vers le bas et � gauche/droite)
            playerCamera.transform.localPosition = Vector3.Lerp(startingPosition, targetPosition, t);

            yield return null; // Attendre la prochaine frame
        }

        // Assurer que la cam�ra termine exactement dans sa position finale
        playerCamera.transform.localRotation = targetRotation;
        playerCamera.transform.localPosition = targetPosition;
        GameManager.instance.SetHasMoved(false);
        //Reset la Camera au spawn
        ResetCamera();
        GameManager.instance.SetIsPlayerAlive(true);
        isAlive = true;
        fallCoroutine = null;
    }

    public void ApplyWindMovement()
    {
        isWindBlowing = true;
    }

    public void ResetWindMovement()
    {
        isWindBlowing = false;
    }

    public void ResetCamera()
    {
        playerCamera.transform.localPosition = initialCameraPosition;
        playerCamera.transform.localRotation = initialCameraRotation;
    }

    private IEnumerator JumpCoroutine()
    {
        isJumping = true;
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    public void OnDroneEventFct()
    {
        if (!isJumping)
        {
            GameManager.instance.OnLoseEvent?.Invoke();
        }
    }
}
