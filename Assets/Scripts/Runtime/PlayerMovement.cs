using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [SerializeField] private SFX vertigeSound;

    [Header("Speed")]
    [SerializeField] private float moveSpeed;              // Vitesse actuelle du joueur
    [SerializeField] private float maxSpeed;               // Vitesse maximale
    [SerializeField] private float acceleration;           // Taux d'acc�l�ration
    [SerializeField] private float distance;

    // Variables pour la gestion du contr�le par alternance
    private float timePressingSameKey = 0f;  // Temps pass� � maintenir la m�me touche
    [SerializeField] private float maxPressTime = 1f;          // Temps maximal avant de perdre de la vitesse si on maintient la m�me touche
    private bool lastKeyWasUp = true;        // Savoir si la derni�re touche �tait la fl�che du haut (initialis� � "haut" pour le premier appui)

    private float movementInput;         // Stocke l'input de mouvement (-1 pour reculer, 1 pour avancer)
    private PlayerControls controls;     // Instance des contr�les

    [Header("Vertigo Settings")]
    [SerializeField] private float timeBeforeVertigo = 3f; // Temps avant de d�clencher les effets de vertige
    private float idleTimer = 0f;                          // Temps d'immobilit�
    private bool isVertigoActive = false;                           // Savoir si le joueur est immobile

    [Header("Jump")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpDuration;
    private bool isJumping = false;

    public static UnityEvent OnStartVertigoEvent = new UnityEvent();
    public static UnityEvent OnStopVertigoEvent = new UnityEvent();
    public static UnityEvent OnDroneEvent = new UnityEvent();

    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("plus d'une instance de PlayerMovement dans la scene");
            return;
        }
        instance = this;
        // Initialisation des contr�les
        controls = new PlayerControls();

        // Lier l'action Move � une m�thode pour capturer la valeur d'entr�e
        controls.Player.Move.performed += ctx => OnMove(ctx.ReadValue<float>());
        controls.Player.Move.canceled += ctx => OnStopMove();

        initialPlayerPosition = transform.position;
        initialPlayerRotation = transform.rotation;
    }

    private void OnEnable()
    {
        // Activer les contr�les
        controls.Enable();
        GameManager.OnRespawnEvent.AddListener(OnRespawn);
        OnStartVertigoEvent.AddListener(PlayRandomVertigoSound);
        OnDroneEvent.AddListener(OnDrone);
        controls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        controls.Disable();
        GameManager.OnRespawnEvent.RemoveAllListeners();
        OnStartVertigoEvent.RemoveAllListeners();
        OnDroneEvent.RemoveAllListeners();
        controls.Player.Jump.performed -= OnJumpPerformed;
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

            // Appeler l'�v�nement StopIdle lorsque le joueur recommence � bouger
            if (isVertigoActive && moveSpeed != 0)
            {
                OnStopVertigoEvent.Invoke(); // D�clenche l'�v�nement pour arr�ter les effets de vertige
                isVertigoActive = false;
                // R�initialiser le timer d'immobilit�
                idleTimer = 0f;
            }


            // V�rifie si on a appuy� sur la m�me touche trop longtemps
            timePressingSameKey += Time.deltaTime;

            if (timePressingSameKey > maxPressTime)
            {
                // Si on d�passe le temps limite, la vitesse redescend � 0
                //moveSpeed = Mathf.Max(0f, moveSpeed - acceleration * Time.deltaTime * 2); // Perte de vitesse
                moveSpeed = 0f;
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

        // *** MISE � JOUR IMPORTANTE : Incr�menter l'idleTimer en fonction de la vitesse r�elle ***
        if (moveSpeed == 0)
        {
            idleTimer += Time.deltaTime;
            // Si le joueur est immobile depuis assez longtemps
            if (idleTimer >= timeBeforeVertigo && !isVertigoActive && GameManager.instance.GetHasMoved() && !WindScript.instance.GetIsWindBlowing())
            {
                // D�clenche l'�v�nement d'immobilit�
                OnStartVertigoEvent.Invoke();
                isVertigoActive = true;
                
            }
        }
        else
        {
            // R�initialiser l'idleTimer si la vitesse n'est pas nulle
            idleTimer = 0f;
        }

        // Appliquer le mouvement du joueur en fonction de la vitesse
        int numberOfPigeon = PigeonManager.instance.PigeonAmountOnPerch;
        Vector3 move = new Vector3(0, 0, moveSpeed * Mathf.Pow(0.8f, numberOfPigeon) * Time.deltaTime);
        transform.Translate(move);
        distance += moveSpeed * Time.deltaTime;
    }

    public void PlayRandomVertigoSound()
    {
        vertigeSound.PlaySfx();
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

    public void ResetPlayerTransform()
    {
        transform.position = initialPlayerPosition;
        transform.rotation = initialPlayerRotation;
    }

    public float GetDistance()
    {
        return distance;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (WindScript.instance.GetIsWindBlowing())
        {
            GameManager.OnLoseEvent?.Invoke();
        }
        else
        {
            StartCoroutine(JumpCoroutine());
        }
    }

    private IEnumerator JumpCoroutine()
    {
        Debug.Log("j'applique la coroutine de saut");
        isJumping = true;

        // Sauvegarder la position initiale de la cam�ra avant le saut
        Vector3 startPosition = transform.localPosition;

        float elapsedTime = 0f;

        // L'effet du saut consiste � monter puis � redescendre, donc on va animer cela en deux phases (aller-retour)
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Utiliser un facteur sinuso�dal pour simuler un mouvement de saut r�aliste (monter puis redescendre)
            float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            // Appliquer la position verticale pendant le saut (en ajoutant l'offset � la position initiale)
            transform.localPosition = new Vector3(
                startPosition.x,                     // Garder la position X constante
                startPosition.y + heightOffset,       // Appliquer l'offset pour le saut sur Y
                startPosition.z                      // Garder la position Z constante
            );

            // Attendre la prochaine frame avant de continuer
            yield return null;
        }

        // S'assurer que la cam�ra revient exactement � sa position initiale � la fin du saut
        transform.localPosition = startPosition;

        isJumping = false;
    }

    public void OnDrone()
    {
        if (!isJumping)
        {
            GameManager.OnLoseEvent?.Invoke();
        }
    }

    public void OnRespawn()
    {
        isVertigoActive = false;
        distance = 0f;
        ResetPlayerTransform();
    }
}
