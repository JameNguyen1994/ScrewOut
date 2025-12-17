using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;

public class CoinIncreaseFx : CurrencyIncreaseValueEffect
{
    protected override void OnValueChanged(object data)
    {
        try
        {
            int value = (int)data;
            int coin = 0;

            if (int.TryParse(txtValue.text, out coin))
            {
                coin += value;
            }

            txtValue.text = $"{coin}";
        }
        catch (Exception e)
        {
            Debug.LogError($"class {nameof(CoinIncreaseFx)} error: {e.Message}");
            return;
        }

        base.OnValueChanged(data);
    }
}
