using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Vitesse du déplacement
    private PlayerControls controls; // Instance des contrôles
    private Rigidbody rb;         // Le Rigidbody pour gérer les mouvements physiques
    private float movementInput;  // Stocke la valeur du mouvement (-1 pour reculer, +1 pour avancer)

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();  // Associe le Rigidbody

        // Initialiser les contrôles
        controls = new PlayerControls();

        // Lier l'action Move à une méthode de retour pour capturer la valeur d'entrée
        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => movementInput = 0f; // Arrêter le mouvement quand la touche est relâchée
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

    private void FixedUpdate()
    {
        // Appliquer le mouvement sur l'axe Z en fonction de l'input (-1 = reculer, 1 = avancer)
        Vector3 move = new Vector3(0, 0, movementInput) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);
    }
}