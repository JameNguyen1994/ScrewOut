using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;

public class ExpBoxIncreaseFx : CurrencyIncreaseValueEffect
{
    protected override void OnValueChanged(object data)
    {
        // try
        // {
        //     int value = (int)data;
        //     
        //     if (ExpBar.Instance != null)
        //     {
        //         var trkData = Db.storage.TRK_DATA.DeepClone();
        //
        //         if (LevelController.Instance.Level.LevelId % 2 == 0)
        //         {
        //             trkData.mulScoreExp += value;
        //         }
        //         else
        //         {
        //             trkData.singleScoreExp += value;
        //         }
        //         
        //         trkData.expOfSection += value;
        //
        //         Db.storage.TRK_DATA = trkData;
        //         ExpBar.Instance.AddExp(value);
        //             
        //     }
        //     // txtValue.text = $"{value}";
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError($"class {nameof(StarIncreaseFx)} error: {e.Message}");
        //     return;
        // }
        
        base.OnValueChanged(data);
    }
}
