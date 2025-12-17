#if UNITY_FIREBASE
using Firebase;
using Firebase.Analytics;
#endif
using PS.Ad;
using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAnalyticController : Singleton<FirebaseAnalyticController>
{

    static bool isInitialized;

    protected override void CustomAwake()
    {
        if (!isInitialized)
        {
            Initializing();
        }
    }

    private void Initializing()
    {
#if UNITY_FIREBASE 
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result != DependencyStatus.Available)
            {
                print($"can not fix dependency with status: {task.Result}");
                isInitialized = false;
                return;
            }
        
            isInitialized = true;
        });
#endif
    }

    public static void TrackingLevelStart(string level, string mode)
    {
#if UNITY_FIREBASE
        if (!isInitialized) { return; }

        Parameter[] lvlStartParams =
        {
            new Parameter(FirebaseAnalytics.ParameterLevel, level),
            new Parameter("level_mode", mode),
        };
        
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, lvlStartParams);
#endif
    }

    public static void TrackingLevelEnd(string level, string mode, string success)
    {
#if UNITY_FIREBASE
        Debug.Log("Firebase start iap");
        if (!isInitialized) { return; }

        Parameter[] lvlEndParams =
        {
            new Parameter(FirebaseAnalytics.ParameterLevel, level),
            new Parameter("level_mode", mode),
            new Parameter(FirebaseAnalytics.ParameterSuccess, success)
        };
        Debug.Log("Firebase iap");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, lvlEndParams);
#endif
    }

    public static void TrackingIAP( string id, string price, string currency)
    {
#if UNITY_FIREBASE
        if (!isInitialized) { return; }

        Parameter[] iapRevenueParams =
        {
        
            new Parameter("product_id", id),
            new Parameter("price", price),
            new Parameter("currency", currency)
        };
        
        FirebaseAnalytics.LogEvent("iap_sdk", iapRevenueParams);
#endif
    }

    public static void TrackingAdRevenue(double revenue, string adUnitId, string adType, string precision,
            string networkName, string placementName)
    {
#if UNITY_FIREBASE
        if (!isInitialized) { return; }
        Debug.Log("Firebase Tracking ads done");

        Parameter[] impressionParameters =
        {
            new Parameter("ad_platform", "AppLovin"),
            new Parameter("ad_source", networkName),
            new Parameter("ad_unit_name", adUnitId),
            new Parameter("ad_format",adType),
            new Parameter("value",revenue),
            new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif

    }

  
}