using System;
using System.Collections.Generic;
using Rest.API;
using UnityEngine;

public class BitlabsOfferwall : OfferwallBase
{
    private string apiKey;

    [SerializeField] private string androidApiKey;
    [SerializeField] private string iosApiKey;
    
    
    private string uuid;
    private string token;
    IOwDb owDb;
    
    public override void Show()
    {
        if (!IsInitialized)
        {
            return;
        }
        
        BitLabs.LaunchOfferWall();
    }

    public override void Init(Dictionary<string, object> data)
    {
        if (IsInitialized)
        {
            return;
        }

        owDb = new OwDb();
        var userInfo = owDb.GetDataCustom("OW_USER_INFO", new OfferwallUserInfo() { token = "", uuid = "" });
        
        if (string.IsNullOrEmpty(userInfo.token) || string.IsNullOrEmpty(userInfo.uuid))
        {
            BAL bal = new BAL();
            bal.RegisterUser().ContinueWith(task =>
            {
                UnityMainThreadDispatcher.Instance.Enqueue(OnRegisterUserCallback, task.Result);
            });
            return;
        }
        
        Initialize(userInfo.uuid, userInfo.token);
        
    }

    void Initialize(string uuid, string token)
    {
#if UNITY_ANDROID
        apiKey = androidApiKey;
#endif
        
#if UNITY_IOS
        apiKey = iosApiKey;
#endif
        BitLabs.Init(apiKey, uuid);
        BitLabs.SetRewardCallback(gameObject.name);
        this.uuid = uuid;
        this.token = token;
        IsInitialized = true;
    }
    
    void OnRegisterUserCallback(ResponseResult<RegisterUserResponse> result)
    {
        if (result.Error != null)
        {
            print($"[Login] Error: {result.Error}");
            return;
        }

        var uInfo = new OfferwallUserInfo();
        uInfo.uuid = result.Data.UserId;
        uInfo.token = result.Data.Token;
        
        owDb.SetDataCustom("OW_USER_INFO", uInfo);
        
        Initialize(uInfo.uuid, uInfo.token);
    }
    
    [System.Reflection.Obfuscation(Exclude = true)]
    private void RewardCallback(string payout)
    {
        Debug.Log($"[BitLabs] on reward callback: {payout}");
        InvokeClose(null);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            return;
        }
        
        FetchReward();
    }

    #if UNITY_EDITOR
    [EasyButtons.Button]
    void TestFetch()
    {
        var userInfo = owDb.GetDataCustom("OW_USER_INFO", new OfferwallUserInfo() { token = "", uuid = "" });
        BAL bal = new BAL();
        bal.GetRewardBitLabResponse(userInfo.uuid, userInfo.token, Application.identifier).ContinueWith(task => OnRewardS2SCallback(task.Result));
    }
    #endif

    public override void FetchReward()
    {
        if (!IsInitialized)
        {
            return;
        }
        
        BAL bal = new BAL();
        bal.GetRewardBitLabResponse(uuid, token, Application.identifier).ContinueWith(task => OnRewardS2SCallback(task.Result));
    }
    
    private void OnRewardS2SCallback(ResponseResult<RewardBitLabResponse> result)
    {
        if (result.Error != null)
        {
            return;
        }

        UnityMainThreadDispatcher.Instance.Enqueue(OnRewardProdegeCallback, result.Data);
    }
    
    private void OnRewardProdegeCallback(RewardBitLabResponse reward)
    {
        
        var dataOw = owDb.GetDataCustom("OW_PRODEGE_DATA", new OfferwallData() { TotalRevenue = 0, TotalVirtualCurrency = 0 });
        
        double rev = reward.TotalRev - dataOw.TotalRevenue;
        double vc = reward.TotalValue - dataOw.TotalVirtualCurrency;
        
        Debug.Log($"[Offerwall] Reward: rev = {rev}, vc = {vc}");

        if (rev <= 0 && vc <= 0)
        {
            return;
        }
        
        owDb.SetDataCustom<OfferwallData>("OW_PRODEGE_DATA", new OfferwallData()
        {
            TotalRevenue = reward.TotalRev,
            TotalVirtualCurrency = reward.TotalValue
        });
        
        Dictionary<string, object> imressionData = new Dictionary<string, object>
        {
            { "revenue", rev },
            { "currency", "USD"},
            { "adUnitId", ""},
            { "adType", "offerwall" },
            { "networkName", "bitlabs" },
            { "vc", vc }
        };
        
        InvokeImpression(imressionData);
    }
}
