using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;

public class StarIncreaseFx : CurrencyIncreaseValueEffect
{
    protected override void OnValueChanged(object data)
    {
        try
        {
            int value = (int)data;
            var userInfo = Db.storage.USER_INFO;
            userInfo.star += value;
            Db.storage.USER_INFO = userInfo;
            txtValue.text = $"{userInfo.star}";
        }
        catch (Exception e)
        {
            Debug.LogError($"class {nameof(StarIncreaseFx)} error: {e.Message}");
            return;
        }
        
        base.OnValueChanged(data);
    }
}
