// TestAnalyticsEvent.cs
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class TestAnalyticsEvent:MonoBehaviour
{
    void Start()
    {
        AnalyticsManager.Instance.SendEvent(AnalyticsEventNames.test_simple_event, new Dictionary<string, object>
            {
                { "score", 100 },
                { "time", 5.0f },
                { "itemName", "kkk" },
            });     
    }
}
