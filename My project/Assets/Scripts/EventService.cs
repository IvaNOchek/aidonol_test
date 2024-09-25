using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventService : MonoBehaviour
{
    public EventManager eventManager;
    public EventSender eventSender;
    public CooldownManager cooldownManager;

    private bool isSendingScheduled = false;

    private async void Start()
    {
        // Загрузка недоставленных событий при старте
        await eventManager.LoadEventsAsync();
    }

    private void Update()
    {
        if (cooldownManager.IsCooldownActive)
        {
            if (!isSendingScheduled)
            {
                isSendingScheduled = true;
                StartCoroutine(SendEvents());
            }
        }
    }

    public void TrackEvent(string type, string data)
    {
        EventData eventData = new EventData(type, data);
        eventManager.AddEvent(eventData);

        // Если таймер не активен, запускаем его
        if (!cooldownManager.IsCooldownActive)
        {
            cooldownManager.StartCooldown();
        }
    }

    private IEnumerator SendEvents()
    {
        isSendingScheduled = false;

        List<EventData> eventsToUpload = eventManager.GetAndClearEvents();
        if (eventsToUpload.Count == 0)
        {
            yield break;
        }

        Task sendTask = eventSender.SendEventsAsync(eventsToUpload, events =>
        {
            eventManager.SaveEventsAsync(events).Forget();
        });

        while (!sendTask.IsCompleted)
        {
            yield return null;
        }
    }
}