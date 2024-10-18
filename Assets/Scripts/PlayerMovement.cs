using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;              // Vitesse actuelle du joueur
    public float maxSpeed;               // Vitesse maximale
    public float acceleration;           // Taux d'accélération
    public Camera playerCamera;          // Référence à la caméra du joueur
    public float cameraTiltAngle;        // Angle de rotation de la caméra
    public float movementInput;         // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls;     // Instance des contrôles
    private Quaternion initialCameraRotation;  // Stocker la rotation initiale de la caméra

    // Variables pour la gestion du contrôle par alternance
    private float timePressingSameKey = 0f;  // Temps passé à maintenir la même touche
    public float maxPressTime = 1f;          // Temps maximal avant de perdre de la vitesse si on maintient la même touche

    private bool lastKeyWasUp = true;        // Savoir si la dernière touche était la flèche du haut (initialisé à "haut" pour le premier appui)

    private void Awake()
    {
        // Initialisation des contrôles
        controls = new PlayerControls();

        // Lier l'action Move à une méthode pour capturer la valeur d'entrée
        controls.Player.Move.performed += ctx => OnMove(ctx.ReadValue<float>());
        controls.Player.Move.canceled += ctx => OnStopMove();

        // Stocker la rotation initiale de la caméra
        initialCameraRotation = playerCamera.transform.localRotation;
    }

    private void OnEnable()
    {
        // Activer les contrôles
        controls.Enable();
    }

    private void OnDisable()
    {
        // Désactiver les contrôles
        controls.Disable();
    }

    private void Update()
    {
        if (movementInput != 0)
        {
            // Vérifie si on a appuyé sur la même touche trop longtemps
            timePressingSameKey += Time.deltaTime;
            if (timePressingSameKey > maxPressTime)
            {
                // Si on dépasse le temps limite, la vitesse redescend à 0
                moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime * 2); // Perte de vitesse
                Debug.Log("Maintien trop long sur la même touche : vitesse réduite");
            }
            else
            {
                // Sinon, on augmente la vitesse progressivement
                moveSpeed += acceleration * Time.deltaTime;
                moveSpeed = Mathf.Min(maxSpeed, moveSpeed); // Limiter à la vitesse max
            }
        }
        else
        {
            // Si aucune touche n'est pressée, la vitesse redescend lentement
            moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime);
        }

        // Appliquer l'inclinaison à la caméra selon la vitesse
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle * (moveSpeed / maxSpeed), 0, 0);
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);

        // Appliquer le mouvement du joueur en fonction de la vitesse
        Vector3 move = new Vector3(0, 0, moveSpeed) * Time.deltaTime;
        transform.Translate(move);
    }

    // Gestion de l'entrée de mouvement
    private void OnMove(float input)
    {
        if (input > 0 && lastKeyWasUp == false) // Si on appuie sur flèche haut après flèche bas
        {
            lastKeyWasUp = true;
            timePressingSameKey = 0f; // Réinitialiser le temps passé sur la touche
        }
        else if (input < 0 && lastKeyWasUp == true) // Si on appuie sur flèche bas après flèche haut
        {
            lastKeyWasUp = false;
            timePressingSameKey = 0f; // Réinitialiser le temps passé sur la touche
        }

        movementInput = input; // Stocker la direction de l'input
    }

    // Quand on arrête de bouger (lorsqu'aucune touche n'est appuyée)
    private void OnStopMove()
    {
        movementInput = 0f; // Arrêter le mouvement
    }
}
