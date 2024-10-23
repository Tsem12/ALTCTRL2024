using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;             // Référence à la caméra du joueur

    [Header("MovementTilt")]
    [SerializeField] private float cameraTiltAngle = 5f;            // Angle de rotation pour pencher la caméra latéralement

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;       // Vitesse du balancement
    [SerializeField] private float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement latéral (gauche-droite)
    [SerializeField] private float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    [SerializeField] private float tiltAngle = 5f;            // Angle de rotation pour pencher la caméra latéralement

    [Header("Fall")]
    [SerializeField] private float fallHeight;
    [SerializeField] private float sideFall;
    [SerializeField] private float fallDuration;

    private PlayerMovement playerMovement;  // Référence au script PlayerMovement
    private float timer = 0.0f;             // Timer pour l'oscillation
    private Vector3 initialCameraPosition;  // Stocker la position initiale de la caméra
    private Quaternion initialCameraRotation; // Stocker la rotation initiale de la caméra

    private void Start()
    {
        // Obtenir la référence au script PlayerMovement
        playerMovement = GetComponent<PlayerMovement>();

        // Stocker la position initiale et la rotation initiale de la caméra
        initialCameraPosition = playerCamera.transform.localPosition;
        initialCameraRotation = playerCamera.transform.localRotation;
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.GetMoveSpeed() != 0)
        {
            // Appliquer l'inclinaison à la caméra selon la vitesse
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle * (playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed()), 0, 0);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);


            // Calculer la quantité de bobbing en fonction de la vitesse du joueur
            float speedFactor = Mathf.Clamp01(playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed());
            float bobbingAmountX = baseBobbingAmountX * speedFactor; // Amplitude basée sur la vitesse
            float bobbingAmountY = baseBobbingAmountY * speedFactor; // Amplitude basée sur la vitesse

            // Appliquer le head bobbing
            ApplyHeadBobbing(bobbingAmountX, bobbingAmountY);

            // Gérer l'inclinaison de la caméra en fonction de l'input
            ApplyCameraTilt();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyDeathCameraEffect(true);
        }
    }

    private void ApplyHeadBobbing(float bobbingAmountX, float bobbingAmountY)
    {
        // Mettre à jour le timer
        timer += Time.deltaTime * bobbingSpeed;

        // Calculer les nouvelles positions basées sur le timer
        float offsetX = Mathf.Sin(timer) * bobbingAmountX; // Oscillation sur l'axe X
        float offsetY = Mathf.Sin(timer) * bobbingAmountY; // Oscillation sur l'axe Y

        // Appliquer le mouvement de bobbing à la position de la caméra en conservant la position initiale
        playerCamera.transform.localPosition = new Vector3(
            initialCameraPosition.x + offsetX,  // Ajout du décalage X à la position initiale X
            initialCameraPosition.y + offsetY,  // Ajout du décalage Y à la position initiale Y
            initialCameraPosition.z             // La position Z reste la même
        );
    }

    private void ApplyCameraTilt()
    {
        // Incliner la caméra en fonction de l'input du joueur
        if (playerMovement != null)
        {
            float tilt = 0f;
            float movementInput = playerMovement.GetMovementInput();
            if (movementInput > 0) // Flèche du haut maintenue
            {
                tilt = -tiltAngle; // Pencher à gauche
            }
            else if (movementInput < 0) // Flèche du bas maintenue
            {
                tilt = tiltAngle;  // Pencher à droite
            }

            // Appliquer l'inclinaison sur l'axe Z
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(0, 0, tilt);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime);
        }
    }

    //bool = gauche/droite
    public void ApplyDeathCameraEffect(bool side)
    {
        // Commence une coroutine pour animer la chute de la caméra
        StartCoroutine(DeathCameraFall(side));
    }

    private IEnumerator DeathCameraFall(bool side)
    {
        float elapsedTime = 0f;

        // Déterminer la direction de la chute (gauche ou droite)
        float tiltDirection = side ? 90f : -90f; // 90 degrés à gauche ou à droite

        // Ajuster la translation horizontale (X) en fonction de la direction de la chute
        float horizontalShift = side ? sideFall : -sideFall; // Tomber plus loin sur le côté (1.5 unités à gauche ou à droite)

        // Position et rotation finales après la chute
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(0, 0, tiltDirection);

        // Ajouter un décalage plus prononcé vers le bas (-3 unités sur Y) et sur le côté en fonction du paramètre 'side'
        Vector3 targetPosition = initialCameraPosition + new Vector3(horizontalShift, -fallHeight, 0);

        // Animer la chute
        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fallDuration;

            // Lerp la rotation vers la cible
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, t);

            // Lerp la position vers la cible (tomber vers le bas et à gauche/droite)
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, t);

            yield return null; // Attendre la prochaine frame
        }

        // Assurer que la caméra termine exactement dans sa position finale
        playerCamera.transform.localRotation = targetRotation;
        playerCamera.transform.localPosition = targetPosition;
    }


    public void ResetCamera()
    {
        playerCamera.transform.localPosition = initialCameraPosition;
        playerCamera.transform.localRotation = initialCameraRotation;
    }
}
