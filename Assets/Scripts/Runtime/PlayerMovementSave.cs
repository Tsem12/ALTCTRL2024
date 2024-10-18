using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSave : MonoBehaviour
{
    public float moveSpeed;          // Vitesse actuelle du joueur
    public float maxSpeed;           // Vitesse maximale
    public float acceleration;       // Taux d'accélération
    public Camera playerCamera;      // Référence à la caméra du joueur
    public float cameraTiltAngle;    // Angle de rotation de la caméra
    private float movementInput;     // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls; // Instance des contrôles
    private Quaternion initialCameraRotation;  // Stocker la rotation initiale de la caméra

    // Variables pour le head bobbing
    public float bobbingSpeed = 0.1f;        // Vitesse du balancement latéral
    public float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement latéral (gauche-droite)
    public float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    private float timer = 0.0f;              // Timer pour l'oscillation
    private float midpointX = 0f;            // Position de base de la caméra sur l'axe X
    private float midpointY = 0f;            // Position de base de la caméra sur l'axe Y

    private void Awake()
    {
        // Initialisation des contrôles
        controls = new PlayerControls();

        // Lier l'action Move à une méthode pour capturer la valeur d'entrée
        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => movementInput = 0f;  // Arrêter le mouvement quand la touche est relâchée

        // Stocker la rotation initiale de la caméra
        initialCameraRotation = playerCamera.transform.localRotation;

        // Stocker les positions de départ de la caméra
        midpointX = playerCamera.transform.localPosition.x; // Position initiale pour le balancement latéral
        midpointY = playerCamera.transform.localPosition.y; // Position initiale pour le balancement vertical
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
        Debug.Log("movementInput : " + movementInput);
        // Gestion de l'input pour avancer ou reculer
        if (movementInput > 0) // Si on appuie sur la flèche du haut (avancer)
        {
            Debug.Log("toto");
            // Augmenter progressivement la vitesse
            moveSpeed += acceleration * Time.deltaTime;
            if (moveSpeed >= maxSpeed)  // Vérifier si on atteint la vitesse maximale
            {
                moveSpeed = maxSpeed;   // Limiter à la vitesse max
                Debug.Log("Vitesse maximale atteinte !");
            }

            // Appliquer l'inclinaison à la rotation initiale de la caméra
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);
        }
        else // Si on appuie sur la flèche du bas (reculer ou stopper)
        {
            // Arrêter le joueur
            moveSpeed = 0f;

            // Revenir à la rotation initiale de la caméra
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, initialCameraRotation, 0.1f);
        }

        // Calculer le mouvement du joueur (uniquement vers l'avant)
        Vector3 move = new Vector3(0, 0, moveSpeed) * Time.deltaTime;
        transform.Translate(move);

        // ---- Head Bobbing Latéral et Vertical pour funambule ----
        if (movementInput != 0) // Si le joueur avance
        {
            timer += bobbingSpeed * (moveSpeed / maxSpeed); // Ajuster la vitesse du balancement selon la vitesse du joueur
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }

            // Calculer le balancement latéral (gauche-droite) en fonction de l'oscillation
            float bobbingAmountX = baseBobbingAmountX * (moveSpeed / maxSpeed);
            float wavesliceX = Mathf.Sin(timer);
            float translateChangeX = wavesliceX * bobbingAmountX;

            // Calculer le balancement vertical (haut-bas) en fonction de l'oscillation
            float bobbingAmountY = baseBobbingAmountY * (moveSpeed / maxSpeed);
            float wavesliceY = Mathf.Cos(timer); // Utilisation de Cos pour un déphasage par rapport à X
            float translateChangeY = wavesliceY * bobbingAmountY;

            // Appliquer les mouvements latéral et vertical à la caméra
            Vector3 localPosition = playerCamera.transform.localPosition;
            localPosition.x = midpointX + translateChangeX; // Déplacement sur l'axe X pour simuler le balancement latéral
            localPosition.y = midpointY + translateChangeY; // Déplacement sur l'axe Y pour simuler un léger head bobbing vertical
            playerCamera.transform.localPosition = localPosition;
        }
        else
        {
            // Si le joueur ne bouge pas, on réinitialise la position de la caméra
            Vector3 localPosition = playerCamera.transform.localPosition;
            localPosition.x = midpointX;
            localPosition.y = midpointY;
            playerCamera.transform.localPosition = localPosition;
        }
    }
}
