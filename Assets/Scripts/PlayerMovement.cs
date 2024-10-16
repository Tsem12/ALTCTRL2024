using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Vitesse du d�placement
    private PlayerControls controls; // Instance des contr�les
    private Rigidbody rb;         // Le Rigidbody pour g�rer les mouvements physiques
    private float movementInput;  // Stocke la valeur du mouvement (-1 pour reculer, +1 pour avancer)

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();  // Associe le Rigidbody

        // Initialiser les contr�les
        controls = new PlayerControls();

        // Lier l'action Move � une m�thode de retour pour capturer la valeur d'entr�e
        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => movementInput = 0f; // Arr�ter le mouvement quand la touche est rel�ch�e
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

    private void FixedUpdate()
    {
        // Appliquer le mouvement sur l'axe Z en fonction de l'input (-1 = reculer, 1 = avancer)
        Vector3 move = new Vector3(0, 0, movementInput) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);
    }
}