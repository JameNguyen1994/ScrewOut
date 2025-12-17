using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelDifficultyManager : Singleton<LevelDifficultyManager>
{
    //   [SerializeField] private List<TextAsset> levelFiles;       // Danh sách file JSON
    [SerializeField] private LevelDifficultyData currentLevel; // Level đang được load
    [SerializeField] private TextAsset defaultData; // Level đang được load
    [SerializeField] private int processEasy = -1;
    [SerializeField] private int currentProcess = 0;


    /// <summary>
    /// Load dữ liệu 1 level từ file JSON trong danh sách
    /// </summary>
    public void InitLevel(LevelMap levelMap)
    {
        /*        if (level < 1 || level > levelFiles.Count)
                {
                    Debug.LogError($"Level {level} không tồn tại trong danh sách!");
                    return;
                }

                TextAsset ta = levelFiles[level - 1]; // giả sử file thứ n tương ứng level n
                if (ta == null)
                {
                    Debug.LogError($"File cho Level {level} bị null!");
                    return;
                }

                try
                {
                    currentLevel = JsonUtility.FromJson<LevelDifficultyData>(ta.text);
                    Debug.Log($"Đã load Level {level} từ file {ta.name}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Lỗi parse JSON Level {level}: {e}");
                }*/
        var textAsset = levelMap.LevelMapDataJson;
        if (textAsset != null)
        {

            currentLevel = JsonUtility.FromJson<LevelDifficultyData>(textAsset.text);
            Debug.Log($"Đã load Level {levelMap.LevelId} từ file {textAsset.name}");
        }
        else
        {
            currentLevel = JsonUtility.FromJson<LevelDifficultyData>(defaultData.text);
            Debug.LogWarning($"LevelMapDataJson is null, loaded default data from {defaultData.name}");
        }


    }

    /// <summary>
    /// Lấy list<int> [ease, normal, hard] từ 1 process
    /// </summary>
    public List<int> GetPercents(int currentProcess)
    {
        this.currentProcess = currentProcess;
        if (currentProcess <= processEasy)
        {
            var strProcess = "";
            for (int i = 0; i < currentLevel.lstProcessData.Count; i++)
            {
                strProcess += $"Process:{currentLevel.lstProcessData[i].process} ";
            }
            Debug.Log($"BoxCollider Force Easy \n currentProcess:{currentProcess} processEasy:{processEasy} in {strProcess}");
            return new List<int>() { 100, 0, 0 };
        }
        if (currentLevel == null || currentLevel.lstProcessData == null)
            return new List<int>() { 0, 0, 0 };

        var process = currentLevel.lstProcessData.FindLast(p => p.process <= currentProcess);
        if (process == null || process.boxDifficulty == null)
        {
            return new List<int>() { 0, 0, 0 };
        }

        return new List<int>()
    {
        process.boxDifficulty.easePercent,
        process.boxDifficulty.normalPercent,
        process.boxDifficulty.hardPercent
    };
    }
    private int GetNextProcess()
    {
        var nextProcess = currentLevel.lstProcessData.Find(p => p.process > currentProcess);
        if (nextProcess == null)
        {
            return 101;
        }
        return nextProcess.process;
    }
    public void SetProcessEasy()
    {
        var nextProcess = GetNextProcess();
        processEasy = nextProcess;
    }

}
