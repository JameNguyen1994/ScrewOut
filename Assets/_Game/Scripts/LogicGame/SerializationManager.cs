using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using Storage;

public class SerializationManager
{
    public static bool IsQuitGame = false;

    public static void SaveDataInGamePlayAll(bool isSave = false)
    {
        if (!BoosterHandlerHammer.IsOver5Seconds() || IsQuitGame)
        {
            return;
        }

        if (!isSave)
        {
            return;
        }

        if (LevelController.Instance == null || LevelController.Instance.IsReload)
        {
            ClearAllDataInGamePlay();
            return;
        }

        if (LevelController.Instance != null)
        {
            if (LevelController.Instance.Level == null || !LevelController.Instance.VerifyCanSave())
            {
                ClearAllDataInGamePlay();
                return;
            }
        }

        LevelController.Instance.Level.Serialize();
        LevelController.Instance.Serialize();
        LevelController.Instance.SecretBox.Serialize();
        LevelController.Instance.BaseBox.Serialize();
        ScrewBlockedRealTimeController.Instance.Serialize();

        PlayerPrefsSaveTime.SaveNow();

        EditorLogger.Log(">>>> SaveDataInGamePlayAll");
    }

    public static void ClearAllDataInGamePlay()
    {
        SerializationService.ClearData(Define.SECRET_BOX_ID);
        SerializationService.ClearData(Define.LEVEL_CONTROLLER_ID);

        for (int i = 0; i < Define.IDENTIFIERS.Length; i++)
        {
            SerializationService.ClearData(Define.IDENTIFIERS[i].Value);
        }

        PlayerPrefsSaveTime.Clear();

        EditorLogger.Log(">>>> ClearAllDataInGamePlay");
    }

    public static async UniTask InitializeFromSaveAll()
    {
        ScrewBlockedRealTimeController.Instance.InitFormSave();
        LevelController.Instance.InitializeFromSave();
        LevelController.Instance.SecretBox.InitializeFromSave();
        LevelController.Instance.BaseBox.InitializeFromSave();
        await UniTask.Delay(1000);
        LevelController.Instance.Level.InitializeFromSave();
        EditorLogger.Log(">>>> InitializeFromSave");
    }

    public static bool HaveDataToReloadGamePlay()
    {
        if (PlayerPrefsSaveTime.IsNoneData())
        {
            return false;
        }

        if (SerializationService.IsNoneData(Define.SECRET_BOX_ID)
         || SerializationService.IsNoneData(Define.LEVEL_CONTROLLER_ID))
        {
            return false;
        }

        for (int i = 0; i < Define.IDENTIFIERS.Length; i++)
        {
            if (SerializationService.IsNoneData(Define.IDENTIFIERS[i].Value))
            {
                return false;
            }

            BoxData box = SerializationService.DeserializeObject<BoxData>(Define.IDENTIFIERS[i].Value);

            if (box == null)
            {
                return false;
            }
        }

        if (Db.storage.ScrewBlockedRealTimeData == null
         || IsNoneDataDict(Db.storage.ScrewBlockedRealTimeData.DicTotalScrew)
         || IsNoneDataDict(Db.storage.ScrewBlockedRealTimeData.DicCurrBlockedScrew)
         || IsNoneDataDict(Db.storage.ScrewBlockedRealTimeData.DicCurrScrewResolved)
         || IsNoneDataDict(Db.storage.ScrewBlockedRealTimeData.DicTotalScrew)
         || IsNoneDataDict(Db.storage.ScrewBlockedRealTimeData.DicCurrScrew))
        {
            return false;
        }

        BaseTrayData baseTray = SerializationService.DeserializeObject<BaseTrayData>(Define.LEVEL_CONTROLLER_ID);

        if (baseTray == null)
        {
            return false;
        }

        BoxData boxData = SerializationService.DeserializeObject<BoxData>(Define.SECRET_BOX_ID);

        if (boxData == null)
        {
            return false;
        }

        return true;
    }

    private static bool IsNoneDataDict(Dictionary<ScrewColor, int> data)
    {
        return data == null;
    }

    public static async UniTask<bool> ShowConfirmAsync(string message)
    {
        UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

        Action<bool> callback = (result) =>
        {
            tcs.TrySetResult(result);
        };

        PopupConfirm.Instance.InitData(callback, message);

        return await tcs.Task;
    }
}