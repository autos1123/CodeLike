using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public static class AnalyticsEventNames
{
    public const string test_simple_event = "test_simple_event";
}

public class AnalyticsManager : MonoSingleton<AnalyticsManager>
{
    bool _initialized = false;
    bool _dataCollectionStarted = false;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    public async void Initialize()
    {
        if(_initialized) return;
        await UnityServices.InitializeAsync();
        _initialized = true;
        Debug.Log("[Analytics] Initialized");


        if(true)
            StartDataCollection();
    }

    public void StartDataCollection()
    {
        if(!_initialized || _dataCollectionStarted) return;
        AnalyticsService.Instance.StartDataCollection();
        _dataCollectionStarted = true;
        Debug.Log("[Analytics] Data collection started");
    }

    public void StopDataCollection()
    {
        if(!_initialized || !_dataCollectionStarted) return;
        AnalyticsService.Instance.StopDataCollection();
        _dataCollectionStarted = false;
        Debug.Log("[Analytics] Data collection stopped");
    }

    public void SendEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if(!_initialized)
        {
            Debug.LogWarning($"[Analytics] Not initialized yet. Queuing or dropping event: {eventName}");
            return;
        }
        AnalyticsService.Instance.CustomData(eventName, parameters ?? new Dictionary<string, object>());
        Debug.Log($"[Analytics] {eventName} SendEvent");
    }
}
