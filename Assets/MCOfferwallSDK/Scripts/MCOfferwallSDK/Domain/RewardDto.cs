using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardDTO
{
    /// <summary>
    /// Publisher Revenue in configured currency
    /// send this value to your MMP or Analytics
    /// </summary>
    double _totalRevenue;

    /// <summary>
    /// Total amount of Virtual Currency to be assigned to the user
    /// </summary>
    double _virtualCurrencyReward;
    // Constructor

    public RewardDTO(double totalRevenue, double virtualCurrencyReward) {

        _totalRevenue = totalRevenue;
        _virtualCurrencyReward = virtualCurrencyReward;
    }

    public double GetRewardInVirtualCurrency() {
        return _virtualCurrencyReward;
    }
    public double GetRevenue()
    {
        return _totalRevenue;
    }



}