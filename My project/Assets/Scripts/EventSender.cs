using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class EventSender : MonoBehaviour
{
    public string serverUrl;

    public async Task SendEventsAsync(List<EventData> events, Action<List<EventData>> onFail)
    {
        if (events.Count == 0)
        {
            return;
        }

        // Преобразование списка событий в JSON
        string json = JsonUtility.ToJson(new {events});

        using (HttpClient client = new())
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(serverUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Events sent successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to send events: {response.StatusCode} - {response.ReasonPhrase}");
                    onFail?.Invoke(events);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                onFail?.Invoke(events);
            }
        }
    }
}