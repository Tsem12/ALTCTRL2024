using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float moveSpeed;              // Vitesse actuelle du joueur
    [SerializeField] private float maxSpeed;               // Vitesse maximale
    [SerializeField] private float acceleration;           // Taux d'accélération

    // Variables pour la gestion du contrôle par alternance
    private float timePressingSameKey = 0f;  // Temps passé à maintenir la même touche
    [SerializeField] private float maxPressTime = 1f;          // Temps maximal avant de perdre de la vitesse si on maintient la même touche
    private bool lastKeyWasUp = true;        // Savoir si la dernière touche était la flèche du haut (initialisé à "haut" pour le premier appui)

    private float movementInput;         // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls;     // Instance des contrôles

    [Header("Idle Settings")]
    [SerializeField] private float timeBeforeVertigo = 3f; // Temps avant de déclencher les effets de vertige
    private float idleTimer = 0f;                          // Temps d'immobilité
    [SerializeField] private bool isIdle = false;                           // Savoir si le joueur est immobile

    [Header("Unity Events")]
    public UnityEvent onIdleEvent;
    public UnityEvent onStopIdleEvent;

    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private void Awake()
    {
        // Initialisation des contrôles
        controls = new PlayerControls();

        // Lier l'action Move à une méthode pour capturer la valeur d'entrée
        controls.Player.Move.performed += ctx => OnMove(ctx.ReadValue<float>());
        controls.Player.Move.canceled += ctx => OnStopMove();

        initialPlayerPosition = transform.position;
        initialPlayerRotation = transform.rotation;
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
        HandleMovement();
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetMovementInput()
    {
        return movementInput;
    }

    private void HandleMovement()
    {
        if (movementInput != 0)
        {
            if (!GameManager.instance.GetHasMoved())
            {
                GameManager.instance.SetHasMoved(true);
            }

            // Appeler l'événement StopIdle lorsque le joueur recommence à bouger
            if (isIdle && moveSpeed != 0)
            {
                onStopIdleEvent.Invoke(); // Déclenche l'événement pour arrêter les effets de vertige
                isIdle = false;
                // Réinitialiser le timer d'immobilité
                idleTimer = 0f;
            }


            // Vérifie si on a appuyé sur la même touche trop longtemps
            timePressingSameKey += Time.deltaTime;

            if (timePressingSameKey > maxPressTime)
            {
                // Si on dépasse le temps limite, la vitesse redescend à 0
                //moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime * 2); // Perte de vitesse
                moveSpeed = 0f;
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

        // *** MISE À JOUR IMPORTANTE : Incrémenter l'idleTimer en fonction de la vitesse réelle ***
        if (moveSpeed == 0)
        {
            idleTimer += Time.deltaTime;
            // Si le joueur est immobile depuis assez longtemps
            if (idleTimer >= timeBeforeVertigo && !isIdle && GameManager.instance.GetHasMoved())
            {
                Debug.Log("quoicoubeh");
                // Déclenche l'événement d'immobilité
                onIdleEvent.Invoke();
                isIdle = true;
            }
        }
        else
        {
            // Réinitialiser l'idleTimer si la vitesse n'est pas nulle
            idleTimer = 0f;
        }

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

    public void ResetPlayerTransform()
    {
        transform.position = initialPlayerPosition;
        transform.rotation = initialPlayerRotation;
    }
}
