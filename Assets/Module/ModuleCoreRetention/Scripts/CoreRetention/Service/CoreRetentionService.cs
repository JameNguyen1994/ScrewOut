using Storage;
using UnityEngine;

public class CoreRetentionService
{
    public static Sprite GetLevelThumnail(int level)
    {
        string thumbnail = LevelMapService.GetLevelThumbnail(level);

        Sprite result = Resources.Load<Sprite>($"LevelMap/{thumbnail}");

        if (result == null)
        {
            result = Resources.Load<Sprite>($"LevelMap/Lv_1_Candy");
        }

        return result;
    }

    public static void ActiveWinLevelEvent()
    {
        EditorLogger.Log("[CoreRetention] ActiveWinLevelEvent");

        var rewardData = Db.storage.RewardData.DeepClone();
        rewardData.eventWinLevel = true;
        Db.storage.RewardData = rewardData;
    }

    public static bool IsWinLevelEventActive()
    {
        return Db.storage.RewardData.eventWinLevel;
    }

    public static void DoneWinLevelEvent()
    {
        EditorLogger.Log("[CoreRetention] DoneWinLevelEvent");

        var rewardData = Db.storage.RewardData.DeepClone();
        rewardData.eventWinLevel = false;
        Db.storage.RewardData = rewardData;
    }
}
