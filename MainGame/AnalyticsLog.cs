using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
using UnityEngine.Analytics;
#endif

public class AnalyticsLog : MonoBehaviour
{

    public static void Print(string title,string subtitle,string values="1")
    {

        Analytics.CustomEvent(title, new Dictionary<string, object> {
            {subtitle,values }
        });
    }
}
