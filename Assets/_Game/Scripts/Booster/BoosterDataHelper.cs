using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterDataHelper : Singleton<BoosterDataHelper>
{
    [SerializeField] private BoosterDataSO boosterDataSO;
    public BoosterData GetBoosterData(BoosterType boosterType)
    {
        var booster = boosterDataSO.data.Find(x => x.boosterType == boosterType);
        
        if (booster==null)
        {
            Debug.Log($"NULL Booster_Hammer Data With Type {boosterType}");
        }
        
        return booster;
    }
}
