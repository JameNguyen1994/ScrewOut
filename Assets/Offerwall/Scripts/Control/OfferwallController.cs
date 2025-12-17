using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PS.Analytic.Event;
using PS.Utils;
using UnityEngine;
using UnityEngine.Events;

public sealed class OfferwallController : Singleton<OfferwallController>
{
    [SerializeField] private OfferwallBase mcOfferwall;
    [SerializeField] private OfferwallBase bitlabOfferwall;
    [SerializeField] private UIManager uiManager;
    
    public bool IsInitialized { get; private set; }
    private string placement = "Unknown";
    public string Placement => placement;
    private IOwDb db;
    public IOwDb Db => db;
    private string remoteData;

    public static event UnityAction<int> OnEarnCoinEvent; // event fire when press claim coin 

    public void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        db = new OwDb();
        RegisterEvent();
        
        mcOfferwall.Init(null);
        try
        {
            bitlabOfferwall.Init(null);
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>[Offerwall]</color> Bitlab Offerwall Init Error: {e.Message}");
        }
        
        IsInitialized = true;
    }

    public void SetRemoteData(string data)
    {
        remoteData = data;
    }
    
    public void ShowNotify(int notifyId, UnityAction<Dictionary<string, object>> callback)
    {
        uiManager.Notify.Show(new Dictionary<string, object>
        {
            ["spr_index"] = notifyId
        } ,callback);
    }

    public void ShowOfferwallList()
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(remoteData);
        uiManager.PopupOfferList.Show(data,null);
    }

    public void HideOfferwallList()
    {
        uiManager.PopupOfferList.Hide();
    }
    
    public void SetPlacement(string placement)
    {
        if (!IsInitialized)
        {
            return;
        }
        
        this.placement = placement;
    }
    
    public void OnLaunchMCOfferwall()
    {
        if (!IsInitialized)
        {
            return;
        }
        
        mcOfferwall.Show();
        GameAnalyticEvent.Event("OFFERWALL_CLICKED", new Dictionary<string, object>
        {
            {"placement", placement},
            { "partner_name", "my chip" },
            { "level_id" , Storage.Db.storage.USER_INFO.level.GetDecrypted() }
        });
    }

    public void OnLaunchBitlabOfferwall()
    {
        if (!IsInitialized)
        {
            return;
        }
        
        bitlabOfferwall.Show();
        GameAnalyticEvent.Event("OFFERWALL_CLICKED", new Dictionary<string, object>
        {
            {"placement", placement},
            { "partner_name", "bitlabs" },
            { "level_id" , Storage.Db.storage.USER_INFO.level.GetDecrypted() }
        });
    }

    public void RegisterEvent()
    {
        mcOfferwall.OnCloseEvent += OnCloseOfferwallEvent;
        bitlabOfferwall.OnCloseEvent += OnCloseOfferwallEvent;
        
        mcOfferwall.OnImpressionEvent += OnImpressionEvent;
        bitlabOfferwall.OnImpressionEvent += OnImpressionEvent;
    }

    private void OnImpressionEvent(Dictionary<string, object> data)
    {
        double vc = (double) data["vc"];
        
        float vcData = db.GetData("VC", 0.0f);
        vcData += (float)vc;
        db.SetData("VC", vcData);
        
        UnityMainThreadDispatcher.Instance.Enqueue(OnSendImpressionDataEvent, data);
    }
    
    void OnSendImpressionDataEvent(Dictionary<string, object> data)
    {
        CancelInvoke(nameof(ShowNotificationVerify));
        double rev = (double) data["revenue"];
        string currency = (string) data["currency"];
        string adUnitId = (string) data["adUnitId"];
        string adType = (string) data["adType"];
        string networkName = (string) data["networkName"];
        
        SingularAdData singularAdData = new SingularAdData(networkName, currency, rev);
        singularAdData.WithAdUnitId(adUnitId);
        singularAdData.WithAdType(adType);
        singularAdData.WithNetworkName(networkName);
        SingularSDK.AdRevenue(singularAdData);
        
        GameAnalyticEvent.Event("OFFERWALL_IMPRESSION", new Dictionary<string, object>
        {
            { "revenue", rev},
            { "currency", currency},
            { "adUnitId", adUnitId},
            { "adType", adType },
            { "partner_name", networkName }
        });
        
        GameAnalyticEvent.Event("OFFER_REWARD_RECEIVED", new Dictionary<string, object>
        {
            {"coin_amount", data["vc"]},
            { "partner_name", networkName }
        });
        
        int vcInt = (int)db.GetData("VC", 0.0f);

        if (vcInt > 0)
        {
            uiManager.PopupOfferCompleted.Show(new Dictionary<string, object>()
            {
                ["vc"] = vcInt
            }, (callbackData) =>
            {
                var vcData = db.GetData("VC", 0.0f);
                vcData -= (int) callbackData["vc"];
                db.SetData("VC", vcData);
                OnEarnCoinEvent?.Invoke(vcInt);
            });
        }
    }

    private void OnCloseOfferwallEvent(Dictionary<string, object> arg0)
    {
        ShowNotify(0, null);
        CancelInvoke(nameof(ShowNotificationVerify));
        Invoke(nameof(ShowNotificationVerify), 300);
    }

    void ShowNotificationVerify()
    {
        ShowNotify(1, null);
    }
}
