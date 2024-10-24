using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] // La structure s�rialisable pour l'inspecteur Unity
public struct DistanceEvent
{
    public float distanceThreshold;   // Distance pour d�clencher cet �v�nement
    public UnityEvent onDistanceReached; // L'�v�nement � d�clencher
}

public class EventManager : MonoBehaviour
{
    [Header("R�f�rence au joueur")]
    [SerializeField] private PlayerMovement playerMovement; // R�f�rence au script de mouvement du joueur

    [Header("�v�nements par distance")]
    [SerializeField] private List<DistanceEvent> distanceEvents; // Liste ordonn�e d'�v�nements

    private int currentEventIndex = 0; // L'index de l'�v�nement � v�rifier (commence par le premier)

    private void Update()
    {
        // Si tous les �v�nements ont d�j� �t� d�clench�s, on n'a plus rien � v�rifier
        if (currentEventIndex >= distanceEvents.Count)
            return;

        // Obtenir la distance parcourue par le joueur
        float distanceTravelled = playerMovement.GetDistance();

        // V�rifier si la distance parcourue atteint ou d�passe le seuil du prochain �v�nement
        if (distanceTravelled >= distanceEvents[currentEventIndex].distanceThreshold)
        {
            // D�clencher l'�v�nement
            distanceEvents[currentEventIndex].onDistanceReached.Invoke();

            // Passer � l'�v�nement suivant
            currentEventIndex++;
        }
    }
}
