using System;
using System.Collections.Generic;
using UnityEngine;

public class MychipOfferwall : OfferwallBase
{
    MCOfferwallObject mcofferwall;
    
    public override void Show()
    {
        if (!IsInitialized)
        {
            return;
        }
        
        mcofferwall.ShowOfferwall();
    }

    public override void Init(Dictionary<string, object> data)
    {
        if (IsInitialized)
        {
            return;
        }
        
        mcofferwall = MCOfferwallObject.Instance;
        RegisterEvent();
        IsInitialized = true;
    }

    public override void FetchReward()
    {
        // my chip auto fetch reward, not need manual fetch
    }

    void RegisterEvent()
    {
        if (!mcofferwall)
        {
            return;
        }
        
        mcofferwall.OnRewardReceived.AddListener(OnOfferwallReceived);
        mcofferwall.OnRewardError.AddListener(OnRewardError);
        mcofferwall.OnOfferwallClosed.AddListener(OnOfferwallClosed);
    }

    private void OnOfferwallClosed()
    {
        InvokeClose(null);
    }

    private void OnRewardError(Exception error)
    {
        print($"OnRewardError: {error.Message}");
    }

    [EasyButtons.Button]
    void TestEvent()
    {
        OnOfferwallReceived(new RewardDTO(0.5, 100));
    }
    
    void OnOfferwallReceived(RewardDTO reward)
    {
        Dictionary<string, object> imressionData = new Dictionary<string, object>
        {
            { "revenue", reward.GetRevenue() },
            { "currency", "USD"},
            { "adUnitId", Application.platform == RuntimePlatform.Android? mcofferwall.AdUnitIdAndroid : mcofferwall.AdUnitIdIos},
            { "adType", "offerwall" },
            { "networkName", "my chip" },
            { "vc", reward.GetRewardInVirtualCurrency()}
        };
        
        InvokeImpression(imressionData);
    }
}