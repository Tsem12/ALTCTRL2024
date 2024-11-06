using UnityEngine;

public class MoveAndTeleport : MonoBehaviour
{
    public float distance = 10f;    // Distance paramétrable
    public float speed = 5f;        // Vitesse paramétrable
    private Vector3 startPosition;  // Position de départ
    private Vector3 targetPosition; // Position cible

    void Start()
    {
        // Stocke la position de départ
        startPosition = transform.position;

        // Calcule la position cible en fonction de la distance donnée
        targetPosition = startPosition + transform.forward * distance;
    }

    void Update()
    {
        // Déplace le GameObject vers la position cible
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Si le GameObject a atteint la position cible
        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            // Le téléporte à la position de départ
            transform.position = startPosition;
        }
    }


    void OnDrawGizmos()
    {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPosition, 0.2f);  

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.2f); 

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPosition, targetPosition);
    }
}