using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverBuyIAP : Singleton<CoverBuyIAP>
{
    [SerializeField] private GameObject cover;
    public void OnEnableCover(bool enable)
    {
        Debug.Log($"OnEnableCover {enable}");
        cover.SetActive(enable);
    }
}
