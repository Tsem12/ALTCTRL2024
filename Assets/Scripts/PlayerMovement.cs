using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;              // Vitesse actuelle du joueur
    public float maxSpeed;               // Vitesse maximale
    public float acceleration;           // Taux d'acc�l�ration
    public Camera playerCamera;          // R�f�rence � la cam�ra du joueur
    public float cameraTiltAngle;        // Angle de rotation de la cam�ra
    public float movementInput;         // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls;     // Instance des contr�les
    private Quaternion initialCameraRotation;  // Stocker la rotation initiale de la cam�ra

    // Variables pour la gestion du contr�le par alternance
    private float timePressingSameKey = 0f;  // Temps pass� � maintenir la m�me touche
    public float maxPressTime = 1f;          // Temps maximal avant de perdre de la vitesse si on maintient la m�me touche

    private bool lastKeyWasUp = true;        // Savoir si la derni�re touche �tait la fl�che du haut (initialis� � "haut" pour le premier appui)

    private void Awake()
    {
        // Initialisation des contr�les
        controls = new PlayerControls();

        // Lier l'action Move � une m�thode pour capturer la valeur d'entr�e
        controls.Player.Move.performed += ctx => OnMove(ctx.ReadValue<float>());
        controls.Player.Move.canceled += ctx => OnStopMove();

        // Stocker la rotation initiale de la cam�ra
        initialCameraRotation = playerCamera.transform.localRotation;
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
        if (movementInput != 0)
        {
            // V�rifie si on a appuy� sur la m�me touche trop longtemps
            timePressingSameKey += Time.deltaTime;
            if (timePressingSameKey > maxPressTime)
            {
                // Si on d�passe le temps limite, la vitesse redescend � 0
                moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime * 2); // Perte de vitesse
                Debug.Log("Maintien trop long sur la m�me touche : vitesse r�duite");
            }
            else
            {
                // Sinon, on augmente la vitesse progressivement
                moveSpeed += acceleration * Time.deltaTime;
                moveSpeed = Mathf.Min(maxSpeed, moveSpeed); // Limiter � la vitesse max
            }
        }
        else
        {
            // Si aucune touche n'est press�e, la vitesse redescend lentement
            moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime);
        }

        // Appliquer l'inclinaison � la cam�ra selon la vitesse
        Quaternion targetRotation = initialCameraRotation * Quaternion.Euler(cameraTiltAngle * (moveSpeed / maxSpeed), 0, 0);
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, 0.1f);

        // Appliquer le mouvement du joueur en fonction de la vitesse
        Vector3 move = new Vector3(0, 0, moveSpeed) * Time.deltaTime;
        transform.Translate(move);
    }

    // Gestion de l'entr�e de mouvement
    private void OnMove(float input)
    {
        if (input > 0 && lastKeyWasUp == false) // Si on appuie sur fl�che haut apr�s fl�che bas
        {
            lastKeyWasUp = true;
            timePressingSameKey = 0f; // R�initialiser le temps pass� sur la touche
        }
        else if (input < 0 && lastKeyWasUp == true) // Si on appuie sur fl�che bas apr�s fl�che haut
        {
            lastKeyWasUp = false;
            timePressingSameKey = 0f; // R�initialiser le temps pass� sur la touche
        }

        movementInput = input; // Stocker la direction de l'input
    }

    // Quand on arr�te de bouger (lorsqu'aucune touche n'est appuy�e)
    private void OnStopMove()
    {
        movementInput = 0f; // Arr�ter le mouvement
    }
}
