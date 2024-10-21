using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;             // R�f�rence � la cam�ra du joueur

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;       // Vitesse du balancement
    [SerializeField] private float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement lat�ral (gauche-droite)
    [SerializeField] private float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    [SerializeField] private float tiltAngle = 5f;            // Angle de rotation pour pencher la cam�ra lat�ralement

    private PlayerMovement playerMovement;  // R�f�rence au script PlayerMovement
    private float timer = 0.0f;             // Timer pour l'oscillation
    private Vector3 initialCameraPosition;  // Stocker la position initiale de la cam�ra
    private Quaternion initialCameraRotation; // Stocker la rotation initiale de la cam�ra

    private void Start()
    {
        // Obtenir la r�f�rence au script PlayerMovement
        playerMovement = GetComponent<PlayerMovement>();

        // Stocker la position initiale et la rotation initiale de la cam�ra
        initialCameraPosition = playerCamera.transform.localPosition;
        initialCameraRotation = playerCamera.transform.localRotation;
    }

    private void Update()
    {
        if (playerMovement != null)
        {
            // Calculer la quantit� de bobbing en fonction de la vitesse du joueur
            float speedFactor = Mathf.Clamp01(playerMovement.GetMoveSpeed() / playerMovement.GetMaxSpeed());
            float bobbingAmountX = baseBobbingAmountX * speedFactor; // Amplitude bas�e sur la vitesse
            float bobbingAmountY = baseBobbingAmountY * speedFactor; // Amplitude bas�e sur la vitesse

            // Appliquer le head bobbing
            ApplyHeadBobbing(bobbingAmountX, bobbingAmountY);

            // G�rer l'inclinaison de la cam�ra en fonction de l'input
            ApplyCameraTilt();
        }
    }

    private void ApplyHeadBobbing(float bobbingAmountX, float bobbingAmountY)
    {
        // Mettre � jour le timer
        timer += Time.deltaTime * bobbingSpeed;

        // Calculer les nouvelles positions bas�es sur le timer
        float offsetX = Mathf.Sin(timer) * bobbingAmountX; // Oscillation sur l'axe X
        float offsetY = Mathf.Sin(timer) * bobbingAmountY; // Oscillation sur l'axe Y

        // Appliquer le mouvement de bobbing � la position de la cam�ra en conservant la position initiale
        playerCamera.transform.localPosition = new Vector3(
            initialCameraPosition.x + offsetX,  // Ajout du d�calage X � la position initiale X
            initialCameraPosition.y + offsetY,  // Ajout du d�calage Y � la position initiale Y
            initialCameraPosition.z             // La position Z reste la m�me
        );
    }

    private void ApplyCameraTilt()
    {
        // Incliner la cam�ra en fonction de l'input du joueur
        if (playerMovement != null)
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
    }
    
}
