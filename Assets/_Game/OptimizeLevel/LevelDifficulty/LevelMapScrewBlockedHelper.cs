using System.Collections.Generic;
using UnityEngine;

public class LevelMapScrewBlockedHelper : MonoBehaviour
{
    [SerializeField] private List<TextAsset> lstJsonLevelScrewBlocked;


    public void InitLevel(int level, LevelMap levelMap)
    {
        /*if (IngameData.IS_GEN_SCREW_BLOCK)
            return;*/
        bool isValidLevel = false;
        if (level >= 1 && level <= lstJsonLevelScrewBlocked.Count)
        {
            var levelData = JsonUtility.FromJson<LevelScrewBlockedData>(lstJsonLevelScrewBlocked[level - 1].text);
            if (levelData != null)
            {
               // isValidLevel = true;
                levelMap.LstBlockedData = levelData.lstScrewBlockedData;
                levelMap.ConvertIndexToShape();
                Debug.Log($"[LevelMapScrewBlockedHelper] Initialized Level {level} with {levelData.lstScrewBlockedData.Count} ScrewBlockedData entries");
            }
        }
        if (!isValidLevel)
        {
            Debug.Log($"[LevelMapScrewBlockedHelper] No ScrewBlockedData found for Level {level}");
            levelMap.TestDetectAllScrew();

/*            levelMap.FakeData();*/
            levelMap.ConvertIndexToShape();

        }
    }
}
