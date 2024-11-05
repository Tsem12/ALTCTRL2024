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
    [Header("Événements par distance")]
    [SerializeField] private List<DistanceEvent> distanceEvents; // Liste ordonnée d'événements

    private int currentEventIndex = 0; // L'index de l'événement à vérifier (commence par le premier)

    private void OnEnable()
    {
        GameManager.OnRespawnEvent.AddListener(OnRespawn);
    }

    private void OnDisable()
    {
        GameManager.OnRespawnEvent.RemoveAllListeners();
    }

    private void Update()
    {
        // Si tous les événements ont déjà été déclenchés, on n'a plus rien à vérifier
        if (currentEventIndex >= distanceEvents.Count)
            return;

        // Obtenir la distance parcourue par le joueur
        float distanceTravelled = PlayerMovement.instance.GetDistance();

        // Vérifier si la distance parcourue atteint ou dépasse le seuil du prochain événement
        if (distanceTravelled >= distanceEvents[currentEventIndex].distanceThreshold)
        {
            // Déclencher l'événement
            distanceEvents[currentEventIndex].onDistanceReached.Invoke();

            // Passer à l'événement suivant
            currentEventIndex++;
        }
    }

    public void OnRespawn()
    {
        currentEventIndex = 0;
    }
}
