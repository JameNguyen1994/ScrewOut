using System;
using System.Collections;
using System.Collections.Generic;
using OneSignalSDK;
using OneSignalSDK.InAppMessages;
using OneSignalSDK.Notifications;
using OneSignalSDK.User.Models;
using OneSignalSDK.User;
using UnityEngine;
using OneSignalSDK.Debug.Models;

public class OneSignalController : MonoBehaviour
{
    private static OneSignalController _instance;

    public static OneSignalController Instance => _instance;

    [SerializeField] private string appId;
    [SerializeField] private LogLevel logLevel;
    [SerializeField] private LogLevel alertLevel;
    [SerializeField] private bool requireUserPrivacyConsent;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    void Init()
    {
        OneSignal.Debug.LogLevel = logLevel;
        // Enable lines below to debug issues with OneSignal
        OneSignal.Debug.LogLevel = logLevel;
        OneSignal.Debug.AlertLevel = alertLevel;

        OneSignal.Initialize(appId);
        Debug.Log("OneSignal Initialize");
        // Setup the default live activity
        OneSignal.LiveActivities.SetupDefault();

        // Setting ConsentRequired to true will prevent the OneSignalSDK from operating until
        // PrivacyConsent is also set to true
        OneSignal.ConsentRequired = requireUserPrivacyConsent;

        // Setup the below to listen for and respond to events from notifications
        OneSignal.Notifications.Clicked += _notificationOnClick;
        OneSignal.Notifications.ForegroundWillDisplay += _notificationOnDisplay;
        OneSignal.Notifications.PermissionChanged += _notificationPermissionChanged;

        // Setup the below to listen for and respond to events from in-app messages
        OneSignal.InAppMessages.WillDisplay += _iamWillDisplay;
        OneSignal.InAppMessages.DidDisplay += _iamDidDisplay;
        OneSignal.InAppMessages.WillDismiss += _iamWillDismiss;
        OneSignal.InAppMessages.DidDismiss += _iamDidDismiss;
        OneSignal.InAppMessages.Clicked += _iamOnClick;

        // Setup the below to listen for and respond to state changes
        OneSignal.User.PushSubscription.Changed += _pushSubscriptionChanged;
        OneSignal.User.Changed += _userStateChanged;
        PromptForPush();
    }
    public async void PromptForPush()
    {

        Debug.Log("Opening permission prompt for push notifications and awaiting result...");

        var result = await OneSignal.Notifications.RequestPermissionAsync(true);

        if (result)
            Debug.Log("Notification permission accepeted");
        else
            Debug.Log("Notification permission denied");
        Debug.Log($"OneSignal Subscription ID: {OneSignal.Default.User.PushSubscription.Id}");

    }


    private void _notificationOnClick(object sender, NotificationClickEventArgs e)
    {

    }

    private void _notificationOnDisplay(object sender, NotificationWillDisplayEventArgs e)
    {
        var additionalData = e.Notification.AdditionalData != null
            ? Json.Serialize(e.Notification.AdditionalData)
                : null;

        e.Notification.Display();
    }

    private void _notificationPermissionChanged(object sender, NotificationPermissionChangedEventArgs e)
    {
    }

    private void _iamWillDisplay(object sender, InAppMessageWillDisplayEventArgs e)
    {
    }

    private void _iamDidDisplay(object sender, InAppMessageDidDisplayEventArgs e)
    {
    }

    private void _iamWillDismiss(object sender, InAppMessageWillDismissEventArgs e)
    {
    }

    private void _iamDidDismiss(object sender, InAppMessageDidDismissEventArgs e)
    {
    }

    private void _iamOnClick(object sender, InAppMessageClickEventArgs e)
    {
    }

    private void _pushSubscriptionChanged(object sender, PushSubscriptionChangedEventArgs e)
    {
    }

    private void _userStateChanged(object sender, UserStateChangedEventArgs e)
    {
    }

}
