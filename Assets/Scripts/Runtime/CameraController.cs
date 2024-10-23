using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;             // R�f�rence � la cam�ra du joueur

    [Header("MovementTilt")]
    [SerializeField] private float cameraTiltAngle = 5f;            // Angle de rotation pour pencher la cam�ra lat�ralement

    [Header("Bobbing")]
    [SerializeField] private float bobbingSpeed = 0.1f;       // Vitesse du balancement
    [SerializeField] private float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement lat�ral (gauche-droite)
    [SerializeField] private float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    [SerializeField] private float tiltAngle = 5f;            // Angle de rotation pour pencher la cam�ra lat�ralement

    private PlayerMovement playerMovement;  // R�f�rence au script PlayerMovement
    private float timer = 0.0f;             // Timer pour l'oscillation
    private Vector3 initialCameraPosition;   // Stocker la position initiale de la cam�ra
    private Quaternion initialCameraRotation; // Stocker la rotation initiale de la cam�ra

    [Header("Wind Settings")]
    [SerializeField] private float windSpeed = 10f;        
    [SerializeField] private float windTiltMultiplier = 1f;

    [SerializeField] private WindScript _windScript;

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
        if (playerMovement != null && playerMovement.GetMoveSpeed() != 0)
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

        if (_windScript != null && _windScript.isWindBlowing)
        {
            // Appliquer une rotation sur l'axe Z (roll) selon la vitesse du vent
            float windRollAngle = cameraTiltAngle * windSpeed * windTiltMultiplier; // Calcul du roll en fonction de la vitesse du vent

            if(_windScript._windDirection == WindDirection.West || _windScript._windDirection == WindDirection.NorthWest || _windScript._windDirection == WindDirection.SouthWest)
            {
                windRollAngle = -windRollAngle;
            }

            // Inclure l'effet de roll dans la rotation de la cam�ra
            Quaternion windRotation = initialCameraRotation * Quaternion.Euler(0, 0, windRollAngle);

            // Appliquer la rotation � la cam�ra (roll + rotation d�j� d�finie par la vitesse du joueur)
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, windRotation, 0.1f);
        }
        else
        {
            // Remettre le roll � z�ro de mani�re fluide quand le vent ne souffle pas
            Quaternion resetRotation = initialCameraRotation; // Retour � la rotation initiale (sans inclinaison)
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, resetRotation, 0.1f);
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

    public void ResetCamera()
    {
        playerCamera.transform.localPosition = initialCameraPosition;
        playerCamera.transform.localRotation = initialCameraRotation;
    }
}
