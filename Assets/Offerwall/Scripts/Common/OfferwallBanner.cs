using Storage;
using System.Collections.Generic;
using UnityEngine;

public class OfferwallBanner : MonoBehaviour
{
    public void Init(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void OnShowListOfferwallClick()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            ["placement"] = OfferwallController.Instance.Placement,
            ["level_id"] = Db.storage.USER_INFO.level.GetDecrypted()
        };

        GameAnalyticsSDK.GameAnalytics.NewDesignEvent("OFFERWALL_OPEN", dict);
        OfferwallController.Instance.ShowOfferwallList();
    }
}