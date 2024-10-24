using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] // La structure sérialisable pour l'inspecteur Unity
public struct DistanceEvent
{
    public float distanceThreshold;   // Distance pour déclencher cet événement
    public UnityEvent onDistanceReached; // L'événement à déclencher
}

public class EventManager : MonoBehaviour
{
    [Header("Référence au joueur")]
    [SerializeField] private PlayerMovement playerMovement; // Référence au script de mouvement du joueur

    [Header("Événements par distance")]
    [SerializeField] private List<DistanceEvent> distanceEvents; // Liste ordonnée d'événements

    private int currentEventIndex = 0; // L'index de l'événement à vérifier (commence par le premier)

    private void Update()
    {
        // Si tous les événements ont déjà été déclenchés, on n'a plus rien à vérifier
        if (currentEventIndex >= distanceEvents.Count)
            return;

        // Obtenir la distance parcourue par le joueur
        float distanceTravelled = playerMovement.GetDistance();

        // Vérifier si la distance parcourue atteint ou dépasse le seuil du prochain événement
        if (distanceTravelled >= distanceEvents[currentEventIndex].distanceThreshold)
        {
            // Déclencher l'événement
            distanceEvents[currentEventIndex].onDistanceReached.Invoke();

            // Passer à l'événement suivant
            currentEventIndex++;
        }
    }
}
