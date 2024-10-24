using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private List<AudioClip> vertigeSound;

    [SerializeField] private WindScript windScript;

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

    [Header("Idle Settings")]
    [SerializeField] private float timeBeforeVertigo = 3f; // Temps avant de d�clencher les effets de vertige
    private float idleTimer = 0f;                          // Temps d'immobilit�
    [SerializeField] private bool isIdle = false;                           // Savoir si le joueur est immobile

    [Header("Unity Events")]
    public UnityEvent onIdleEvent;
    public UnityEvent onStopIdleEvent;

    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private void Awake()
    {
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
    }

    private void OnDisable()
    {
        // D�sactiver les contr�les
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

            // Appeler l'�v�nement StopIdle lorsque le joueur recommence � bouger
            if (isIdle && moveSpeed != 0)
            {
                onStopIdleEvent.Invoke(); // D�clenche l'�v�nement pour arr�ter les effets de vertige
                isIdle = false;
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
            if (idleTimer >= timeBeforeVertigo && !isIdle && GameManager.instance.GetHasMoved() && !windScript.isWindBlowing)
            {
                // D�clenche l'�v�nement d'immobilit�
                onIdleEvent.Invoke();
                isIdle = true;
                
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
        AudioClip clip = vertigeSound[Random.Range(0, vertigeSound.Count)];
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
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

    public void SetDistance(float newDistance)
    {
        distance = newDistance;
    }
}
