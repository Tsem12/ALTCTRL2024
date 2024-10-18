using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSave : MonoBehaviour
{
    public float moveSpeed;          // Vitesse actuelle du joueur
    public float maxSpeed;           // Vitesse maximale
    public float acceleration;       // Taux d'acc�l�ration
    public Camera playerCamera;      // R�f�rence � la cam�ra du joueur
    public float cameraTiltAngle;    // Angle de rotation de la cam�ra
    private float movementInput;     // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls; // Instance des contr�les
    private Quaternion initialCameraRotation;  // Stocker la rotation initiale de la cam�ra

    // Variables pour le head bobbing
    public float bobbingSpeed = 0.1f;        // Vitesse du balancement lat�ral
    public float baseBobbingAmountX = 0.02f; // Amplitude de base du balancement lat�ral (gauche-droite)
    public float baseBobbingAmountY = 0.01f; // Amplitude de base du balancement vertical (haut-bas)
    private float timer = 0.0f;              // Timer pour l'oscillation
    private float midpointX = 0f;            // Position de base de la cam�ra sur l'axe X
    private float midpointY = 0f;            // Position de base de la cam�ra sur l'axe Y

    private void Awake()
    {
        // Initialisation des contr�les
        controls = new PlayerControls();

        // Lier l'action Move � une m�thode pour capturer la valeur d'entr�e
        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => movementInput = 0f;  // Arr�ter le mouvement quand la touche est rel�ch�e

        // Stocker la rotation initiale de la cam�ra
        initialCameraRotation = playerCamera.transform.localRotation;

        // Stocker les positions de d�part de la cam�ra
        midpointX = playerCamera.transform.localPosition.x; // Position initiale pour le balancement lat�ral
        midpointY = playerCamera.transform.localPosition.y; // Position initiale pour le balancement vertical
    }

    private void OnEnable()
    {
        // Activer les contr�les
        controls.Enable();
    }

    private void OnDisable()
    {
        // D�sactiver les contr�les
        controls.Disable();
    }

    private void Update()
    {
        Debug.Log("movementInput : " + movementInput);
        // Gestion de l'input pour avancer ou reculer
        if (movementInput > 0) // Si on appuie sur la fl�che du haut (avancer)
        {
            Debug.Log("toto");
            // Augmenter progressivement la vitesse
            moveSpeed += acceleration * Time.deltaTime;
            if (moveSpeed >= maxSpeed)  // V�rifier si on atteint la vitesse maximale
            {
                moveSpeed = maxSpeed;   // Limiter � la vitesse max
                Debug.Log("Vitesse maximale atteinte !");
            }

            // Appliquer l'inclinaison � la rotation initiale de la cam�ra
            Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle, 0, 0);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);
        }
        else // Si on appuie sur la fl�che du bas (reculer ou stopper)
        {
            // Arr�ter le joueur
            moveSpeed = 0f;

            // Revenir � la rotation initiale de la cam�ra
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, initialCameraRotation, 0.1f);
        }

        // Calculer le mouvement du joueur (uniquement vers l'avant)
        Vector3 move = new Vector3(0, 0, moveSpeed) * Time.deltaTime;
        transform.Translate(move);

        // ---- Head Bobbing Lat�ral et Vertical pour funambule ----
        if (movementInput != 0) // Si le joueur avance
        {
            timer += bobbingSpeed * (moveSpeed / maxSpeed); // Ajuster la vitesse du balancement selon la vitesse du joueur
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }

            // Calculer le balancement lat�ral (gauche-droite) en fonction de l'oscillation
            float bobbingAmountX = baseBobbingAmountX * (moveSpeed / maxSpeed);
            float wavesliceX = Mathf.Sin(timer);
            float translateChangeX = wavesliceX * bobbingAmountX;

            // Calculer le balancement vertical (haut-bas) en fonction de l'oscillation
            float bobbingAmountY = baseBobbingAmountY * (moveSpeed / maxSpeed);
            float wavesliceY = Mathf.Cos(timer); // Utilisation de Cos pour un d�phasage par rapport � X
            float translateChangeY = wavesliceY * bobbingAmountY;

            // Appliquer les mouvements lat�ral et vertical � la cam�ra
            Vector3 localPosition = playerCamera.transform.localPosition;
            localPosition.x = midpointX + translateChangeX; // D�placement sur l'axe X pour simuler le balancement lat�ral
            localPosition.y = midpointY + translateChangeY; // D�placement sur l'axe Y pour simuler un l�ger head bobbing vertical
            playerCamera.transform.localPosition = localPosition;
        }
        else
        {
            // Si le joueur ne bouge pas, on r�initialise la position de la cam�ra
            Vector3 localPosition = playerCamera.transform.localPosition;
            localPosition.x = midpointX;
            localPosition.y = midpointY;
            playerCamera.transform.localPosition = localPosition;
        }
    }
}
