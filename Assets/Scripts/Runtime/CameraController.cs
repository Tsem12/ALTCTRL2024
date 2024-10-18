using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;             // Référence à la caméra du joueur

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;       // Vitesse du balancement
    [SerializeField] private float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement latéral (gauche-droite)
    [SerializeField] private float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    [SerializeField] private float tiltAngle = 5f;            // Angle de rotation pour pencher la caméra latéralement

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
        if (playerMovement != null)
        {
            // Calculer la quantité de bobbing en fonction de la vitesse du joueur
            float speedFactor = Mathf.Clamp01(playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed());
            float bobbingAmountX = baseBobbingAmountX * speedFactor; // Amplitude basée sur la vitesse
            float bobbingAmountY = baseBobbingAmountY * speedFactor; // Amplitude basée sur la vitesse

            // Appliquer le head bobbing
            ApplyHeadBobbing(bobbingAmountX, bobbingAmountY);

            // Gérer l'inclinaison de la caméra en fonction de l'input
            ApplyCameraTilt();
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
}
