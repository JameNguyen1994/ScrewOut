using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BalanceDTO
{
    public double userLTV;
    public double lastSyncUserLTV;

    public double publisherLTV;
    public double lastSyncPublisherLTV;

    public double userLTVInVirtualCurrency;
    public double lastSyncUserLTVInVirtualCurrency;
    // Constructor
    public BalanceDTO(double ltv, double lastLtvSync)
    {
        this.userLTVInVirtualCurrency = ltv;
        this.lastSyncUserLTVInVirtualCurrency = lastLtvSync;
    }
}