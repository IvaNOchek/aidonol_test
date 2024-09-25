using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private const string EVENTS_FILE_PATH = "events.dat";
    private List<EventData> eventsToUpload = new List<EventData>();

    public void AddEvent(EventData eventData)
    {
        eventsToUpload.Add(eventData);
        SaveEventsAsync().Forget();
    }

    public List<EventData> GetAndClearEvents()
    {
        List<EventData> eventsToSend = new List<EventData>(eventsToUpload);
        eventsToUpload.Clear();
        return eventsToSend;
    }

    public async UniTask SaveEventsAsync(List<EventData> events)
    {
        eventsToUpload.AddRange(events);
        await SaveEventsAsync();
    }

    private async UniTask SaveEventsAsync()
    {
        string json = JsonUtility.ToJson(new { events = eventsToUpload });
        await File.WriteAllTextAsync(EVENTS_FILE_PATH, json);
    }

    public async UniTask LoadEventsAsync()
    {
        if (File.Exists(EVENTS_FILE_PATH))
        {
            string json = await File.ReadAllTextAsync(EVENTS_FILE_PATH);
            var savedData = JsonUtility.FromJson<SavedEventData>(json);
            eventsToUpload.AddRange(savedData.events);
        }
    }

    [Serializable]
    private struct SavedEventData
    {
        public EventData[] events;
    }

    private async void Start()
    {
        await LoadEventsAsync();
    }
}