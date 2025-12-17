using Assets.MCOfferwallSDK.Scripts.MCOfferwallSDK.Domain;
using Assets.Scripts.MCOfferwallSDK.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.MCOfferwallSDK.Domain;

public class MCOfferwallObject : MonoBehaviour
{
    private MCOfferwallController offerwall;

    [SerializeField]
    public string AdUnitIdAndroid;
    [SerializeField]
    public string AdUnitIdIos;

    [SerializeField]
    public bool isDebug;

    public RewardEvent OnRewardReceived;
    public UnityEvent<Exception> OnRewardError;
    public UnityEvent OnOfferwallClosed;

    private BalanceService balanceService;
    private UserService userService;

    // Stores additional custom parameters set via MCOfferwallObject.Instance.SetParam.
    private QueryParameterBuilder customQueryParameters = new QueryParameterBuilder();

    public static MCOfferwallObject Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeComponents();
    }

    
    public void InitializeComponents()
    {
        if (offerwall == null)
        {
            offerwall = GetComponentInChildren<MCOfferwallController>();
            offerwall.OnOfferwallClosed += Offerwall_OnOfferwallClosed;
        }

        if (balanceService == null)
        {
            balanceService = new BalanceService();
            balanceService.OnBalanceErrorReceived += BalanceService_OnErrorReceived;
            balanceService.OnBalanceReceived += BalanceService_OnRewardReceived;
        }

        if (userService == null)
            userService = new UserService();
    }

    
    private void Offerwall_OnOfferwallClosed()
    {
        OnOfferwallClosed?.Invoke();
    }


    private void OnAppStartOrResume()
    {
        Debug.Log($"Running code on App Start or Resume {Instance.AdUnitIdAndroid}:{Instance.AdUnitIdIos}");
        string adUnitId = Application.platform == RuntimePlatform.Android ? Instance.AdUnitIdAndroid : Instance.AdUnitIdIos;

        if (OnRewardReceived != null && (OnRewardReceived.GetEventCount() > 0))
            balanceService.GetBalance(userService.GetOrCreateId(), adUnitId, isDebug);
    }

    private void Start()
    {
        OnAppStartOrResume();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            OnAppStartOrResume();
        }
    }

    private void BalanceService_OnRewardReceived(BalanceDTO balance)
    {
        Debug.Log("bonus");
        OnRewardReceived?.Invoke(new RewardDTO(
            balance.userLTV - balance.lastSyncUserLTV,
            balance.userLTVInVirtualCurrency - balance.lastSyncUserLTVInVirtualCurrency));
    }

    private void BalanceService_OnErrorReceived(Exception err)
    {
        Debug.Log(err.ToString());
        OnRewardError?.Invoke(err);
    }

    #region Public Methods

    /// <summary>
    /// Shows the offerwall using the platform-specific ad unit id and any custom parameters previously set.
    /// </summary>
    public void ShowOfferwall()
    {
        string adUnitId = Application.platform == RuntimePlatform.Android ? AdUnitIdAndroid : AdUnitIdIos;
        offerwall.Show(adUnitId, customQueryParameters);
    }

    /// <summary>
    /// Sets a persistent user identifier. If not set, a UUID is generated automatically on first use.
    /// </summary>
    /// <param name="userId">Your app's stable user id.</param>
    public void SetUserId(string userId)
    {
        userService.SetId(userId);
    }
    
    /// <summary>
    /// Sets the Apple Identifier for Advertisers (IDFA).
    /// </summary>
    /// <param name="idfa">Raw IDFA string.</param>
     public void SetIDFA(string idfa)
    {
        userService.SetIDFA(idfa);
    }

    /// <summary>
    /// Sets the Google Advertising ID (GAID).
    /// </summary>
    /// <param name="gaid">Raw GAID string.</param>
    public void SetGAID(string gaid)
    {
        userService.SetGAID(gaid);
    }

    /// <summary>
    /// Sets the user age. Expected range is 0–100 (inclusive). Any other value is treated as invalid and will clear stored age.
    /// </summary>
    /// <param name="age">Age in years (0–100).</param>
    public void SetAge(int age)
    {
        userService.SetAge(age);
    }

    /// <summary>
    /// Sets the user gender to be sent to the offerwall backend.
    /// </summary>
    /// <param name="gender">One of the defined <see cref="MCGenderEnum"/> values.</param>
    public void SetGender(MCGenderEnum gender)
    {
        userService.SetGender(gender);
    }

    /// <summary>Sets affiliate sub-parameter 1 (string passthrough).</summary>
    /// <param name="aff_sub1">Raw string value.</param>
    public void SetAffSub1(string aff_sub1)
    {
        userService.SetAffSub1(aff_sub1);
    }

    /// <summary>Sets affiliate sub-parameter 2 (string passthrough).</summary>
    /// <param name="aff_sub2">Raw string value.</param>
    public void SetAffSub2(string aff_sub2)
    {
        userService.SetAffSub2(aff_sub2);
    }
    
    /// <summary>Sets affiliate sub-parameter 3 (string passthrough).</summary>
    /// <param name="aff_sub3">Raw string value.</param>
    public void SetAffSub3(string aff_sub3)
    {
        userService.SetAffSub3(aff_sub3);
    }

    /// <summary>Sets affiliate sub-parameter 4 (string passthrough).</summary>
    /// <param name="aff_sub4">Raw string value.</param>
    public void SetAffSub4(string aff_sub4)
    {
        userService.SetAffSub4(aff_sub4);
    }

    /// <summary>Sets affiliate sub-parameter 5 (string passthrough).</summary>
    /// <param name="aff_sub5">Raw string value.</param>
    public void SetAffSub5(string aff_sub5)
    {
        userService.SetAffSub5(aff_sub5);
    }

    /// <summary>
    /// Public interface for setting additional custom parameters.
    /// For example: MCOfferwallObject.Instance.SetParam("custom_param", "value");
    /// </summary>
    public void SetParam(string key, string value)
    {
        customQueryParameters.SetParam(key, value);
    }

    #endregion
}



